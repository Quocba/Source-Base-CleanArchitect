using Application.Payload.Response.OCR;
using Domain.Payload.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.OCRHelper
{
    public interface IOCRHelper
    {
        Task<ApiResponse<OcrResult>> ExtractTextAsync(Stream imageStream, string language = "vie");
    }
}
