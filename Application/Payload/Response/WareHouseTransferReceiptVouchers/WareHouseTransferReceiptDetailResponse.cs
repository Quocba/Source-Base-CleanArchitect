using System;

namespace Application.Payload.Response.WareHouseTransferReceiptVouchers
{
    public class WareHouseTransferReceiptDetailResponse
    {
        public Guid? Id { get; set; }
        public string? No { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? Paid { get; set; }
        public decimal? ReamainingAmount { get; set; }
    }
}
