using Application.Payload.Base.BaseRequest;
using Application.Payload.Base.Paginate;
using Application.Payload.Response.Users;
using Domain.Payload.Base;
using MediatR;

namespace Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQuery : GetListsRequest, IRequest<ApiResponse<ProcedurePagingResponse<UserProfileResponse>>>
    {
    }
}
