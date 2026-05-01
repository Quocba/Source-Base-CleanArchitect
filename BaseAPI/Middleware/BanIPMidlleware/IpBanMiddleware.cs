using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace YourNamespace.Middleware
{
    public class IpBanMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, SlidingCounter> _ipRequestCount = new();
        private static readonly object _lock = new();
        private const int RequestLimit = 100;
        private static readonly TimeSpan Window = TimeSpan.FromSeconds(10);

        private static readonly string BlacklistFolder = Path.Combine(AppContext.BaseDirectory,"wwwroot", "Blacklist");
        private static readonly string BlacklistFile = Path.Combine(BlacklistFolder, "banned_ips.txt");

        public IpBanMiddleware(RequestDelegate next)
        {
            _next = next;

            if (!Directory.Exists(BlacklistFolder))
                Directory.CreateDirectory(BlacklistFolder);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrWhiteSpace(ip))
            {
                await _next(context);
                return;
            }

            if (IsIpBanned(ip))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Your IP has been banned.");
                return;
            }

            var counter = _ipRequestCount.GetOrAdd(ip, _ => new SlidingCounter());
            counter.Increment();

            if (counter.Count >= RequestLimit)
            {
                BanIp(ip);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Your IP has been banned for excessive requests.");
                return;
            }

            await _next(context);
        }

        private bool IsIpBanned(string ip)
        {
            if (!File.Exists(BlacklistFile)) return false;

            var bannedIps = File.ReadAllLines(BlacklistFile);
            return bannedIps.Contains(ip);
        }

        private void BanIp(string ip)
        {
            lock (_lock)
            {
                var currentIps = File.Exists(BlacklistFile)
                    ? new HashSet<string>(File.ReadAllLines(BlacklistFile))
                    : new HashSet<string>();

                if (!currentIps.Contains(ip))
                {
                    File.AppendAllText(BlacklistFile, ip + Environment.NewLine, Encoding.UTF8);
                    Console.WriteLine($"[BAN] IP {ip} has been banned and written to {BlacklistFile}");
                }
            }
        }

        private class SlidingCounter
        {
            private int _count;
            private DateTime _windowStart;

            public SlidingCounter()
            {
                _count = 0;
                _windowStart = DateTime.UtcNow;
            }

            public int Count => _count;

            public void Increment()
            {
                var now = DateTime.UtcNow;
                if (now - _windowStart > Window)
                {
                    _count = 1;
                    _windowStart = now;
                }
                else
                {
                    _count++;
                }
            }
        }
    }
}
