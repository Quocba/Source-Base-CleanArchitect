using Application.IUnitOfWork;
using Application.Payload.Response.Categories;
using Domain.Entities;
using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Categories.Queries.Gets
{
    public class GetCategoriesQueryHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                         ILogger<GetCategoriesQueryHandle> _logger)
        : IRequestHandler<GetCategoriesQuery, ApiResponse<List<CategoryResponse>>>
    {
        public async Task<ApiResponse<List<CategoryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _unitOfWork.Context.Set<Category>()
                    .Include(c => c.Products)
                    .Where(c => !c.IsDeleted && c.IsActive);

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLower();
                    query = query.Where(c => c.Name.ToLower().Contains(search) || c.Code.ToLower().Contains(search));
                }

                var categories = await query.OrderBy(c => c.Name)
                    .Select(c => new CategoryResponse
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        Description = c.Description,
                        IsActive = c.IsActive,
                        ProductCount = c.Products.Count(p => !p.IsDeleted),
                        CreatedDate = c.CreatedDate
                    })
                    .ToListAsync(cancellationToken);

                return new ApiResponse<List<CategoryResponse>>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Lấy danh sách danh mục thành công.",
                    Data = categories
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách danh mục: {Message}", ex.Message);
                throw;
            }
        }
    }
}
