using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Enum
{
    public enum EBulkRiceRiceLotStatus
    {
        [Description("Đang xử lý")]
        InProcess,

        [Description("Đã hoàn thành")]
        Done
    }
}
