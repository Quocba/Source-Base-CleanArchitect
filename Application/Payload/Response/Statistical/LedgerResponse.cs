using Application.Payload.Base.Paginate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Payload.Response.Staticscal
{
    public class LedgerResponse
    {
        public decimal OpeningBalance { get; set; }
        public decimal AmountCollected { get; set; }
        public decimal AmountSpent { get; set; }
        public decimal Exist {  get; set; }
        public decimal CashReceipts { get; set; }
        public decimal CashPayments { get; set; }
        public decimal EndingBalance { get; set; }

        public ProcedurePagingResponse<LedgerDetailResponse> Details { get; set; }
    }

    public class LedgerDetailResponse
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PaymentVoucherNo { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ReceiptVoucherNo { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid BusinessPartnerId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BusinessPartnerName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Note { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? CollectAmouuts { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? SpentAmounts { get; set; } = 0;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Debit { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Type { get; set; }
    }


}
