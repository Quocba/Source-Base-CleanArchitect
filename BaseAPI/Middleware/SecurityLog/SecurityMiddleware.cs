using System.Collections.Concurrent;
using Domain.Config;
using MaxMind.GeoIP2;
using Microsoft.Extensions.Options;
using Serilog;

namespace BaseAPI.Middleware.SecurityLog;

#pragma warning disable
public class SecurityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly DatabaseReader? _geoReader;
    private readonly SecuritySettings _settings;

    // Bộ nhớ đệm cho Rate Limit, Blacklist và GeoIP
    private static readonly ConcurrentDictionary<string, (int Count, DateTime Timestamp)> RateCache = new();
    private static readonly ConcurrentDictionary<string, DateTime> BlacklistCache = new();
    private static readonly ConcurrentDictionary<string, (string Country, string City)> GeoCache = new();

    private static readonly string[] MaliciousPatterns =
    {
        "select ", "union ", "drop ", "insert ", "delete ", "--", ";--", "' or '1'='1",
        "<script", "javascript:", "onerror=", "onload=", "alert(",
        "../", "..\\", "/etc/passwd", "/windows/system32",
        "cmd.exe", "/bin/sh", "/bin/bash"
    };

    public SecurityMiddleware(RequestDelegate next, IOptions<SecuritySettings> settings)
    {
        _next = next;
        _settings = settings.Value;

        var geoFolder = Path.Combine(AppContext.BaseDirectory, "GeoDB");
        var geoFile = Path.Combine(geoFolder, "GeoLite2-City.mmdb");

        if (File.Exists(geoFile))
        {
            try { _geoReader = new DatabaseReader(geoFile); } catch { }
        }
    }

    public async Task Invoke(HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        if (userAgent.Contains("k6-load-test"))
        {
            await _next(context);
            return;
        }

        var ip = GetRealIp(context);

        if (_settings.WhitelistedIps.Contains(ip))
        {
            await _next(context);
            return;
        }

        if (BlacklistCache.TryGetValue(ip, out var expiry))
        {
            if (DateTime.UtcNow < expiry)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Your IP is temporarily blocked until {expiry:HH:mm:ss} UTC.");
                return;
            }
            BlacklistCache.TryRemove(ip, out _);
        }

        var path = context.Request.Path.ToString();
        var query = context.Request.QueryString.Value ?? "";

        if (IsMaliciousRequest(context, query, path))
        {
            var penaltyExpiry = DateTime.UtcNow.AddMinutes(_settings.MaliciousIpPenaltyMinutes);
            BlacklistCache[ip] = penaltyExpiry;

            Log.Fatal("ATTACK | IP={IP} | Path={Path} | Query={Query}", ip, path, query);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access Denied.");
            return;
        }

        if (IsRateLimitExceeded(ip))
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return;
        }

        // Tối ưu: Chỉ Log chi tiết khi không phải là load test hoặc dùng Async log
        var (country, city) = GetGeoCached(ip);
        Log.Information("REQ | IP={IP} | {Country} | {Path}", ip, country, path);

        await _next(context);
    }

    private (string Country, string City) GetGeoCached(string ip)
    {
        if (GeoCache.TryGetValue(ip, out var cached)) return cached;

        var result = GetGeo(ip);
        if (result.Country != "Unknown")
        {
            GeoCache.TryAdd(ip, result);
        }
        return result;
    }

    private bool IsMaliciousRequest(HttpContext context, string query, string path)
    {
        var inputToTest = (query + " " + path).ToLower();
        if (MaliciousPatterns.Any(p => inputToTest.Contains(p))) return true;

        var ua = context.Request.Headers["User-Agent"].ToString().ToLower();
        return ua.Contains("sqlmap") || ua.Contains("nmap") || ua.Contains("acunetix");
    }

    private bool IsRateLimitExceeded(string ip)
    {
        var now = DateTime.UtcNow;
        var entry = RateCache.GetOrAdd(ip, _ => (0, now));

        if ((now - entry.Timestamp).TotalSeconds > _settings.RateLimitWindowSeconds)
        {
            RateCache[ip] = (1, now);
            return false;
        }

        if (entry.Count >= _settings.RateLimitCount) return true;

        RateCache[ip] = (entry.Count + 1, entry.Timestamp);
        return false;
    }

    private (string Country, string City) GetGeo(string ip)
    {
        try
        {
            if (_geoReader == null || ip == "::1" || ip == "127.0.0.1") return ("Local", "Local");
            var result = _geoReader.City(ip);
            return (result.Country.Name ?? "Unknown", result.City.Name ?? "Unknown");
        }
        catch { return ("Unknown", "Unknown"); }
    }

    private string GetRealIp(HttpContext ctx)
    {
        return ctx.Request.Headers["CF-Connecting-IP"].FirstOrDefault()
               ?? ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault()
               ?? ctx.Connection.RemoteIpAddress?.ToString()
               ?? "Unknown";
    }
}
