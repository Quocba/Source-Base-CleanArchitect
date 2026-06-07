using System;
using System.Collections.Generic;

namespace Application.Payload.Response.RicePackingOrders
{
    public class RicePackingOrderPDFInfoResponse
    {
        public Guid Id { get; set; }
        public Guid WareHouseId { get; set; }
        public string OrderNo { get; set; } = default!;
        public string? Note { get; set; }
        public decimal TotalAmounts { get; set; }
        public DateTime? CreatedDate { get; set; }

        // Business Partner info
        public Guid BusinessPartnerId { get; set; }
        public string? BusinessPartnerName { get; set; }
        public string? NickName { get; set; }
        public string? BusinessPartnerPhone { get; set; }
        public string? BusinessPartnerAdrress { get; set; }
        public string? CitizenId { get; set; }

        public List<RicePackingOrderDetailsResponse>? Details { get; set; }
    }
}
