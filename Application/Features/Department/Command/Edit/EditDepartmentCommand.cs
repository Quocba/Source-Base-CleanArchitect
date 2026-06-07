using Discord.Net;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Features.Department.Command.Edit
{
    public class EditDepartmentCommand : IRequest<ApiResponse<string>>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
