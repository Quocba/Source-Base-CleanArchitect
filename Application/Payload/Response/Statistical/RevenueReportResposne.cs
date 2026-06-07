using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Statistical
{
    public class RevenueReportResposne
    {
        public decimal SalesRevenue { get; set; }
        public decimal OtherRevenue { get; set; }
        public decimal BeginningInventoryValue { get; set; }
        public decimal EndingInventoryValue { get; set; }

        // --- CHI PHÍ ---
        public decimal PurchaseCost { get; set; }
        public decimal BulkPurchaseCost { get; set; }
        public decimal PackingPurchaseCost { get; set; }
        public decimal OperatingCost { get; set; }
        public decimal DepreciationCost { get; set; }

        public decimal TotalRevenue => SalesRevenue + OtherRevenue + (EndingInventoryValue - BeginningInventoryValue);
        public decimal TotalCost => PurchaseCost + BulkPurchaseCost + PackingPurchaseCost + OperatingCost + DepreciationCost;
        public decimal Profit => TotalRevenue - TotalCost;
    }
}
