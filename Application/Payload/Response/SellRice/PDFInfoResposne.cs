using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.SellRice
{
    public class PDFInfoResposne
    {
        public Guid Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? SellRiceNo { get; set; }
        public Guid? BusinessPartnerId { get; set;  }
        public string? BusinessPartnerName { get; set;  }
        public string? BusinessPartnerPhone { get; set;  }
        public string? NickName { get; set;  }
        public string? DeliveryAddress { get; set; }
        public string? Note { get; set; }
        public int? NumberDueDate { get; set; }
        public bool? IsPacking { get; set; }
        public Guid PackingId { get; set; }
        public string PackingName { get; set; }
        public decimal Weight { get; set; }
        public decimal? TotalAmounts { get; set; } = 0;
        public string? DiscountReason { get; set; }
        public string? SpentType { get; set; }
        public string? CitizenId { get; set; }
        public List<SellRiceDetailsResponse> Details { get; set; }

    }
}
