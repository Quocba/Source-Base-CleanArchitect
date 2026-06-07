using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.FixedAssets
{
    public class FixedAssetsResponse
    {
        public Guid? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public DateTime? UsageTime { get; set; }
        public decimal? Price { get; set; }
        public decimal DepreciationValue { get; set; }
        public decimal UsageYear { get; set; }
        public decimal DepreciationRate { get; set; }
        public decimal AccumulatedDepreciation { get; set; }
        public decimal RemainingValue { get; set; }
        public decimal MonthlyDepreciation { get; set; }
        public DateTime? StartTimeUse { get; set; }
        public int TimeUse { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
