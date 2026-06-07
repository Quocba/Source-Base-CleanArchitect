using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.TripSheets
{
    public class TripSheetResponse
    {
        public Guid? Id { get; set; }
        public string? TripSheetNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? SellRiceOrSellBulkRiceNo { get; set; }
        public Guid? BusinessPartnerId { get; set; }
        public string? BusinessPartnerName { get; set; }
        public string? Driver { get;set; }
        public string? CarNumber { get; set; }
        public string? DeliveryAddress { get; set; }
        public decimal? TotalWeight { get; set; }

    }
}
