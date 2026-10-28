using Domain.Enums;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Application.Features.Users.Command.EditUser
{
    public class EditUserCommand : IRequest<ApiResponse<string>>
    {
        public Guid UserId { get; set; }
        public string? Code { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public long? StorageCapacity { get; set; }
    }
}
