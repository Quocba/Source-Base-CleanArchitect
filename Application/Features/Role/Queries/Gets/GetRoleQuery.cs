using Application.Payload.Base.BaseRequest;
using Application.Payload.Base.Paginate;
using Application.Payload.Response.Role;
using Discord.Net;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Role.Queries.Gets
{
    public class GetRoleQuery : GetListsRequest, IRequest<ApiResponse<ProcedurePagingResponse<RoleResponse>>>
    {
    }
}
