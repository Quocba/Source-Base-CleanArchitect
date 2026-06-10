using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.OCR
{
    public class OcrResult
    {
        public bool IsSuccess { get; set; }
        public string Text { get; set; } = string.Empty;
        public float Confidence { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
