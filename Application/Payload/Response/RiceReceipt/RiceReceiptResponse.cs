using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.RiceReceipt
{
    public class RiceReceiptResponse
    {
        public Guid Id { get; set; }
        public string ReceiptNo { get; set; }
        public string CustomerName { get; set; }
        public string Note { get; set; }
        public decimal Weight { get; set; }
        public decimal TotalAmounts { get; set; }
    }

}
