using Application.Features.Possition.Command.Create;
using Domain.Share.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Features.Possition.Command.Edit;
using Application.Features.Possition.Queries.Gets;

namespace API.Controllers
{
    [ApiController]
    [Route(EndpointManage.ApiVersion + "/posititons")]
    public class PositionController(IMediator _mediator) : Controller
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePosition([FromBody] CreatePositionCommand request)
        {
            var result = await _mediator.Send(request);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPatch("{Id}")]
        [Authorize]
        public async Task<IActionResult> EditPosition(Guid Id, [FromBody] EditPositionCommand request)
        {
            request.Id = Id;
            var result = await _mediator.Send(request);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPositions([FromQuery] GetPositionsQuery request)
        {
            var result = await _mediator.Send(request);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
