using Domain.Payload.Base;
using MediatR;
using System;

namespace Application.Features.Users.Command.DeleteUser
{
    public class DeleteUserCommand : IRequest<ApiResponse<string>>
    {
        public Guid UserId { get; set; }
    }
}
