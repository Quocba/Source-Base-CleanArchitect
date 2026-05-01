using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Enum
{
    public enum WareHouseStatusEnum
    {
        [Description("Đang hoạt động")]
        Active = 1,

        [Description("Ngừng hoạt động")]
        Inactive = 0
    }
}
