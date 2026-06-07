using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Enum
{
    public enum ETripSheetType
    {
        [Description("Gạo")]
        Rice,

        [Description("Gạo xô")]
        BulkRice
    }
}
