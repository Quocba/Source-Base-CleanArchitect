using Microsoft.AspNetCore.Http;

namespace Application.Payload.Request.Uploads
{
    public class UploadRequest
    {
        public IFormFile? File { get; set; }
        public string? CustomFolder { get; set; }
    }
}
