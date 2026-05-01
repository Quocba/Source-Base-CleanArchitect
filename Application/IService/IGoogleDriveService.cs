using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IGoogleDriveService
    {
        /// <summary>
        /// Upload file lên Google Drive và trả về URL public
        /// </summary>
        /// <param name="file">IFormFile từ client</param>
        /// <returns>URL public file trên Google Drive</returns>
        //Task<ApiResponse<string>> UploadAsync(UploadRequest request);
    }
}