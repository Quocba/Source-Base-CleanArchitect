using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace Application.Common.Caching
{
    public static class CacheHelper
    {
        public static string GenerateKey(string prefix, params object[] args)
        {
            if (args == null || args.Length == 0) return prefix;
            return $"{prefix}:{string.Join(":", args.Select(a => a?.ToString() ?? "null"))}";
        }

        public static string GenerateKeyFromRequest(string prefix, object request)
        {
            if (request == null) return prefix;
            
            var json = JsonSerializer.Serialize(request);
            
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower().Substring(0, 16); // Take first 16 chars for brevity
            
            return $"{prefix}req:{hash}";
        }
    }
}
