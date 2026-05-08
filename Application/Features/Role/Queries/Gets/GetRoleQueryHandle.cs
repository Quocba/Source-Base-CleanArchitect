using Application.Payload.Base.Paginate;
using Application.Payload.Response.Role;
using Dapper;
using Domain.Payload.Base;
using Infrastructure.StoreProcedure;
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
            using var connection = _unitOfWork.Context.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            var rows = await connection.QueryAsync<dynamic>(
                DBProcedures.GetRoles,
                new
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    Search = request.Search,
                    Filter = request.Filter
                },
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (!rows.Any())
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

            var first = rows.First();
            int totalRecord = first.TotalRecords;

            var response = new ProcedurePagingResponse<RoleResponse>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecord = totalRecord,
                Items = rows.Select(x => new RoleResponse
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList()
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
