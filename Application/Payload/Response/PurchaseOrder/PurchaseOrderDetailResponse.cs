using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Payload.Response.PurchaseOrder
{
    public class PurchaseOrderDetailResponse
    {

        public Guid? Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? RiceId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? BulkRiceId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? UnitName { get; set; }
        public decimal? Humidity { get; set; }
        public int? PackingQuantity { get; set; }
        public decimal? Weight { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmounts { get; set; }
    }
}
