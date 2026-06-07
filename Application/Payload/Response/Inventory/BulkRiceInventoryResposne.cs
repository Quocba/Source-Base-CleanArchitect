using System;
using System.Collections.Generic;

namespace Application.Payload.Response.Inventory
{
    public class BulkRiceInventoryResposne
    {
        public Guid RiceLotId { get; set; }
        public string? RiceLotCode { get; set; }
        public string? RiceLotName { get; set; }
        public string? BulkRiceOrderNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal Weight { get; set; }
        public decimal RiceLotTotalAmounts { get; set; }
        public decimal Inventory { get; set; }
        public string? Status { get; set; }
        public List<BulkRiceOrderDetailItem> BulkRiceOrderDetails { get; set; } = new();
        public List<SellBulkRiceDetailItem> SellBulkRiceDetails { get; set; } = new();
    }

    public class BulkRiceOrderDetailItem
    {
        public Guid Id { get; set; }
        public Guid BulkRiceId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Sold { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Inventory { get; set; }
    }

    public class SellBulkRiceDetailItem
    {
        public Guid? Id { get; set; }
        public string? SellBulkRiceNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? BulkRiceName { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalAmounts { get; set; }
    }
}
