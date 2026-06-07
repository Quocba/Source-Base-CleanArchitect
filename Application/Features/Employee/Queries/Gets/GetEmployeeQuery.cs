using Application.Payload.Base.BaseRequest;
using Application.Payload.Base.Paginate;
using Application.Payload.Response.Employee;
using Discord.Net;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employee.Queries.Gets
{
    public class GetEmployeeQuery : GetListsRequest, IRequest<ApiResponse<ProcedurePagingResponse<GetEmployeeResponse>>>
    {
    }
}
