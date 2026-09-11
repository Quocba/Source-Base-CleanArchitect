using Application.Helper;
using Application.Payload.Base.Paginate;
using Application.Payload.Response.Products;
using Application.StoreProcedure;
using Domain.Enums;
using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Queries.Gets
{
    public class GetProductsQueryHandle(DataHelper _dataHelper)
        : IRequestHandler<GetProductsQuery, ApiResponse<ProcedurePagingResponse<ProductResponse>>>
    {
        public async Task<ApiResponse<ProcedurePagingResponse<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var rows = await _dataHelper.ExecuteProcedureAsync<dynamic>(
                DBProcedures.GetProducts,
                new
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    Search = request.Search
                },
                cancellationToken
            );

            if (!rows.Any())
            {
                return new ApiResponse<ProcedurePagingResponse<ProductResponse>>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Không có dữ liệu",
                    Data = new ProcedurePagingResponse<ProductResponse>()
                };
            }

            var first = rows.First();
            int totalRecord = first.TotalRecords;

            var response = new ProcedurePagingResponse<ProductResponse>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecord = totalRecord,
                Items = rows.Select(x => new ProductResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    StockQuantity = x.StockQuantity,
                    Status = (ProductStatus)x.Status,
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    CreatedDate = x.CreatedDate,
                    LastModifiedDate = x.LastModifiedDate
                }).ToList()
            };

            return new ApiResponse<ProcedurePagingResponse<ProductResponse>>
            {
                StatusCode = StatusCode.OK,
                Message = "Lấy danh sách sản phẩm thành công",
                Data = response
            };
        }
    }
}
