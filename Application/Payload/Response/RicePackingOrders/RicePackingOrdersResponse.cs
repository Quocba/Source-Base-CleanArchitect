using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.RicePackingOrders
{
    public class RicePackingOrdersResponse
    {
        public Guid Id { get; set; }
        public string? OrderNo { get; set; }
        public Guid? BusinessPartnerId { get; set; }
        public string? BusinessPartnerName { get; set; }
        public string? BusinessPartnerPhone { get; set; }
        public decimal? TotalAmounts { get; set; }
    }
}
