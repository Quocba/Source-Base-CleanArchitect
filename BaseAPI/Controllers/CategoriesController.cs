using Application.Features.Categories.Commands.CreateCategory;
using Application.Features.Categories.Queries.Gets;
using BaseAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BaseAPI.Controllers
{
    [ApiController]
    [Route(EndpointManage.ApiVersion + "/categories")]
    public class CategoriesController(IMediator _mediator) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand request)
        {
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] GetCategoriesQuery request)
        {
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
