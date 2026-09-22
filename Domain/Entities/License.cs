using Domain.Entities.Base;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("License")]
    public class License : BaseEntity<Guid>
    {
        [MaxLength(200)]
        public string LicenseKey { get; set; }

        public ESystemType SystemType { get; set; }

        [MaxLength(255)]
        public string CustomerName { get; set; }
        public int MaxDevice { get; set; }

        public DateTime ExpiresAt { get; set; }

        public ELicenseStatus Status { get; set; }

        public string Note { get; set; }

        public virtual List<LicenseDevices>? LicenseDevices { get; set; }
    }
}
