using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace API.Middleware.GlobalLogger
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;
        private readonly bool _showStackTrace;

        public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger, bool showStackTrace = false)
        {
            _next = next;
            _logger = logger;
            _showStackTrace = showStackTrace;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                await _next(context);
            }
            catch (Exception ex)
            {
                var request = context.Request;
                var path = request.Path;
                var method = request.Method;
                var query = request.QueryString.HasValue ? request.QueryString.Value : "";

                string body = "";
                if (request.ContentLength > 0 && request.Body.CanSeek)
                {
                    request.Body.Seek(0, SeekOrigin.Begin);
                    using var reader = new StreamReader(request.Body, Encoding.UTF8, false, leaveOpen: true);
                    body = await reader.ReadToEndAsync();
                    request.Body.Seek(0, SeekOrigin.Begin);
                }

                var headers = string.Join(", ", request.Headers.Select(h => $"{h.Key}:{h.Value}"));

                var logMessage = $@"
                                ❌ Unhandled Exception
                                Method: {method}
                                Path: {path}{query}
                                Headers: {headers}
                                Body: {body}
                                Message: {ex.Message}
                                StackTrace: {ex.StackTrace}";

                _logger.LogError(logMessage);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Detail = _showStackTrace ? ex.StackTrace : null
                });
            }
        }
    }
}
