using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.RiceTypes
{
    public class RiceTypesResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public Guid? PackingId { get; set; }
        public string? PackingName { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public string? LastModifiedBy { get; set; }
    }
}
