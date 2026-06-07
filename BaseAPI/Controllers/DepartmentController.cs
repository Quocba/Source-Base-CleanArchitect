using Application.Features.Department.Command.Create;
using Domain.Share.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Features.Department.Command.Edit;
using Application.Features.Department.Queries.Gets;

namespace API.Controllers
{
    [ApiController]
    [Route(EndpointManage.ApiVersion + "/departments")]
    public class DepartmentController(IMediator _mediator) : Controller
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand request)
        {
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("{Id}")]
        [Authorize]
        public async Task<IActionResult> Edit(Guid Id, [FromBody] EditDepartmentCommand request)
        {
            request.Id = Id;
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDepartments([FromQuery] GetDepartmentsQuery request)
        {
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
