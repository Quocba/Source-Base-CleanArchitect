using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.ReturnSells
{
    public class ReturnSellRiceDetailsResposne
    {
        public Guid? Id { get; set; }
        public Guid? RiceOrBulkRiceId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public Guid? PackingId { get; set; }
        public string? PackingName { get; set; }
        public int? PackingQuantity { get; set;  }
        public decimal? Weight { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalAmounts { get; set; }
    }
}
