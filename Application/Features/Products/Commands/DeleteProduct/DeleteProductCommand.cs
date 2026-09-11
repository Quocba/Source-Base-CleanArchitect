using Domain.Payload.Base;
using MediatR;
using System;

namespace Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest<ApiResponse<bool>>
    {
        public Guid Id { get; set; }
    }
}
