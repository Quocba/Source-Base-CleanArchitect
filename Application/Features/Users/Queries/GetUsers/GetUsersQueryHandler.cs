using Application.Common.Caching;
using Application.Common.Share.Common;
using Application.Helper;
using Application.Interfaces;
using Application.Payload.Base.Paginate;
using Application.Payload.Response.Users;
using Application.StoreProcedure;
using Domain.Entities;
using Domain.Payload.Base;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQueryHandler(IUnitOfWork _unitOfWork,
                                      GenericCacheInvalidator<User> _userCache,
                                      DataHelper _dataHelper,
                                      ILogger<GetUsersQueryHandler> _logger)
        : IRequestHandler<GetUsersQuery, ApiResponse<ProcedurePagingResponse<UserProfileResponse>>>
    {
        public async Task<ApiResponse<ProcedurePagingResponse<UserProfileResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var parameters = new ListParameters<User>(request.PageNumber, request.PageSize);
                parameters.AddFilter("Search", request.Search);
                parameters.AddFilter("Filter", request.Filter);

                var cacheKey = _userCache.GetCacheKey(parameters);

                if (_userCache.TryGetListCache(cacheKey, out ProcedurePagingResponse<UserProfileResponse>? cacheData) && cacheData != null)
                {
                    return new ApiResponse<ProcedurePagingResponse<UserProfileResponse>>
                    {
                        StatusCode = StatusCode.OK,
                        Message = "Lấy dữ liệu thành công(Cache)",
                        Data = cacheData
                    };
                }

                var proc = new
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    Search = request.Search,
                    Filter = request.Filter
                };

                var rows = await _dataHelper.ExecuteProcedureAsync(
                    DBProcedures.GetUsers,
                    proc,
                    cancellationToken
                );

                if (!rows.Any())
                {
                    return new ApiResponse<ProcedurePagingResponse<UserProfileResponse>>
                    {
                        StatusCode = StatusCode.OK,
                        Message = "Không có dữ liệu",
                        Data = null
                    };
                }

                var response = new ProcedurePagingResponse<UserProfileResponse>
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecord = rows.First().TotalRecord,
                    Items = rows.Select(x => new UserProfileResponse
                    {
                        Id = x.Id,
                        Code = x.Code,
                        UserName = x.UserName,
                        Email = x.Email,
                        Phone = x.Phone,
                        Avatar = x.Avatar,
                        Role = x.Role,
                        IsLock = x.IsLock,
                        CreatedDate = x.CreatedDate,
                        CreatedBy = x.CreatedBy,
                        LastModifiedDate = x.LastModifiedDate,
                        LastModifiedBy = x.LastModifiedBy
                    }).ToList()
                };

                _userCache.SetListCache(cacheKey, response);

                return new ApiResponse<ProcedurePagingResponse<UserProfileResponse>>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Lấy dữ liệu thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetUsersQueryHandler] Đã có lỗi xảy ra khi lấy danh sách người dùng");
                throw new Exception("Đã có lỗi xảy ra khi lấy danh sách người dùng", ex);
            }
        }
    }
}
