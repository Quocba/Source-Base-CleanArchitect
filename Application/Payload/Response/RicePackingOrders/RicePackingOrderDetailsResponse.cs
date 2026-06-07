using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.RicePackingOrders
{
    public class RicePackingOrderDetailsResponse
    {
        public Guid Id { get; set; }
        public Guid PackingId { get; set; }
        public string? Code { get; set; }
        public string? PackingName { get; set; }
        public string? Color { get; set; }
        public string? ThreadColor { get; set; }
        public string? WireColor { get; set; }
        public decimal? Weight {  get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalAmounts { get; set;  }
    }
}
