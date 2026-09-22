using Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("LicenseDevices")]
    public class LicenseDevices : BaseEntity<Guid>
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }

        public Guid LicenseId { get; set; }

        [ForeignKey(nameof(LicenseId))]
        public virtual License? License { get; set; }
    }
}
