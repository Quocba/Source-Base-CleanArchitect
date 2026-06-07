using System;
using System.Collections.Generic;

namespace Application.Payload.Response.BulkRiceOrder
{
    public class BulkRiceOrderPDFInfoResponse
    {
        public Guid? Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? OrderNo { get; set; }
        public string? SpentType { get; set; }
        public string? CitizenId { get; set; }

        // WareHouse
        public Guid? WareHouseId { get; set; }
        public string? WareHouseName { get; set; }

        // Partner (Nhà cung cấp/Đối tác)
        public Guid? PartnerId { get; set; }
        public string? PartnerName { get; set; }
        public string? SupplierAddress { get; set; }

        // BusinessPartner (Khách hàng)
        public Guid? BusinessPartnerId { get; set; }
        public string? BusinessPartnerName { get; set; }
        public string? BusinessPartnerPhone { get; set; }
        public string? NickName { get; set; }
        public string? VehicleNumber { get; set; }
        public string? ScalesCode { get; set; }
        
        public int? TotalPacking { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? TotalAmounts { get; set; }
        public decimal? TotalBrokerageAmounts { get; set; }
        public decimal? BulkRiceDiscount { get; set; }
        public decimal? BrokerageDiscount { get; set; }
        public string? DiscountReason { get; set; }
        public string? HandledByEmployeeName { get; set; }
        public Guid? RiceLotId { get; set; }
        public string? RiceLotName { get; set; }

        public List<BulkRiceOrderDetailsResponse>? Details { get; set; }
    }

    public class BulkRiceOrderDetailsResponse
    {
        public Guid? Id { get; set; }
        public Guid? BulkRiceId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? UnitName { get; set; }
        public decimal? Humidity { get; set; }
        public int? PackingQuantity { get; set; }
        public decimal? Weight { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmounts { get; set; }
        public decimal? BrokerageFees { get; set; }
        public decimal? BrokerageAmounts { get; set; }
        public Guid? RiceBoxId { get; set; }
        public string? RiceBoxName { get; set; }
    }
}
