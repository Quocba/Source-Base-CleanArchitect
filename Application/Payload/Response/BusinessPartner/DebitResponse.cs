using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.BusinessPartner
{
    public class DebitResponse
    {
        public Guid BusinessPartnerId { get; set; }
        public string? BusinessPartnerName { get; set; }
        public string? Code { get; set; }
        public decimal? OpeningBalance { get; set; } = 0;
        public decimal? PeriodAmount { get; set; } = 0;
        public decimal? PaidAmount { get; set; } = 0;
        public decimal? ClosingBalance { get; set; } = 0;
    }
}
