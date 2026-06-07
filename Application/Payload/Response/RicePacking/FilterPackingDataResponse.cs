using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.RicePacking
{
    public class FilterPackingDataResponse
    {
        public decimal? Weight { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set;  }
    }
}
