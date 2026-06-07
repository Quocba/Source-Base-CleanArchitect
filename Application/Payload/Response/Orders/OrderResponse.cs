using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Orders
{
    public class OrderResponse
    {
        public Guid? Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? OrderNo { get; set; }
        public Guid? BusinessPartnerId { get; set; }
        public string? BusinessPartnerName { get; set; }
        public Guid? PartnerId { get; set; }
        public string? PartnerName { get; set; }
        public string? ScalesCode { get; set; }
        public int? TotalPacking { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? TotalAmounts { get; set; }
        public decimal? BrokerageDiscount { get; set; }
        public decimal? RiceDiscount { get; set; }
        public decimal? TotalBrokerageAmounts { get; set; }

    }
}
