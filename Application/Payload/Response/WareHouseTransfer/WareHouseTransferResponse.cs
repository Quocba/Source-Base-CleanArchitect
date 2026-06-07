using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.WareHouseTransfer
{
    public class WareHouseTransferResponse
    {
        public Guid Id { get; set; }
        public string? TransferNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Note { get; set; }
        public Guid?  FromWareHouseId { get; set; }
        public string? FromWareHouseName { get; set; }
        public Guid? ToWareHouseId { get; set; }
        public string? ToWareHouseName { get; set; }
        public decimal? TotalAmounts { get; set; }
        public Guid? TransferredById { get; set; }
        public string? TransferredBy { get; set; }
    }
}
