using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.BulkRice
{
    public class BulkRiceResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Thumbnail { get; set; }  
        public string Description { get; set; } = null!;
        public decimal Stock { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public decimal Price { get; set; } = 0;
        public bool IsFollow { get; set; }
        public bool IsDeleted { get; set; }
        public Guid WareHouseId { get; set; }
        public Guid RiceTypeId { get; set; }
        public string? RiceTypeName { get; set; }
    }
}
