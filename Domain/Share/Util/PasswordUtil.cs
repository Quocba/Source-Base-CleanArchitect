using System.Security.Cryptography;
using System.Text;

namespace Domain.Share.Util
{
    public static class PasswordUtil
    {
        public static string HashPassword(string rawPassword)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawPassword));
            return Convert.ToBase64String(bytes);
        }

        public static string ToMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
