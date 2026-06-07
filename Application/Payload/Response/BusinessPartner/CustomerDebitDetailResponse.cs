using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.BusinessPartner
{
    public class CustomerDebitDetailResponse
    {
        public Guid Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? No { get; set; }
        public decimal? TotalAmounts { get; set; }
        public decimal? Received { get; set; }
        public decimal? Remaining { get; set; }
        public string? Type { get; set; }
    }
}
