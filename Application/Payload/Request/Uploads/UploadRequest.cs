using Domain.Entities.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Request.Uploads
{
    public class UploadRequest
    {
        public IFormFile? File { get; set; }
        public FolderPath? Folder { get; set; }
        public string? CustomFolder { get; set; }
    }
}
