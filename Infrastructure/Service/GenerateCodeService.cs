using Application.IService;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Service
{
    public class GenerateCodeService : IGenerateCodeService
    {
        private const string CompanyCode = "ND";

        public string GenerateDepartmentCode(string departmentName, int sequence)
        {
            if (string.IsNullOrWhiteSpace(departmentName))
                throw new ArgumentException("Department name is required");

            var normalized = RemoveVietnameseAccent(departmentName)
                                .ToUpper();

            var ignoredWords = new[] { "PHONG", "PHÒNG", "BAN", "DEPARTMENT" };

            var words = normalized
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => !ignoredWords.Contains(w))
                .ToList();

            if (words.Count == 0)
                throw new Exception("Tên phòng ban không hợp lệ");

            var deptCode = string.Concat(words.Select(w => w[0]));

            return $"{CompanyCode}-{deptCode}-{sequence:D3}";
        }

        public string GenerateEmployeeCode()
        {
            var year = DateTime.Now.Year;
            var random = new Random().Next(1, 9999);

            return $"ND-NV-{year}-{random:D4}";
        }

        public string GenerateWarehouseCode(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address không được rỗng");

            string normalized = RemoveVietnameseAccent(address).ToUpper();

            string cityCode = "UNK";
            if (normalized.Contains("HO CHI MINH") || normalized.Contains("TP HCM"))
                cityCode = "HCM";
            else if (normalized.Contains("HA NOI"))
                cityCode = "HN";
            else if (normalized.Contains("DA NANG"))
                cityCode = "DN";

            string districtCode = "QX";
            var match = Regex.Match(normalized, @"QUAN\s*(\d+)");
            if (match.Success)
                districtCode = $"Q{match.Groups[1].Value}";

            string hash = GenerateShortHash(normalized);

            return $"{CompanyCode}-WH-{cityCode}-{districtCode}-{hash}";
        }

        private static string GenerateShortHash(string input)
        {
            var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hashBytes)[..4];
        }

        private static string RemoveVietnameseAccent(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}