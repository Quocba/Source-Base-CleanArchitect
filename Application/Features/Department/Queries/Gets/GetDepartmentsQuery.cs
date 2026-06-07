using Application.Payload.Base.BaseRequest;
using Application.Payload.Base.Paginate;
using Application.Payload.Response.Department;
using Discord.Net;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Department.Queries.Gets
{
    public class GetDepartmentsQuery : GetListsRequest, IRequest<ApiResponse<ProcedurePagingResponse<DepartmentResponse>>>
    {
    }
}
