using BaseAPI.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Features.Auth.Command.Login;

namespace API.Controllers
{
    [ApiController]
    [Route(EndpointManage.ApiVersion + "/auth")]
    [AllowAnonymous]
    public class AuthController(IMediator _mediator) : Controller
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginCommand command)
        {
            var response = await _mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }
    }
}
