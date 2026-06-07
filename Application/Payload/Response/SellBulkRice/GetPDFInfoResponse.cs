using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.SellBulkRice
{
    public class GetPDFInfoResponse
    {
        public Guid Id { get; set; }
        public Guid? BusinessPartnerId { get; set; }
        public string? BusinessPartnerName { get; set; }
        public string? NickName { get; set; }
        public string? SellBulkRiceNo { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? NumberDueDate { get; set; }
        public bool? IsBag { get; set; }
        public string? DiscountReason { get; set; }
        public string? SpentType { get; set; }
        public decimal? TotalAmounts { get; set; } = 0;
        public List<SellBulkRiceDetailsResponse>? Details { get; set; }
    }
}
