using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.WareHouseTransfer
{
    public class WareHouseTransferDetailResponse
    {
        public Guid? Id { get; set; }
        public Guid? RiceId { get; set; }
        public string? RiceCode { get; set; }
        public string? Rice { get; set; }
        public string? Driver { get; set; }
        public string? CarNumber { get; set; }
        public string? Unit { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalAmounts { get; set; }
    }
}
