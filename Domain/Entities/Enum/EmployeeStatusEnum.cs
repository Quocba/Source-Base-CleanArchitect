using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Enum
{
    public enum EmployeeStatusEnum
    {
        [Description("Đang hoạt động")]
        Active = 0,
        [Description("Nghỉ Phép")]
        OnLeave = 1,
        [Description("Ngừng Hoạt Động")]
        InActive = 2
    }
}
