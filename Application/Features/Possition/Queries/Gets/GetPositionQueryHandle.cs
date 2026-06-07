using Application.Payload.Base.Paginate;
using Application.Payload.Response.Position;
using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Possition.Queries.Gets
{
    public class GetPositionQueryHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                        ILogger<GetPositionQueryHandle> _logger)
        : IRequestHandler<GetPositionsQuery, ApiResponse<ProcedurePagingResponse<PositionResponse>>>
    {
        public async Task<ApiResponse<ProcedurePagingResponse<PositionResponse>>> Handle(GetPositionsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Context.Set<Domain.Entities.Position>()
                                   .Where(x => x.IsDeleted == false);

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(x => x.Name!.Contains(request.Search));
            }

            var totalRecord = await query.CountAsync(cancellationToken);

            var items = await query.OrderBy(x => x.Id)
                                   .Skip((request.PageNumber - 1) * request.PageSize)
                                   .Take(request.PageSize)
                                   .Select(row => new PositionResponse
                                   {
                                       Id = row.Id,
                                       Name = row.Name,
                                       Description = row.Description,
                                       IsDeleted = row.IsDeleted
                                   })
                                   .ToListAsync(cancellationToken);

            if (!items.Any())
            {
                return new ApiResponse<ProcedurePagingResponse<PositionResponse>>
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = "Không có dữ liệu",
                    Data = null
                };
            }

            var response = new ProcedurePagingResponse<PositionResponse>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecord = totalRecord,
                Items = items
            };

            return new ApiResponse<ProcedurePagingResponse<PositionResponse>>
            {
                StatusCode = 200,
                Message = "Lấy danh sách chức vụ thành công",
                Data = response
            };
        }
    }
}
