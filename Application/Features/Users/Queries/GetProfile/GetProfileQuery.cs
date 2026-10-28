using Application.Payload.Response.Users;
using Domain.Payload.Base;
using MediatR;
using System;

namespace Application.Features.Users.Queries.GetProfile
{
    public class GetProfileQuery : IRequest<ApiResponse<UserProfileResponse>>
    {
        public Guid? UserId { get; set; }
    }
}
