using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.RicePacking
{
    public class RicePackingResponse
    {
        public Guid Id { get; set; }
        public string? Type { get; set; }
        public string? Code { get; set; }
        public string? Thumbnail { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? ThreadColor { get; set; }
        public string? WireColor { get; set; }
        public decimal? Weight { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalAmouts { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
