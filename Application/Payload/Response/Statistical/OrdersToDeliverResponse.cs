using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Statistical
{
    public class OrdersToDeliverResponse
    {
        public Guid? BusinessPartnerId { get; set; }
        public string? BusinessPartnerName { get; set; }
        public string? No { get; set; }
        public decimal? Weight { get; set; }
        public decimal? TotalAmounts { get; set; }
        public string? Status { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}
