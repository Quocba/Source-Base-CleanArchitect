using Domain.Payload.Base;
using MediatR;
using System;

namespace Application.Features.Users.Command.LockUser
{
    public class LockUserCommand : IRequest<ApiResponse<string>>
    {
        public Guid UserId { get; set; }
        public bool? IsLock { get; set; }
    }
}
