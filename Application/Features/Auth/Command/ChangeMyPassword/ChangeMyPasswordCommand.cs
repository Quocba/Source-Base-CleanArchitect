using Discord.Net;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Command.ChangeMyPassword
{
    public class ChangeMyPasswordCommand : IRequest<ApiResponse<string>>
    {
        [Required(ErrorMessage = "Nhập mật khẩu cũ")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có tối thiểu 6 kí tự")]
        [MaxLength(32, ErrorMessage = "Mật khẩu tối đa chỉ được 32 kí tự")]
        public string NewPassword { get; set; }
    }
}
