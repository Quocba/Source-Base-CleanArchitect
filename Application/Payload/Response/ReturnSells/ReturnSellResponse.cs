using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.ReturnSells
{
    public class ReturnSellResponse
    {
        public Guid Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ReturnSellNo { get; set; }
        public Guid? BusinessPartnerId { get; set; }
        public string? BusinessPartnerName { get; set; }
        public string? Driver { get; set; }
        public string? CarNumber { get;set;  }
        public string? Note { get; set; }
        public decimal? TotalAmounts { get; set; }

    }
}
