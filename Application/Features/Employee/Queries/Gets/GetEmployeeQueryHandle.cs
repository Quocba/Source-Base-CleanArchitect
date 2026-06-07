using Application.Payload.Base.Paginate;
using Application.Payload.Response.Employee;
using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Employee.Queries.Gets
{
    public class GetEmployeeQueryHandle(IUnitOfWork.IUnitOfWork _unitOfWork)
        : IRequestHandler<GetEmployeeQuery, ApiResponse<ProcedurePagingResponse<GetEmployeeResponse>>>
    {
        public async Task<ApiResponse<ProcedurePagingResponse<GetEmployeeResponse>>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Context.Set<Domain.Entities.Employee>()
                                   .Include(e => e.User)
                                   .ThenInclude(u => u.Role)
                                   .Include(e => e.Department)
                                   .Include(e => e.Position)
                                   .Include(e => e.CreatedByNavigation)
                                   .Include(e => e.LastModifiedByNavigation)
                                   .Where(e => e.IsDeleted == false || e.IsDeleted == null);

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(e => e.FullName!.Contains(request.Search) ||
                                         e.Code!.Contains(request.Search) ||
                                         e.Phone!.Contains(request.Search) ||
                                         e.Email!.Contains(request.Search));
            }

            var totalRecord = await query.CountAsync(cancellationToken);

            var items = await query.OrderByDescending(e => e.CreatedDate)
                                   .Skip((request.PageNumber - 1) * request.PageSize)
                                   .Take(request.PageSize)
                                   .Select(e => new GetEmployeeResponse
                                   {
                                       Id = e.Id,
                                       Code = e.Code,
                                       FullName = e.FullName,
                                       Gender = e.Gender,
                                       Phone = e.Phone,
                                       Email = e.Email,
                                       Address = e.Address,
                                       DBO = e.DBO,
                                       Avatar = e.Avatar,
                                       Bank = e.Bank,
                                       BankNo = e.BankNo,
                                       BankAccountHolder = e.BankAccountHolder,
                                       Status = e.Status,
                                       DepartmentId = e.DepartmentId,
                                       Department = e.Department != null ? e.Department.Name : null,
                                       PositionId = e.PositionId,
                                       Position = e.Position != null ? e.Position.Name : null,
                                       IsDeleted = e.IsDeleted,
                                       CreatedDate = e.CreatedDate,
                                       LastModifiedDate = e.LastModifiedDate,
                                       CreatedBy = e.CreatedByNavigation != null ? e.CreatedByNavigation.FullName! : "",
                                       LastModifiedBy = e.LastModifiedByNavigation != null ? e.LastModifiedByNavigation.FullName! : "",
                                       RoleId = e.User != null ? e.User.RoleId : Guid.Empty,
                                       Role = (e.User != null && e.User.Role != null) ? e.User.Role.Name! : ""
                                   })
                                   .ToListAsync(cancellationToken);

            if (!items.Any())
            {
                return new ApiResponse<ProcedurePagingResponse<GetEmployeeResponse>>
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = "No Content",
                    Data = new ProcedurePagingResponse<GetEmployeeResponse>
                    {
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalRecord = 0,
                        Items = new List<GetEmployeeResponse>()
                    }
                };
            }

            var response = new ProcedurePagingResponse<GetEmployeeResponse>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecord = totalRecord,
                Items = items
            };

            return new ApiResponse<ProcedurePagingResponse<GetEmployeeResponse>>
            {
                StatusCode = 200,
                Message = "Lấy dữ liệu thành công",
                Data = response
            };
        }
    }
}
