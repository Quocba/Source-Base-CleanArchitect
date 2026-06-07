using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Features.Auth.Command.LockAndUnlock
{
    public class LockAndUnLockCommand : IRequest<ApiResponse<string>>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Guid EmployeeId { get; set; }
    }
}
