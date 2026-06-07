using Application.Payload.Response.Employee;
using Discord.Net;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Queries.GetMe
{
    public class GetMeQuery : IRequest<ApiResponse<GetMeResponse>>
    {

    }
}
