using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BaseAPI.Helper
{
    public static class CommonUtil
    {
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
