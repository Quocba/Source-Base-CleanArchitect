namespace Domain.Share.Util
{
    public static class ContractNoUtil
    {
        /// <summary>
        /// Trả về mã theo định dạng yyyyMMdd-HHmmss
        /// Ví dụ: 20240618-220715
        /// Dùng DateTime.Now (theo local time). Nếu muốn dùng UTC, đổi sang DateTime.UtcNow.
        /// </summary>
        public static string GenerateSimple()
        {
            return DateTime.Now.ToString("yyyyMMdd-HHmmss");
        }

        /// <summary>
        /// Trả về mã theo định dạng yyyyMMdd-HHmmss-fff (thêm milliseconds) để giảm xung đột khi gọi nhanh.
        /// Ví dụ: 20240618-220715-123
        /// </summary>
        public static string GenerateWithMillis()
        {
            return DateTime.Now.ToString("yyyyMMdd-HHmmss-fff");
        }

        /// <summary>
        /// Trả về mã theo định dạng yyyyMMdd-HHmmss + random numeric suffix (mặc định 3 chữ số)
        /// Ví dụ: 20240618-220715-482
        /// </summary>
        public static string GenerateWithRandom(int randomDigits = 3)
        {
            var rnd = new Random();
            int max = (int)Math.Pow(10, randomDigits) - 1;
            int min = (int)Math.Pow(10, randomDigits - 1);
            int value = rnd.Next(min, max + 1);
            return $"{DateTime.Now:yyyyMMdd-HHmmss}-{value}";
        }

        /// <summary>
        /// Phiên bản thread-safe + gần như đảm bảo duy nhất: dùng ticks mod và Random kết hợp.
        /// </summary>
        public static string GenerateUnique(int extraDigits = 4)
        {
            // ticks low bits (an toàn hơn khi nhiều thread cùng gọi)
            long ticksLow = DateTime.Now.Ticks & 0xFFFFF; // lấy một phần của ticks
            var rnd = new Random(unchecked((int)DateTime.Now.Ticks));
            int r = rnd.Next((int)Math.Pow(10, extraDigits - 1), (int)Math.Pow(10, extraDigits) - 1);
            return $"{DateTime.Now:yyyyMMdd-HHmmss}-{ticksLow}-{r}";
        }

        /// <summary>
        /// Thêm tiền tố tuỳ ý, ví dụ "CTR-20240618-220715"
        /// </summary>
        public static string GenerateWithPrefix(string prefix, bool includeMillis = false)
        {
            var body = includeMillis ? DateTime.Now.ToString("yyyyMMdd-HHmmss-fff")
                                     : DateTime.Now.ToString("yyyyMMdd-HHmmss");
            if (string.IsNullOrWhiteSpace(prefix)) return body;
            return $"{prefix.TrimEnd('-')}-{body}";
        }
    }
}
