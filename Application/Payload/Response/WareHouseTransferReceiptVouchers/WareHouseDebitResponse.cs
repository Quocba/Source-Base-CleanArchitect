using System;

namespace Application.Payload.Response.WareHouseTransferReceiptVouchers
{
    public class WareHouseDebitResponse
    {
        public Guid Id { get; set; }
        public string No { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public string? Type { get; set; }
        public decimal? TotalAmounts { get; set; }
        public decimal? Received { get; set;  }
        public decimal? NeedReceive => TotalAmounts - (Received ?? 0);
    }
}
