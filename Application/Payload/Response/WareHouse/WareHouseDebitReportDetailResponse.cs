using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.WareHouse
{
    public class WareHouseDebitReportDetailResponse
    {
        public Guid WareHouseId { get;set; }
        public DateTime CreatedDate { get; set; }
        public string WareHouseTransferNo { get; set; }
        public decimal TotalAmounts { get; set; }
        public decimal Paid { get; set; }
        public decimal NeedCollect { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
