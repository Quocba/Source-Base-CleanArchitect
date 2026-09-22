using Application.Payload.Request.Auth;
using Application.Payload.Response.Auth;
using Domain.Payload.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Command.Login
{
    public class LoginCommand : LoginRequest, IRequest<ApiResponse<LoginResposne>>
    {
    }
}
