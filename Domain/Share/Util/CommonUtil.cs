using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Domain.Entities.Enum;
using Microsoft.AspNetCore.Http;

namespace Domain.Share.Util
{
    public static class CommonUtil
    {
        private static Random random = new Random();
        private static HashSet<string> generatedCodes = new HashSet<string>();
        private static HashSet<string> generatedReceiptCodes = new HashSet<string>();
        public static long GenerateOrderCode()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public static string TranferNo()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string code;
            do
            {
                int length = random.Next(7, 15);
                char[] buffer = new char[length];
                for (int i = 0; i < length; i++)
                {
                    buffer[i] = chars[random.Next(chars.Length)];
                }
                code = "TR" + new string(buffer);
            } while (generatedCodes.Contains(code));

            generatedCodes.Add(code);
            return code;
        }

        public static string ReceiptNo()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string code;
            do
            {
                int length = random.Next(6, 14); // phần ký tự sau "RCP", tổng ≥ 9
                char[] buffer = new char[length];
                for (int i = 0; i < length; i++)
                {
                    buffer[i] = chars[random.Next(chars.Length)];
                }
                code = "RCP" + new string(buffer);
            } while (generatedReceiptCodes.Contains(code));

            generatedReceiptCodes.Add(code);
            return code;
        }


        public static int TransferFee(decimal soTien, bool cungNganHang)
        {
            if (cungNganHang)
                return 0;
            else
            {
                decimal tyLe = 0.0003m;
                decimal phiMin = 10000m;
                decimal phiMax = 500000m;

                decimal phiDecimal = soTien * tyLe;

                if (phiDecimal < phiMin) phiDecimal = phiMin;
                if (phiDecimal > phiMax) phiDecimal = phiMax;

                return Convert.ToInt32(Math.Round(phiDecimal, 0));
            }
        }

        private static string GetBaseUploadPath()
        {
            var sharedRoot = @"C:\inetpub\wwwroot\NgocDaiBackend\Sharing";
            var projectSharedPath = Path.Combine(sharedRoot, "NgocDai");
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

        public static async Task<string> UploadCV(IFormFile file, string folderName = "CVFILE")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var uploadPath = Path.Combine(GetBaseUploadPath(), folderName);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(folderName, fileName).Replace("\\", "/");
        }

        public static async Task<string> UploadCoverletter(IFormFile file, string folderName = "Coverletter")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var uploadPath = Path.Combine(GetBaseUploadPath(), folderName);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(folderName, fileName).Replace("\\", "/");
        }


        public static string Slugify(string input)
        {
            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC).ToLower().Replace(" ", "-");
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

        private static string RemoveDiacritics(string text)
        {
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
    }
}
