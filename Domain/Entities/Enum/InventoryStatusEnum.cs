using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Enum
{
    public enum InventoryStatusEnum
    {
        [Description("Chờ xử lý")]
        InProceess = 0,

        [Description("Đã xử lý")]
        Processed = 1
    }
}
