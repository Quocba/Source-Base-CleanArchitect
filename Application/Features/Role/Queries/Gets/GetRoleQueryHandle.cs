using Application.Payload.Base.Paginate;
using Application.Payload.Response.Role;
using Domain.Payload.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Role.Queries.Gets
{
    public class GetRoleQueryHandle(IUnitOfWork.IUnitOfWork _unitOfWork)
        : IRequestHandler<GetRoleQuery, ApiResponse<ProcedurePagingResponse<RoleResponse>>>
    {
        public async Task<ApiResponse<ProcedurePagingResponse<RoleResponse>>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Context.Set<Domain.Entities.Role>().AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(x => x.Name!.Contains(request.Search));
            }

            var totalRecord = await query.CountAsync(cancellationToken);

            var items = await query.OrderBy(x => x.Id)
                                   .Skip((request.PageNumber - 1) * request.PageSize)
                                   .Take(request.PageSize)
                                   .Select(x => new RoleResponse
                                   {
                                       Id = x.Id,
                                       Name = x.Name
                                   })
                                   .ToListAsync(cancellationToken);

            if (!items.Any())
            {
                return new ApiResponse<ProcedurePagingResponse<RoleResponse>>
                {
                    StatusCode = 204,
                    Message = "No Content",
                    Data = new ProcedurePagingResponse<RoleResponse>
                    {
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalRecord = 0,
                        Items = new List<RoleResponse>()
                    }
                };
            }

            var response = new ProcedurePagingResponse<RoleResponse>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecord = totalRecord,
                Items = items
            };

            return new ApiResponse<ProcedurePagingResponse<RoleResponse>>
            {
                StatusCode = 200,
                Message = "Lấy dữ liệu thành công",
                Data = response
            };
        }
    }
}
