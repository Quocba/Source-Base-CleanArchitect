using Application.Features.PaymentVouchers.Command.CreatePaymentVoucher;
using Domain.Payload.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace NgocDaiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentVouchersController : BaseController<PaymentVouchersController>
    {
        [HttpPost]
        public async Task<IActionResult> Create(CreatePaymentVoucherCommand command)
        {
            var response = await Mediator.Send(command);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
