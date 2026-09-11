using Application.Payload.Response.Products;
using Domain.Payload.Base;
using MediatR;
using System;

namespace Application.Features.Products.Queries.GetById
{
    public class GetProductByIdQuery : IRequest<ApiResponse<ProductResponse>>
    {
        public Guid Id { get; set; }
    }
}
