using Application.Payload.Response.Categories;
using Domain.Payload.Base;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.Categories.Queries.Gets
{
    public class GetCategoriesQuery : IRequest<ApiResponse<List<CategoryResponse>>>
    {
        public string? Search { get; set; }
    }
}
