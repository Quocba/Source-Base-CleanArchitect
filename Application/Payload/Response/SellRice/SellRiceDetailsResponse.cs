using MassTransit.Futures.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.SellRice
{
    public class SellRiceDetailsResponse
    {
        public Guid? Id { get; set; }
        public Guid? RiceId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public Guid? PackingId { get; set; }
        public string? PackingName { get; set; }
        public decimal? PackingWeight { get; set; } = 0;
        public decimal? Weight { get;set;  }
        public bool? IsBag { get; set; }
        public int? PackingNumber { get; set;  }    
        public decimal? TotalWeight { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmounts { get; set; }
        public Guid? RiceBooxId { get; set; }
        public string? RiceBox { get; set; }
    }
}
