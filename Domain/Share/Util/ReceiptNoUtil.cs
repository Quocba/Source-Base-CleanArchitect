namespace Domain.Share.Util
{
    public static class ReceiptNoUtil
    {
        /// <summary>
        /// Tạo số phiếu cơ bản theo format: {type}-{yyyy}-{00001}
        /// Lưu ý: số thứ tự cần lấy từ DB để đảm bảo liên tục và không trùng.
        /// </summary>
        public static string Generate(string type, int year, int sequence)
        {
            return $"{type}-{year}-{sequence:D5}";
        }

        /// <summary>
        /// Tạo phiếu thu mẫu với số sequence tự nhập.
        /// Ví dụ: PT-2025-00001
        /// </summary>
        public static string GenerateReceipt(int year, int sequence)
        {
            return Generate("PT", year, sequence);
        }

        /// <summary>
        /// Tạo phiếu chi mẫu với số sequence tự nhập.
        /// Ví dụ: PC-2025-00001
        /// </summary>
        public static string GeneratePayment(int year, int sequence)
        {
            return Generate("PC", year, sequence);
        }

        /// <summary>
        /// Tạo số phiếu tạm thời theo thời gian (tránh trùng nhưng không liên tục).
        /// Ví dụ: PT-20250919-101530
        /// Dùng khi chưa commit DB nhưng vẫn cần "mã chờ".
        /// </summary>
        public static string GenerateTemporary(string type)
        {
            return $"{type}-{DateTime.Now:yyyyMMdd-HHmmss}";
        }

        /// <summary>
        /// Thêm hậu tố random để chắc chắn không trùng (dùng trong môi trường multi-thread).
        /// Ví dụ: PT-20250919-101530-482
        /// </summary>
        public static string GenerateWithRandom(string type, int randomDigits = 3)
        {
            var rnd = new Random();
            int max = (int)Math.Pow(10, randomDigits) - 1;
            int min = (int)Math.Pow(10, randomDigits - 1);
            int value = rnd.Next(min, max + 1);
            return $"{type}-{DateTime.Now:yyyyMMdd-HHmmss}-{value}";
        }
    }
}
