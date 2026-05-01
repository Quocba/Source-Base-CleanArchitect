using Application.IService;
using Domain.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class JWTService :  IJWTService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public JWTService(JwtSettings settings, IHttpContextAccessor? httpContextAccessor)
    {
        _jwtSettings = settings;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GenerateToken(IEnumerable<Claim> customClaims)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        string ip = "Unknown";
        string fingerprint = "Unknown";

        if (httpContext != null)
        {
            ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ip))
            {
                ip = ip.Split(',')[0].Trim();
            }
            else
            {
                ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            }

            fingerprint = GenerateFingerprint(httpContext);
        }

        var claimsList = new List<Claim>(customClaims)
        {
            new Claim("ip", ip),
            new Claim("fp", fingerprint)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claimsList,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateFingerprint(HttpContext context)
    {
        var ua = context.Request.Headers["User-Agent"].ToString();
        var lang = context.Request.Headers["Accept-Language"].ToString();
        var platform = context.Request.Headers["Sec-CH-UA-Platform"].ToString();

        var raw = $"{ua}|{lang}|{platform}";
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(raw);

        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }

    private string NormalizePermissionKey(string module)
    {
        if (string.IsNullOrWhiteSpace(module))
            return string.Empty;

        var noDiacritics = RemoveDiacritics(module);
        var noSpaces = new string(noDiacritics
            .Where(c => !char.IsWhiteSpace(c))
            .ToArray());

        return noSpaces.ToUpperInvariant();
    }

    private string RemoveDiacritics(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var ch in normalized)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                sb.Append(ch);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC)
                 .Replace("Đ", "D")
                 .Replace("đ", "d");
    }

    public string? GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public string? GetUser()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public string GenerateActivationToken(string email)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}