using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.BulkRice
{
    public class BulkRiceByRiceLotOrderResponse
    {
        public Guid? BulkRiceId { get; set; }
        public Guid? RiceLotId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public decimal? Weight { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
