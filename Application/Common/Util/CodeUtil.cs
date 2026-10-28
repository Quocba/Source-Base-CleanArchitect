namespace Application.Common.Util
{
    public static class CodeUtil
    {
        private const string Prefix = "DCS";

        public static string GenerateUserCode(int sequence)
        {
            return $"{Prefix}{sequence}";
        }

        public static string GenerateUserCode(int sequence, int digits)
        {
            return digits > 0 ? $"{Prefix}{sequence.ToString().PadLeft(digits, '0')}" : $"{Prefix}{sequence}";
        }

        public static string GenerateUserCode(string sequence)
        {
            return $"{Prefix}{sequence?.Trim()}";
        }
    }
}
