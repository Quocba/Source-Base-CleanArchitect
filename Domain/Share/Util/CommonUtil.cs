using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Domain.Share.Util
{
    public static class CommonUtil
    {
        public static string Slugify(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC).ToLower().Replace(" ", "-");
        }

        public static async Task<string> SaveImageAsync(IFormFile file, string uploadRoot, string subFolder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var folderPath = Path.Combine(uploadRoot, subFolder);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(subFolder, fileName).Replace("\\", "/");
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string GenerateSlug(string title, string? suffix = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            title = title.ToLowerInvariant();
            string normalized = RemoveDiacritics(title);
            
            title = Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
            title = Regex.Replace(title, @"\s+", "-").Trim('-');
            title = Regex.Replace(title, @"-+", "-");

            if (!string.IsNullOrWhiteSpace(suffix))
            {
                title += "-" + suffix.ToLowerInvariant();
            }

            return title;
        }
    }
}
