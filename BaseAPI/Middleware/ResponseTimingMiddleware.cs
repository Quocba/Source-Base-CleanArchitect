using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace BaseAPI.Middleware
{
    public class ResponseTimingMiddleware(RequestDelegate _next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            context.Response.OnStarting(() =>
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;
                var elapsedTotalMs = stopwatch.Elapsed.TotalMilliseconds;

                context.Response.Headers["X-Response-Time"] = $"{elapsedMs}ms";
                context.Response.Headers["Server-Timing"] = $"total;dur={elapsedTotalMs.ToString("F2", CultureInfo.InvariantCulture)};desc=\"API Execution Time\"";

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
