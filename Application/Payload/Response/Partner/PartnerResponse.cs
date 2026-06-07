using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Partner
{
    public class PartnerResponse
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? NickName { get; set; }
        public string? Phone { get; set; }
        public decimal? Existing { get; set;  }
        public string? Address { get; set; }
        public string? Status { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
