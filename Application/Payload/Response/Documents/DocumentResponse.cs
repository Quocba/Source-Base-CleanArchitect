using Domain.Entities.Enum;
using System;

namespace Application.Payload.Response.Documents
{
    public class DocumentResponse
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public Guid ObjectId { get; set; }
        public string Type { get; set; }
    }
}
