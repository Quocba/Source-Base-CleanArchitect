using Application.IUnitOfWork;
using Domain.Entities;
using Domain.Payload.Base;
using Domain.Share.Common;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using RabbitMQContract.Payload.Product;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                           ILogger<CreateProductCommandHandle> _logger,
                                           IPublishEndpoint _publishEndpoint)
        : IRequestHandler<CreateProductCommand, ApiResponse<string>>
    {
        /*
            1. Validate: Kiểm tra trùng mã sản phẩm (Code).
            2. Nếu CategoryId có giá trị, kiểm tra danh mục có tồn tại không.
            3. Tạo thực thể Product mới.
            4. Lưu dữ liệu vào Database qua UnitOfWork.
            5. Gửi message (ProductCreatedMessage) vào RabbitMQ để xử lý bất đồng bộ (cache, notify).
            6. Trả về kết quả thành công với StatusCode = 201 Created.
        */
        public async Task<ApiResponse<string>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool isCodeExists = await _unitOfWork.GetRepository<Product>()
                    .AnyAsync(x => x.Code == request.Code && !x.IsDeleted);

                if (isCodeExists)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.Conflict,
                        Message = $"Mã sản phẩm '{request.Code}' đã tồn tại trong hệ thống."
                    };
                }

                if (request.CategoryId.HasValue)
                {
                    bool isCategoryExists = await _unitOfWork.GetRepository<Category>()
                        .AnyAsync(x => x.Id == request.CategoryId.Value && !x.IsDeleted);

                    if (!isCategoryExists)
                    {
                        return new ApiResponse<string>
                        {
                            StatusCode = StatusCode.BadRequest,
                            Message = "Danh mục được chọn không tồn tại."
                        };
                    }
                }

                var newProduct = new Product
                {
                    Id = Guid.NewGuid(),
                    Code = request.Code.Trim().ToUpper(),
                    Name = request.Name.Trim(),
                    Description = request.Description?.Trim(),
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    Status = request.Status,
                    CategoryId = request.CategoryId,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _unitOfWork.GetRepository<Product>().AddAsync(newProduct);
                await _unitOfWork.CommitAsync();

                // Bắn Event sang RabbitMQ để các Consumer xử lý nền (Consumer Example)
                try
                {
                    await _publishEndpoint.Publish(new ProductCreatedMessage
                    {
                        ProductId = newProduct.Id,
                        Code = newProduct.Code,
                        Name = newProduct.Name,
                        Price = newProduct.Price,
                        CreatedDate = newProduct.CreatedDate
                    }, cancellationToken);
                }
                catch (Exception exRabbit)
                {
                    // Log cảnh báo nhưng không làm đứt luồng tạo sản phẩm chính
                    _logger.LogWarning(exRabbit, "Không thể gửi message vào RabbitMQ: {Message}", exRabbit.Message);
                }

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.Created,
                    Message = "Tạo mới sản phẩm thành công.",
                    Data = newProduct.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới sản phẩm: {Message}", ex.Message);
                throw;
            }
        }
    }
}
