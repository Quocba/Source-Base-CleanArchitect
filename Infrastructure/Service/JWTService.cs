using Application.IService;
using Infrastructure.Config;
using Domain.Entities;
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
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JWTService(JwtSettings settings, IHttpContextAccessor? httpContextAccessor)
    {
        _jwtSettings = settings ?? new JwtSettings();
        _httpContextAccessor = httpContextAccessor;
        var signingKey = !string.IsNullOrEmpty(_jwtSettings.Key)
            ? _jwtSettings.Key
            : "DefaultSecretKeyForJwtAuthentication32CharsMin!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public string GenerateToken(User user)
    {
        return GenerateToken(user.Id, user.UserName ?? string.Empty, user.Role.ToString());
    }

    public string GenerateToken(Guid userId, string userName, string? roleName)
    {
        var httpContext = _httpContextAccessor?.HttpContext;

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

        var claims = new List<Claim>(5)
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, roleName ?? "User"),
            new Claim("ip", ip),
            new Claim("fp", fingerprint)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: _signingCredentials
        );

        return _tokenHandler.WriteToken(token);
    }

    public string GenerateFingerprint(HttpContext context)
    {
        var ua = context.Request.Headers["User-Agent"].ToString();
        var lang = context.Request.Headers["Accept-Language"].ToString();
        var platform = context.Request.Headers["Sec-CH-UA-Platform"].ToString();

        var raw = $"{ua}|{lang}|{platform}";
        Span<byte> hashBytes = stackalloc byte[32];
        int byteCount = Encoding.UTF8.GetByteCount(raw);
        Span<byte> utf8Bytes = byteCount <= 512 ? stackalloc byte[byteCount] : new byte[byteCount];
        Encoding.UTF8.GetBytes(raw, utf8Bytes);
        SHA256.HashData(utf8Bytes, hashBytes);

        return Convert.ToBase64String(hashBytes);
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

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            signingCredentials: _signingCredentials
        );

        return _tokenHandler.WriteToken(token);
    }
}