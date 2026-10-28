using Application.Features.Users.Command.CreateUser;
using Application.Features.Users.Command.DeleteUser;
using Application.Features.Users.Command.EditProfile;
using Application.Features.Users.Command.EditUser;
using Application.Features.Users.Command.LockUser;
using Application.Features.Users.Queries.GetProfile;
using Application.Features.Users.Queries.GetUsers;
using BaseAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BaseAPI.Controllers
{
    [ApiController]
    [Route(EndpointManage.Users.Base)]
    [Authorize]
    public class UsersController(IMediator _mediator) : Controller
    {
        [HttpPost]
        [Authorize(Roles = "SYSTEM")]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var response = await _mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "SYSTEM")]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromBody] EditUserCommand command)
        {
            command.UserId = id;
            var response = await _mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "SYSTEM")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _mediator.Send(new DeleteUserCommand { UserId = id });
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("{id:guid}/lock")]
        [Authorize(Roles = "SYSTEM")]
        public async Task<IActionResult> Lock([FromRoute] Guid id, [FromBody] LockUserCommand? command)
        {
            command ??= new LockUserCommand();
            command.UserId = id;
            var response = await _mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var response = await _mediator.Send(new GetProfileQuery());
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileCommand command)
        {
            var response = await _mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("profile")]
        [Authorize]
        public async Task<IActionResult> PatchProfile([FromBody] EditProfileCommand command)
        {
            var response = await _mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Authorize(Roles = "SYSTEM")]
        public async Task<IActionResult> Gets([FromQuery] GetUsersQuery query)
        {
            var response = await _mediator.Send(query);
            return StatusCode(response.StatusCode, response);
        }
    }
}
