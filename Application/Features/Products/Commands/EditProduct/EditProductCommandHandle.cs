using Application.IUnitOfWork;
using Domain.Entities;
using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Commands.EditProduct
{
    public class EditProductCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                         ILogger<EditProductCommandHandle> _logger)
        : IRequestHandler<EditProductCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(EditProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _unitOfWork.GetRepository<Product>()
                    .SingleOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted);

                if (product == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.NotFound,
                        Message = "Không tìm thấy sản phẩm cần cập nhật."
                    };
                }

                if (request.CategoryId.HasValue && request.CategoryId != product.CategoryId)
                {
                    bool isCategoryExists = await _unitOfWork.GetRepository<Category>()
                        .AnyAsync(x => x.Id == request.CategoryId.Value && !x.IsDeleted);

                    if (!isCategoryExists)
                    {
                        return new ApiResponse<string>
                        {
                            StatusCode = StatusCode.BadRequest,
                            Message = "Danh mục cập nhật không tồn tại."
                        };
                    }
                }

                product.Name = request.Name.Trim();
                product.Description = request.Description?.Trim();
                product.Price = request.Price;
                product.StockQuantity = request.StockQuantity;
                product.Status = request.Status;
                product.CategoryId = request.CategoryId;
                product.LastModifiedDate = DateTime.UtcNow;

                _unitOfWork.GetRepository<Product>().Update(product);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Cập nhật sản phẩm thành công.",
                    Data = product.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm Id '{Id}': {Message}", request.Id, ex.Message);
                throw;
            }
        }
    }
}
