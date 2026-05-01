using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQContract.Payload.Request
{
    public class EmailSendFileMessage
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public byte[]? FileBytes { get; set; }
    }
}
