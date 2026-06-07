using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Enum
{
    public enum EBankStatus
    {
        [Description("Đang hoạt động")]
        Active,

        [Description("Ngừng hoạt động")]
        Inactive
    }
}
