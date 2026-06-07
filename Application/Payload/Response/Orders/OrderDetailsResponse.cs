using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Orders
{
    public class OrderDetailsResponse
    {
        public Guid? Id { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? RiceId { get; set; }
        public string? RiceCode { get; set; }
        public string? RiceName { get; set; }
        public decimal? Humidity { get; set; }
        public int? PackingQuantity { get; set; }
        public decimal? PackingWeight { get; set; } = 0;
        public decimal? Weight { get; set; }
        public string? Unit { get; set; } = "Kg";
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmounts { get; set; }
        public decimal? BrokerageFees { get; set; }
        public decimal? BrokerageAmounts { get; set; }
        public Guid? RiceBoxId { get; set; }
        public string? RiceBoxName { get; set; }

    }
}
