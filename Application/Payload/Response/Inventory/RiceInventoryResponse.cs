using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Inventory
{
    public class RiceInventoryResponse
    {
        public Guid? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public decimal? Weight { get; set;  }
        public decimal? Price { get; set;  }
        public decimal? TotalAmounts { get; set; }
        public Guid? RiceTypeId { get; set; }
        public string? RiceType { get; set; }
    }
}
