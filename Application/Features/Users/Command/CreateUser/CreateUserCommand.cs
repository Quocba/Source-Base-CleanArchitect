using Domain.Enums;
using Domain.Payload.Base;
using MediatR;

namespace Application.Features.Users.Command.CreateUser
{
    public class CreateUserCommand : IRequest<ApiResponse<string>>
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public ERole Role { get; set; } = ERole.USER;
        public long? StorageCapacity { get; set; }
    }
}
