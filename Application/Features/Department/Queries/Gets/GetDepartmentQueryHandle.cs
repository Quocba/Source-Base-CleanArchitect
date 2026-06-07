using Application.Payload.Base.Paginate;
using Application.Payload.Response.Department;
using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Department.Queries.Gets
{
    public class GetDepartmentQueryHandle(IUnitOfWork.IUnitOfWork _unitOfWork)
        : IRequestHandler<GetDepartmentsQuery, ApiResponse<ProcedurePagingResponse<DepartmentResponse>>>
    {
        public async Task<ApiResponse<ProcedurePagingResponse<DepartmentResponse>>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Context.Set<Domain.Entities.Department>()
                                   .Include(x => x.CreatedByNavigation)
                                   .Include(x => x.LastModifiedByNavigation)
                                   .Where(x => x.IsDeleted == false);

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(x => x.Name!.Contains(request.Search) || x.Code!.Contains(request.Search));
            }

            var totalRecord = await query.CountAsync(cancellationToken);

            var items = await query.OrderByDescending(x => x.CreateDate)
                                   .Skip((request.PageNumber - 1) * request.PageSize)
                                   .Take(request.PageSize)
                                   .Select(x => new DepartmentResponse
                                   {
                                       Id = x.Id,
                                       Code = x.Code,
                                       Name = x.Name,
                                       Description = x.Description,
                                       IsDeleted = x.IsDeleted,
                                       CreatedDate = x.CreateDate,
                                       CreatedBy = x.CreatedByNavigation != null ? x.CreatedByNavigation.FullName : x.CreatedBy.ToString(),
                                       LastModifiedDate = x.LastModifiedDate,
                                       LastModifiedBy = x.LastModifiedByNavigation != null ? x.LastModifiedByNavigation.FullName : x.LastModifiedBy.ToString()
                                   })
                                   .ToListAsync(cancellationToken);

            if (!items.Any())
            {
                return new ApiResponse<ProcedurePagingResponse<DepartmentResponse>>
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = "Không có dữ liệu",
                    Data = null
                };
            }

            var response = new ProcedurePagingResponse<DepartmentResponse>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecord = totalRecord,
                Items = items
            };

            return new ApiResponse<ProcedurePagingResponse<DepartmentResponse>>
            {
                StatusCode = StatusCode.OK,
                Message = "Lấy danh sách phòng ban thành công",
                Data = response
            };
        }
    }
}
