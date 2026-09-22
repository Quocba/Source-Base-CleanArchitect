using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BaseAPI.Helper
{
    public static class CommonUtil
    {
        private static string GetBaseUploadPath()
        {
            var sharedRoot = @"C:\inetpub\wwwroot\SourceBase\Sharing";
            var projectSharedPath = Path.Combine(sharedRoot, "SourceBase");
            if (Directory.Exists(sharedRoot))
            {
                if (!Directory.Exists(projectSharedPath))
                    Directory.CreateDirectory(projectSharedPath);
                return projectSharedPath;
            }
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        public static async Task<string> SaveImageToRootAsync(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var basePath = Path.Combine(GetBaseUploadPath(), "images");
            var fullFolderPath = Path.Combine(basePath, folderPath);

            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(fullFolderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("images", folderPath, fileName).Replace("\\", "/");
            return relativePath;
        }

        public static async Task<string> SaveImagesToRootAsXmlAsync(List<IFormFile> files, string folderPath)
        {
            if (files == null || files.Count == 0)
                throw new ArgumentException("No files selected.");

            var basePath = Path.Combine(GetBaseUploadPath(), "images");
            var fullFolderPath = Path.Combine(basePath, folderPath);

            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);

            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var fullPath = Path.Combine(fullFolderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = Path.Combine("images", folderPath, fileName).Replace("\\", "/");
                imageUrls.Add(relativePath);
            }

            var xml = new XDocument(
                new XElement("Images",
                    imageUrls.Select(url => new XElement("Image", url))
                )
            );

            return xml.ToString(SaveOptions.None);
        }

        public static string Slugify(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC).ToLower().Replace(" ", "-");
        }

        public static string GenerateSlug(string title, string? suffix = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            title = title.ToLowerInvariant();

            string normalized = title.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            title = sb.ToString().Normalize(NormalizationForm.FormC);

            title = Regex.Replace(title, @"[^a-z0-9\s-]", "");
            title = Regex.Replace(title, @"\s+", "-").Trim('-');
            title = Regex.Replace(title, @"-+", "-");

            if (!string.IsNullOrWhiteSpace(suffix))
            {
                title += "-" + suffix.ToLowerInvariant();
            }

            return title;
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

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

        public static string GetFolderPath(FolderPath folder)
        {
            return folder switch
            {
                FolderPath.Hopdong => "Hopdong",
                FolderPath.HoaDon => "HoaDon",
                FolderPath.XuatKho => "XuatKho",
                FolderPath.SanPham => "SanPham",
                FolderPath.Receipt => "Thu_Chi",
                FolderPath.EmployeeImage => "AnhNhanVien",
                _ => throw new ArgumentOutOfRangeException(nameof(folder), folder, null)
            };
        }
    }
}
