using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.SellRice
{
    public class SellRiceResponse
    {
        public Guid? Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? SellRiceNo { get; set; }
        public Guid? BusinessPartnerId { get; set; }
        public string? BusinessPartnerName { get; set; }
        public string? DeliveryAddress { get; set; }
        public decimal? TotalWeight { get; set; }
        public DateTime? DueDate { get;set;  }
        public decimal? TotalAmounts { get; set; }


    }
}
