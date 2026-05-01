using Domain.KeyHandle;
using Domain.Payload.Base;
using Domain.Share.Common;
using Newtonsoft.Json;

namespace Ecomer_DCS.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IApiKeyValidator validator)
        {
            // Bỏ qua Swagger
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var apiKey))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResponse<string>
                {
                    StatusCode = StatusCode.Unauthorized,
                    Message = "API Key không được gửi.",
                    Data = null
                }));
                return;
            }

            if (!validator.IsValid(apiKey))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResponse<string>
                {
                    StatusCode = StatusCode.Unauthorized,
                    Message = "API Key không hợp lệ.",
                    Data = null
                }));
                return;
            }

            await _next(context);
        }

    }
}
