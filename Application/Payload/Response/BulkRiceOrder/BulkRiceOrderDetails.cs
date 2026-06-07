using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.BulkRiceOrder
{
    public class BulkRiceOrderDetails
    {
        public Guid? Id { get; set;  }
        public Guid? BulkRiceOrderId { get;set;  }
        public Guid? BulkRiceId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set;  }
        public decimal? Humidity { get; set; }
        public decimal? PackingQuantity { get; set; }
        public decimal? Weight { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmounts { get; set;  }
        public decimal? BrokerageFees { get; set; }
        public decimal? BrokerageAmounts { get; set; }
        public Guid? RiceBoxId { get; set; }
        public string? RiceBox { get; set; }
    }
}
