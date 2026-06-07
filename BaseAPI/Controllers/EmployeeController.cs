using Application.Features.Employee.Command.Create;
using Domain.Share.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Features.Employee.Command.Edit;
using Application.Features.Employee.Queries.Gets;

namespace API.Controllers
{
    [ApiController]
    [Route(EndpointManage.ApiVersion + "/employees")]
    public class EmployeeController(IMediator _mediator) : Controller
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeCommand request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _mediator.Send(request);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{Id}")]
        [Authorize]
        public async Task<IActionResult> EditEmployee([FromRoute] Guid Id, [FromBody] EditEmployeeCommand request)
        {
            request.Id = Id;
            var response = await _mediator.Send(request);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetEmployees([FromQuery]GetEmployeeQuery request)
        {
             var response = await _mediator.Send(request);
             return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{warehouseId}/warehouse")]
        [Authorize]
        public async Task<IActionResult> GetByWareHouse(Guid warehouseId,[FromQuery] GetEmployeeQuery request)
        {
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
