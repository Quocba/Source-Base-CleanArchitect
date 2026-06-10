using Application.Payload.Response.OCR; // Giả định OcrResult đang nằm ở đây
using Domain.Payload.Base;
using Domain.Share.Common;
using System;
using System.IO;
using System.Threading.Tasks;
using Tesseract;

namespace Application.OCRHelper
{
    public class TesseractOcrHelper : IOCRHelper
    {
        public async Task<ApiResponse<OcrResult>> ExtractTextAsync(Stream imageStream, string language = "vie")
        {
            // Trường hợp 1: Dữ liệu đầu vào không hợp lệ
            if (imageStream == null || imageStream.Length == 0)
            {
                return new ApiResponse<OcrResult>
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = "Không có thông tin file ảnh đính kèm.",
                    Data = null
                };
            }

            try
            {
                string tessDataPath = @"E:\Code\Back-End\Source-Base-CleanArchitect\Application\OCRHelper\Tesseract";

                if (!Directory.Exists(tessDataPath))
                {
                    return new ApiResponse<OcrResult>
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Message = $"Lỗi hệ thống: Không tìm thấy thư mục tessdata tại: {tessDataPath}",
                        Data = null
                    };
                }

                using var ms = new MemoryStream();
                await imageStream.CopyToAsync(ms);
                byte[] imageBytes = ms.ToArray();

                using var engine = new TesseractEngine(tessDataPath, language, EngineMode.Default);
                using var img = Pix.LoadFromMemory(imageBytes);
                using var page = engine.Process(img);

                var dataResult = new OcrResult
                {
                    IsSuccess = true,
                    Text = page.GetText()?.Trim() ?? string.Empty,
                    Confidence = page.GetMeanConfidence()
                };

                return new ApiResponse<OcrResult>
                {
                    StatusCode = StatusCode.OK, 
                    Message = "Trích xuất văn bản thành công.",
                    Data = dataResult
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<OcrResult>
                {
                    StatusCode = StatusCode.InternalServerError,
                    Message = $"Lỗi trong quá trình xử lý OCR: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}