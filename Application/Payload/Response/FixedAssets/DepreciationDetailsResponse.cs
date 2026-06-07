using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.FixedAssets
{
    public class DepreciationDetailsResponse
    {
        public Guid Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? DepreciationCode { get; set; }
        public string? Note { get; set; }
        public decimal Amounts { get; set; }
        public Guid FixedAssetId { get; set; }
    }
}
