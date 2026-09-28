using System;
using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Util
{
    public static class PasswordUtil
    {
        public static string HashPassword(string rawPassword)
        {
            if (string.IsNullOrEmpty(rawPassword)) return string.Empty;

            Span<byte> hashBytes = stackalloc byte[32];
            int byteCount = Encoding.UTF8.GetByteCount(rawPassword);
            Span<byte> utf8Bytes = byteCount <= 256 ? stackalloc byte[byteCount] : new byte[byteCount];
            Encoding.UTF8.GetBytes(rawPassword, utf8Bytes);
            SHA256.HashData(utf8Bytes, hashBytes);
            return Convert.ToBase64String(hashBytes);
        }

        public static string ToMD5(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
