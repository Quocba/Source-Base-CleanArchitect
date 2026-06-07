using Discord.Net;
using Domain.Entities.Enum;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employee.Command.Create
{
    public class CreateEmployeeCommand : IRequest<ApiResponse<string>>
    {
        public string UserName { get; set; } = null!;

        [StringLength(32, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có từ 6 đến 32 ký tự")]
        public string Password { get; set; } = null!;
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

        [Required(ErrorMessage = "Vui lòng chọn phòng ban")]
        public Guid DepartmentId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chức vụ")]
        public Guid PositionId { get; set; }

        [Required(ErrorMessage = "Chọn vai trò")]
        public Guid RoleId { get; set; }

        [Required(ErrorMessage = "Vui lòng cho phép/không cho phép sử dụng phần mềm")]
        public bool IsUseSoftWare { get; set; }

    }
}
