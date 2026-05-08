using Application.Payload.Request.Uploads;
using Domain.Share.Util;
using Microsoft.AspNetCore.Mvc;

namespace BaseAPI.Controllers
{
    [ApiController]
    [Route("api/v1/uploads")]
    public class UploadController(ILogger<UploadController> _logger, IWebHostEnvironment _env) : Controller
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                 return BadRequest("No file uploaded.");
            try
            {
                string folderName = "uploads";
                
                if (!string.IsNullOrEmpty(request.CustomFolder))
                {
                    if (request.CustomFolder.Contains("..") || Path.IsPathRooted(request.CustomFolder))
                        return BadRequest("Invalid custom path.");
                    
                    folderName = request.CustomFolder;
                }

                var uploadRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                var filePath = await CommonUtil.SaveImageAsync(request.File, uploadRoot, folderName);
                
                return Ok(new { FilePath = filePath });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Upload API] {ex.Message}", ex);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
