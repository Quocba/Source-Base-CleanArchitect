using System;

namespace Application.Payload.Response.WareHouseTransferReceiptVouchers
{
    public class WareHouseTransferReceiptPDFInfoResponse
    {
        public Guid Id { get; set; }
        public string? PaymentVoucherNo { get; set; }
        public Guid ImportWareHouseId { get; set; }
        public string? ImportWareHouseName { get; set; }
        public Guid? BankAccountId { get; set; }
        public string? BankNo { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountName { get; set; }
        public DateTime? Date { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? ReasonPayment { get; set; }
        public decimal? SpentAmount { get; set; }
    }
}
