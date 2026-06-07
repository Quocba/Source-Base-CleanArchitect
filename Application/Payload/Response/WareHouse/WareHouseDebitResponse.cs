using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.WareHouse
{
    public class WareHouseDebitResponse
    {
        public Guid WareHouseId { get; set; }
        public string Name { get; set; }
        public decimal? OpeningBalance { get; set; } = 0;
        public decimal? PeriodAmount { get; set; } = 0;
        public decimal? PaidAmount { get; set; } = 0;
        public decimal? ClosingBalance { get; set; } = 0;
    }
}
