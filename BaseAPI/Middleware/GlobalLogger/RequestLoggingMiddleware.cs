using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BaseAPI.Middleware.GlobalLogger
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var request = context.Request;

            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();
                var statusCode = context.Response?.StatusCode;
                var path = request.Path.Value;
                var method = request.Method;
                var query = request.QueryString.HasValue ? request.QueryString.Value : "";

                var userName = context.User?.Identity?.IsAuthenticated == true
                    ? (context.User.Identity.Name ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                    : "Anonymous";

                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                _logger.LogInformation(
                    "[{Method}] {Path}{Query} | User: {UserName} | IP: {IPAddress} | Status: {StatusCode} | Time: {ElapsedMilliseconds}ms",
                    method, path, query, userName, ipAddress, statusCode, sw.ElapsedMilliseconds);
            }
        }
    }
}
