using Application.IUnitOfWork;
using Domain.Entities;
using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                           ILogger<DeleteProductCommandHandle> _logger)
        : IRequestHandler<DeleteProductCommand, ApiResponse<bool>>
    {
        public async Task<ApiResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _unitOfWork.GetRepository<Product>()
                    .SingleOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted);

                if (product == null)
                {
                    return new ApiResponse<bool>
                    {
                        StatusCode = StatusCode.NotFound,
                        Message = "Không tìm thấy sản phẩm cần xóa.",
                        Data = false
                    };
                }

                product.IsDeleted = true;
                product.LastModifiedDate = DateTime.UtcNow;

                _unitOfWork.GetRepository<Product>().Update(product);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<bool>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Xóa sản phẩm thành công.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm Id '{Id}': {Message}", request.Id, ex.Message);
                throw;
            }
        }
    }
}
