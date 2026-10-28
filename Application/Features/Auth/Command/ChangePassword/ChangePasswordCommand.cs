using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Features.Auth.Command.ChangePassword
{
    public class ChangePasswordCommand : IRequest<ApiResponse<string>>
    {
        [Required(ErrorMessage = "Nhập mật khẩu cũ")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu mới")]
        public string NewPassword { get; set; }
    }
}
