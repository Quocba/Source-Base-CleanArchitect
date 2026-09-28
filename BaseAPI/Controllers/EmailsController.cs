using Application.Features.Emails.Commands.SendEmailQueue;
using BaseAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BaseAPI.Controllers
{
    [ApiController]
    [Route(EndpointManage.ApiVersion + "/emails")]
    public class EmailsController(IMediator _mediator) : Controller
    {
        // Tạm thời comment do chưa dùng RabbitMQ
        // [HttpPost("send-queue")]
        // public async Task<IActionResult> SendQueue([FromBody] SendEmailQueueCommand request)
        // {
        //     var response = await _mediator.Send(request);
        //     return StatusCode(response.StatusCode, response);
        // }
    }
}
