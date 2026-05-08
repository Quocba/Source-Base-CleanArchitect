using Application.Common.Caching;
using Application.Features.BulkRiceOrders.Queries.GetsByWareHosue;
using System;

namespace Application.Common.Caching.BulkRiceOrder
{
    public static class BulkRiceOrderCacheKeys
    {
        public const string Prefix = "bulk_rice_orders:";

        public static string GetListKey(Guid warehouseId, GetByWareHouseQuery query)
        {
            return CacheHelper.GenerateKeyFromRequest($"{Prefix}list:{warehouseId}:", query);
        }

        public static string GetDetailKey(Guid orderId)
        {
            return CacheHelper.GenerateKey($"{Prefix}detail", orderId);
        }
    }
}
