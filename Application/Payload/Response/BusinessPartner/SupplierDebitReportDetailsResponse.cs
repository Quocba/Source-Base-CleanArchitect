using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.BusinessPartner
{
    public class SupplierDebitReportDetailsResponse
    {
        public Guid? Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? PurchaseNo { get; set; }
        public decimal? TotalAmounts { get; set; }
        public decimal? Paid { get; set; }
        public decimal? Debt { get; set; }
        public string? Type { get; set; }
    }
}
