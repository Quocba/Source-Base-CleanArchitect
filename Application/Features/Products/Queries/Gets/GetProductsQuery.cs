using Application.Payload.Base.BaseRequest;
using Application.Payload.Base.Paginate;
using Application.Payload.Response.Products;
using Domain.Payload.Base;
using MediatR;

namespace Application.Features.Products.Queries.Gets
{
    public class GetProductsQuery : GetListsRequest, IRequest<ApiResponse<ProcedurePagingResponse<ProductResponse>>>
    {
    }
}
