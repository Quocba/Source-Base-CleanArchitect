using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Bank
{
    public class VietQRBankResponse
    {
        public string Code { get; set; }
        public string Desc { get; set; }
        public List<VQRBankInfo> Data { get; set; }
    }
    public class VQRBankInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Bin { get; set; }
        public string ShortName { get; set; }
        public string Logo { get; set; }
        public int TransferSupported { get; set; }
        public int LookupSupported { get; set; }
    }

}
