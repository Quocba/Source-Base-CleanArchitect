using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.SellBulkRice
{
    public class SellBulkRiceDetailsResponse
    {
        public Guid Id { get; set; }
        public Guid BulkRiceId { get; set; }
        public string? BulkRiceName { get; set; }
        public string? Code { get; set; }
        public Guid RiceLotId { get; set; }
        public string? RiceLot { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Price { get; set; }
        public int NumberPacking { get; set; }
        public decimal? TotalAmounts { get; set; }
        public Guid RiceBoxId { get; set; }
        public string? RiceBox { get; set; }
    }
}
