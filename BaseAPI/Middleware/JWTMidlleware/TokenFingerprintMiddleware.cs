using Application.IService;

namespace API.Middleware.JWTMidlleware
{
    public class TokenFingerprintMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJWTService _jwtService;
        public TokenFingerprintMiddleware(RequestDelegate next, IJWTService jwtService)
        {
            _next = next;
            _jwtService = jwtService;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;

            }

            var tokenFp = context.User.FindFirst(c => c.Type == "fp")?.Value;
            var currentFp = _jwtService.GenerateFingerprint(context);
            if (!string.IsNullOrEmpty(tokenFp) &&
                           !string.Equals(tokenFp, currentFp, StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Device fingerprint mismatch");
                return;
            }

            await _next(context);
        }
    }
}
