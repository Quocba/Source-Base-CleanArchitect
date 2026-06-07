using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Payload.Response.BusinessPartner
{
    public class BusinessPartnerResponse
    {
        public Guid? Id { get; set;  }
        public string? Code { get; set;  }
        public string? Name { get; set; }
        public string? NickName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public decimal? Existing { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Email { get; set; }
        public string? VehicleNumber { get; set; }
        public string? Status { get; set; }
        public string? PartnerType { get; set; }
        public bool? IsDeleted { get; set;  }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? CreatedBy { get; set;  }
        public string? LastModifiedBy { get; set; }

    }
}
