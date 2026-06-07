using System;

namespace Application.Payload.Response.WareHouseTransferReceiptVouchers
{
    public class WareHouseTransferReceiptResponse
    {
        public Guid Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? PaymentVoucherNo { get; set; }
        public Guid? ImportWareHouseId { get; set; }
        public string? ImportWareHouseName { get; set; }
        public string? ReasonPayment { get; set;  }
        public decimal? SpentAmount { get; set; }
    }
}
