using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Statistical
{
    public class BuySellResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal QuantityBuy { get; set; }
        public decimal QuantitySell { get; set; }
        public decimal TotalBuyAmount { get; set; }
        public decimal TotalSellAmount { get; set; }
        public decimal Stock { get; set; }

    }
}
