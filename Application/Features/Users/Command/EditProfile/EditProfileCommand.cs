using Domain.Payload.Base;
using MediatR;

namespace Application.Features.Users.Command.EditProfile
{
    public class EditProfileCommand : IRequest<ApiResponse<string>>
    {
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
    }
}
