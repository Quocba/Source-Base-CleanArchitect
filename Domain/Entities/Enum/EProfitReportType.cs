using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Enum
{
    public enum EProfitReportType
    {
        SalesRevenue,
        OtherRevenue,
        DifferenceInCommodityValue,
        Order,
        PackingOrder,
        BulkRiceOrder,
        OperatingCosts,
        DepreciationCost,
        SellRice,
        SellBulkRice,
        RetailReceipt,
        InternalReceive,
        InternalSpent
    }
}
