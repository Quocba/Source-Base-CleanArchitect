using Application.Features.Auth.Command.Login;
using Application.Features.Auth.Command.LockAndUnlock;
using Application.Features.Auth.Command.ChangePassword;
using Domain.Share.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Features.Auth.Command.ChangeMyPassword;
using System.Diagnostics.CodeAnalysis;
using Application.Features.Auth.Queries.GetMe;
using Application.Features.Auth.Command.EditMyInfo;

namespace API.Controllers
{
    [ApiController]
    [Route(EndpointManage.ApiVersion + "/auth")]
    public class AuthController(IMediator _mediator) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        {
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("{id}/lock")]
        [Authorize(Roles = "Super, Manager")]
        public async Task<IActionResult> LockAndUnLockUser(Guid id)
        {
            var command = new LockAndUnLockCommand { EmployeeId = id };
            var response = await _mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordCommand command)
        {
            command.EmployeeId = id;
            var response = await _mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("change-my-password")]
        [Authorize]
        public async Task<IActionResult> CHangeMyPassword([FromBody]ChangeMyPasswordCommand request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            GetMeQuery request = new GetMeQuery();
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("edit-info")]
        [Authorize]
        public async Task<IActionResult> EditInfo([FromBody] EditInfoCommand request)
        {
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
