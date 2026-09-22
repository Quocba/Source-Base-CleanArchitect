using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum ESystemType
    {
        [Description("DCS-LabControl")]
        DCSLabControl,

        [Description("DCS-NgocDai")]
        DCSNgocDai
    }
}
