using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Products.Commands.DeleteProduct;
using Application.Features.Products.Commands.EditProduct;
using Application.Features.Products.Queries.GetById;
using Application.Features.Products.Queries.Gets;
using BaseAPI.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BaseAPI.Controllers
{
    [ApiController]
    [Route(EndpointManage.ApiVersion + "/products")]
    public class ProductsController(IMediator _mediator) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand request)
        {
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] EditProductCommand request)
        {
            request.Id = id;
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteProductCommand { Id = id });
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _mediator.Send(new GetProductByIdQuery { Id = id });
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery request)
        {
            var response = await _mediator.Send(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
