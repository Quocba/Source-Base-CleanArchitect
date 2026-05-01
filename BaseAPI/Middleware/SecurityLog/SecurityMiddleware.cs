using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using MaxMind.GeoIP2;
using Serilog;

#pragma warning disable
public class SecurityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly DatabaseReader? _geoReader;

    private static readonly ConcurrentDictionary<string, (int Count, DateTime Timestamp)> RateCache = new();

    private static readonly string[] SqliPatterns =
    {
        "select ", "union ", "drop ", "insert ", "delete ",
        "--", ";--", "' or '1'='1", "\" or \"1\"=\"1"
    };

    private static readonly string[] XssPatterns =
    {
        "<script", "javascript:", "onerror=", "onload="
    };

    public SecurityMiddleware(RequestDelegate next)
    {
        _next = next;

        var geoFolder = Path.Combine(AppContext.BaseDirectory, "GeoDB");
        var geoFile = Path.Combine(geoFolder, "GeoLite2-City.mmdb");

        if (!Directory.Exists(geoFolder))
            Directory.CreateDirectory(geoFolder);

        if (File.Exists(geoFile))
        {
            _geoReader = new DatabaseReader(geoFile);
        }
    }

    public async Task Invoke(HttpContext context)
    {
        var ip = GetRealIp(context);
        var ua = context.Request.Headers["User-Agent"].ToString();
        var path = context.Request.Path.ToString();
        var query = context.Request.QueryString.Value ?? "";

        var (country, city) = GetGeo(ip);

        if (IsRateLimitExceeded(ip))
        {
            Log.Warning("RATE_LIMIT | IP={IP}", ip);
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync("Too Many Requests");
            return;
        }

        if (SqliPatterns.Any(p => query.ToLower().Contains(p)))
            Log.Warning("SQLI | IP={IP} | QUERY={Query}", ip, query);

        if (XssPatterns.Any(p => query.ToLower().Contains(p)))
            Log.Warning("XSS | IP={IP} | QUERY={Query}", ip, query);

        Log.Information(
            "SECURITY | IP={IP} | Country={Country} | City={City} | UA={UA} | Path={Path} | Query={Query}",
            ip, country, city, ua, path, query
        );

        await _next(context);
    }

    private (string Country, string City) GetGeo(string ip)
    {
        try
        {
            if (_geoReader == null) return ("Unknown", "Unknown");
            var result = _geoReader.City(ip);
            return (result.Country.Name ?? "Unknown", result.City.Name ?? "Unknown");
        }
        catch { return ("Unknown", "Unknown"); }
    }

    private bool IsRateLimitExceeded(string ip)
    {
        var now = DateTime.UtcNow;
        var entry = RateCache.GetOrAdd(ip, _ => (0, now));

        if ((now - entry.Timestamp).TotalSeconds > 5)
        {
            RateCache[ip] = (1, now);
            return false;
        }

        if (entry.Count >= 10) return true;

        RateCache[ip] = (entry.Count + 1, entry.Timestamp);
        return false;
    }

    private string GetRealIp(HttpContext ctx)
    {
        return ctx.Request.Headers["CF-Connecting-IP"].FirstOrDefault()
               ?? ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault()
               ?? ctx.Connection.RemoteIpAddress?.ToString()
               ?? "Unknown";
    }
}
