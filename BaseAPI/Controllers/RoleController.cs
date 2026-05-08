using Application.Features.Role.Queries.Gets;
using Domain.Share.Common;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace API.Controllers
{
    [ApiController]
    [Route(EndpointManage.ApiVersion + "/roles")]
    public class RoleController(IMediator _mediator) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetRole([FromQuery]GetRoleQuery request)
        {
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
