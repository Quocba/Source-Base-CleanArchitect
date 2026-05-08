using Domain.Payload.Base;
using MediatR;
using System;

namespace Application.Features.Auth.Command.ChangePassword
{
    public class ChangePasswordCommand : IRequest<ApiResponse<string>>
    {
        public Guid UserId { get; set; }
        public string NewPassword { get; set; } = null!;
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
