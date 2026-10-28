using Application.IService;
using Application.Payload.Request.Uploads;
using BaseAPI.Common;
using BaseAPI.Helper;
using Domain.Payload.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BaseAPI.Controllers
{
    [ApiController]
    [Route("api/v1/uploads")]
    public class UploadController : Controller
    {
        private readonly ILogger<UploadController> _logger;
        private readonly IGoogleDriveService? _googleDriveService;

        public UploadController(ILogger<UploadController> logger, IGoogleDriveService? googleDriveService = null)
        {
            _logger = logger;
            _googleDriveService = googleDriveService;
        }

        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IActionResult> Upload([FromForm] UploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new ApiResponse<string> { StatusCode = 400, Message = "File khÃ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng" });

            try
            {
                string folderPath;

                if (request.Folder.HasValue)
                {
                    folderPath = CommonUtil.GetFolderPath(request.Folder.Value);
                }
                else if (!string.IsNullOrEmpty(request.CustomFolder))
                {
                    if (request.CustomFolder.Contains("..") || Path.IsPathRooted(request.CustomFolder))
                        return BadRequest(new ApiResponse<string> { StatusCode = 400, Message = "ThÆ° má»¥c tÃ¹y chá»‰nh khÃ´ng há»£p lá»‡" });

                    folderPath = request.CustomFolder;
                }
                else
                {
                    folderPath = "General";
                }

                var filePath = await CommonUtil.SaveImageToRootAsync(request.File, folderPath);
                return Ok(new ApiResponse<object>
                {
                    StatusCode = 200,
                    Message = "Táº£i lÃªn thÃ nh cÃ´ng",
                    Data = new { FilePath = filePath }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Upload API] Lá»—i khi lÆ°u file: {Message}", ex.Message);
                return StatusCode(500, new ApiResponse<string>
                {
                    StatusCode = 500,
                    Message = $"Lá»—i server: {ex.Message}"
                });
            }
        }

        [HttpPost("upload-multiple")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IActionResult> UploadMultiple([FromForm] List<IFormFile> files, [FromForm] string? customFolder)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new ApiResponse<string> { StatusCode = 400, Message = "Danh sÃ¡ch file khÃ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng" });

            try
            {
                var folderPath = string.IsNullOrWhiteSpace(customFolder) ? "General" : customFolder.Trim();
                if (folderPath.Contains("..") || Path.IsPathRooted(folderPath))
                    return BadRequest(new ApiResponse<string> { StatusCode = 400, Message = "ThÆ° má»¥c khÃ´ng há»£p lá»‡" });

                var uploadedUrls = new List<string>();
                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var path = await CommonUtil.SaveImageToRootAsync(file, folderPath);
                        uploadedUrls.Add(path);
                    }
                }

                return Ok(new ApiResponse<List<string>>
                {
                    StatusCode = 200,
                    Message = "Táº£i lÃªn nhiá»u file thÃ nh cÃ´ng",
                    Data = uploadedUrls
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Upload API] Lá»—i táº£i lÃªn nhiá»u file: {Message}", ex.Message);
                return StatusCode(500, new ApiResponse<string> { StatusCode = 500, Message = ex.Message });
            }
        }

        [HttpPost("google-drive")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IActionResult> UploadGoogleDrive([FromForm] UploadRequest request)
        {
            if (_googleDriveService == null)
            {
                return StatusCode(501, new ApiResponse<string>
                {
                    StatusCode = 501,
                    Message = "Dá»‹ch vá»¥ Google Drive chÆ°a Ä‘Æ°á»£c cáº¥u hÃ¬nh"
                });
            }

            var response = await _googleDriveService.UploadAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
