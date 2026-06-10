using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Request.OCR
{
    public class OCRRequest
    {
        public IFormFile images { get; set; }
    }
}
