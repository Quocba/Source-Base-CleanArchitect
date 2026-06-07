using Discord.Net;
using Domain.Entities.Enum;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Features.Employee.Command.Edit
{
    public class EditEmployeeCommand : IRequest<ApiResponse<string>>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Guid Id { get; set; }
        public string? FullName { get; set; } = null!;
        public GenderEnum? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime? DBO { get; set; }
        public string? Avatar { get; set; }


        public string? Bank { get; set; }
        public string? BankNo { get; set; }
        public string? BankAccountHolder { get; set; }
        public EmployeeStatusEnum Status { get; set; }
        public Guid? DepartmentId { get; set; }

        public Guid? PositionId { get; set; }



        public Guid? RoleId { get; set; }
    }
}
