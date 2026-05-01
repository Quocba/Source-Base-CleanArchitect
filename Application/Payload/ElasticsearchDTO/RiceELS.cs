using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.ElasticsearchDTO
{
    public class RiceELS
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Stock { get; set; }
        public Guid? WareHouseId { get; set; }
        public bool? IsFlow { get; set; }
        public bool? IsDeleted { get; set; }
        public Guid? UnitId { get; set; }
        public string? UnitName { get; set; }
        public Guid? RiceTypeId { get; set; }
        public string? RiceTypeName { get; set; }
        public string? WareHouseName { get; set; }
        public string? WareHouseAddress { get; set; }
        public decimal? TotalAmounts { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? LastModifiedBy { get; set; }
    }
}
