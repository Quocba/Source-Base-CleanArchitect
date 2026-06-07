using Application.IService;
using Application.IUnitOfWork;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class GenerateCodeService : IGenerateCodeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const string CompanyCode = "ND";

        public GenerateCodeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string GenerateDepartmentCode(string departmentName, int sequence)
        {
            if (string.IsNullOrWhiteSpace(departmentName))
                throw new ArgumentException("Department name is required");

            var normalized = RemoveVietnameseAccent(departmentName).ToUpper();

            var ignoredWords = new[] { "PHONG", "PHÒNG", "BAN", "DEPARTMENT" };

            var words = normalized
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => !ignoredWords.Contains(w))
                .ToList();

            if (!words.Any())
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