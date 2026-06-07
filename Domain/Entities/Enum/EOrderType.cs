using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Enum
{
    public enum EOrderType
    {
        [Description("Nợ")]
        Debt,

        [Description("Trả tiền mặt")]
        Cash
    }
}
