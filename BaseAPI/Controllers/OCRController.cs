using Application.OCRHelper;
using Application.Payload.Request.OCR;
using Application.Payload.Response.OCR;
using Domain.Payload.Base; // Nơi chứa ApiResponse
using Domain.Share.Common; // Nơi chứa StatusCode enum
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/v1/ocr")]
    [ApiController]
    public class OCRController(IOCRHelper _ocrHelper) : Controller
    {
        [HttpPost("extract")]
        public async Task<IActionResult> ExtractText([FromForm] OCRRequest file)
        {
            if (file == null || file.images == null)
            {
                return BadRequest(new ApiResponse<OcrResult>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Vui lòng đính kèm file ảnh với key là 'images'.",
                    Data = null
                });
            }

            var response = await _ocrHelper.ExtractTextAsync(file.images.OpenReadStream());

            return StatusCode((int)response.StatusCode, response);
        }
    }
}