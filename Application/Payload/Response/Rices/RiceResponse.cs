using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Rices
{
    public class RiceResponse
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal Stock { get; set; }
        public decimal TotalAmounts { get; set; }
        public string? Thubnail { get; set; }
        public bool? IsFlow { get; set; }
        public bool? IsDeleted { get; set; }
        public Guid? UnitId { get; set; }
        public string? Unit {  get; set; }
        public Guid? RiceTypeId { get; set; }
        public string? RiceType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }

    }
}
