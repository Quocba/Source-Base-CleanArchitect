using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.PurchaseOrder
{
    public class PurchaseOrderResponse
    {
        public Guid? Id { get; set; }
        public string? OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? BusinessPartnerName { get; set; }
        public string? BusinessPartnerPhone { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? DeliveryAddress { get; set; }
        public decimal? TotalAmounts { get; set; }
        public string? Note { get; set; }
        public bool? IsBulkRice { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
