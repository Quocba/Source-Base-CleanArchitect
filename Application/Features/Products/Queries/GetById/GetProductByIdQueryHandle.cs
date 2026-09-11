using Application.IUnitOfWork;
using Application.Payload.Response.Products;
using Domain.Entities;
using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Queries.GetById
{
    public class GetProductByIdQueryHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                          ILogger<GetProductByIdQueryHandle> _logger)
        : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductResponse>>
    {
        /*
            1. Tìm kiếm sản phẩm theo Id và include thông tin Category.
            2. Nếu không tìm thấy hoặc IsDeleted = true -> trả về 404 NotFound.
            3. Ánh xạ sang ProductResponse và trả về kết quả.
        */
        public async Task<ApiResponse<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _unitOfWork.Context.Set<Product>()
                    .Include(p => p.Category)
                    .SingleOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

                if (product == null)
                {
                    return new ApiResponse<ProductResponse>
                    {
                        StatusCode = StatusCode.NotFound,
                        Message = "Không tìm thấy sản phẩm yêu cầu."
                    };
                }

                var response = new ProductResponse
                {
                    Id = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    Status = product.Status,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category != null ? product.Category.Name : null,
                    CreatedDate = product.CreatedDate,
                    LastModifiedDate = product.LastModifiedDate
                };

                return new ApiResponse<ProductResponse>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Lấy chi tiết sản phẩm thành công.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết sản phẩm Id '{Id}': {Message}", request.Id, ex.Message);
                throw;
            }
        }
    }
}
