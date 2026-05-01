using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.DTO
{
    public class EmailRequest<T>
    {
        public required string To { get; set; }
        public required string Subject { get; set; }
        public required T Body { get; set; }
    }
}
