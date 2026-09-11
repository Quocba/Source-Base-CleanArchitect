using Application.IUnitOfWork;
using Domain.Entities;
using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                            ILogger<CreateCategoryCommandHandle> _logger)
        : IRequestHandler<CreateCategoryCommand, ApiResponse<string>>
    {
        /*
            1. Kiểm tra trùng mã danh mục.
            2. Tạo entity Category mới.
            3. Lưu vào Database và trả về kết quả.
        */
        public async Task<ApiResponse<string>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool isCodeExists = await _unitOfWork.GetRepository<Category>()
                    .AnyAsync(x => x.Code == request.Code && !x.IsDeleted);

                if (isCodeExists)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.Conflict,
                        Message = $"Mã danh mục '{request.Code}' đã tồn tại."
                    };
                }

                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Code = request.Code.Trim().ToUpper(),
                    Name = request.Name.Trim(),
                    Description = request.Description?.Trim(),
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _unitOfWork.GetRepository<Category>().AddAsync(category);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.Created,
                    Message = "Tạo danh mục thành công.",
                    Data = category.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo danh mục: {Message}", ex.Message);
                throw;
            }
        }
    }
}
