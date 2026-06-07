using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Statistical
{
    public class OperatingCostsResponse
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
