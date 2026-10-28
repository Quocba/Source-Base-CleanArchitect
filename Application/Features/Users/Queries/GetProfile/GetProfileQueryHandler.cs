using Application.Common.Caching;
using Application.Common.Share.Common;
using Application.Helper;
using Application.Interfaces;
using Application.IService;
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

namespace Application.Features.Users.Queries.GetProfile
{
    public class GetProfileQueryHandler(IUnitOfWork _unitOfWork,
                                        IJWTService _jwtService,
                                        GenericCacheInvalidator<User> _userCache,
                                        DataHelper _dataHelper,
                                        ILogger<GetProfileQueryHandler> _logger)
        : IRequestHandler<GetProfileQuery, ApiResponse<UserProfileResponse>>
    {
        public async Task<ApiResponse<UserProfileResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Guid currentUserId;
                if (request.UserId.HasValue && request.UserId.Value != Guid.Empty)
                {
                    currentUserId = request.UserId.Value;
                }
                else
                {
                    var userIdStr = _jwtService.GetUser();
                    if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out currentUserId))
                    {
                        return new ApiResponse<UserProfileResponse>
                        {
                            StatusCode = StatusCode.Unauthorized,
                            Message = "KhÃ´ng tÃ¬m tháº¥y thÃ´ng tin xÃ¡c thá»±c cá»§a ngÆ°á»i dÃ¹ng",
                            Data = null
                        };
                    }
                }

                var cacheKey = _userCache.GetEntityCacheKey(currentUserId.ToString());

                if (!_userCache.TryGetEntityCache(currentUserId.ToString(), out UserProfileResponse? profile) || profile == null)
                {
                    var proc = new { UserId = currentUserId };
                    var rows = await _dataHelper.ExecuteProcedureAsync<UserProfileResponse>(
                        DBProcedures.GetUserProfile,
                        proc,
                        cancellationToken
                    );

                    profile = rows.FirstOrDefault();
                    if (profile == null)
                    {
                        return new ApiResponse<UserProfileResponse>
                        {
                            StatusCode = StatusCode.BadRequest,
                            Message = "KhÃ´ng tÃ¬m tháº¥y thÃ´ng tin ngÆ°á»i dÃ¹ng",
                            Data = null
                        };
                    }

                    if (profile.StorageCapacity <= 0)
                    {
                        var userCapacityVal = await _unitOfWork.GetRepository<User>()
                                                               .SingleOrDefaultAsync(
                                                                   predicate: x => x.Id == currentUserId && !x.IsDeleted,
                                                                   selector: x => (long?)x.StorageCapacity,
                                                                   disableTracking: true
                                                               );
                        profile.StorageCapacity = userCapacityVal.HasValue && userCapacityVal.Value > 0 ? userCapacityVal.Value : 5368709120L;
                    }

                    _userCache.SetEntityCache(currentUserId.ToString(), profile, TimeSpan.FromDays(1));
                    _userCache.AddToListCacheKeys(cacheKey);
                }

                if (profile.IsLock)
                {
                    return new ApiResponse<UserProfileResponse>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "TÃ i khoáº£n cá»§a báº¡n Ä‘Ã£ bá»‹ khÃ³a",
                        Data = null
                    };
                }

                var usedBytes = 0L;
                var totalBytes = profile.StorageCapacity > 0 ? profile.StorageCapacity : 5368709120L;
                var freeBytes = Math.Max(0L, totalBytes - usedBytes);

                var result = new UserProfileResponse
                {
                    Id = profile.Id,
                    Code = profile.Code,
                    UserName = profile.UserName,
                    Email = profile.Email,
                    Phone = profile.Phone,
                    Avatar = profile.Avatar,
                    Role = profile.Role,
                    IsLock = profile.IsLock,
                    CreatedDate = profile.CreatedDate,
                    CreatedBy = profile.CreatedBy,
                    LastModifiedDate = profile.LastModifiedDate,
                    LastModifiedBy = profile.LastModifiedBy,
                    StorageCapacity = totalBytes,
                    TotalStorage = Math.Round(totalBytes / 1024d / 1024 / 1024, 2),
                    UsedStorage = Math.Round(usedBytes / 1024d / 1024 / 1024, 2),
                    FreeStorage = Math.Round(freeBytes / 1024d / 1024 / 1024, 2),
                    TotalStorageBytes = totalBytes,
                    UsedStorageBytes = usedBytes,
                    FreeStorageBytes = freeBytes
                };

                return new ApiResponse<UserProfileResponse>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Láº¥y thÃ´ng tin ngÆ°á»i dÃ¹ng thÃ nh cÃ´ng",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetProfileQueryHandler] ÄÃ£ cÃ³ lá»—i xáº£y ra khi láº¥y thÃ´ng tin ngÆ°á»i dÃ¹ng");
                throw new Exception("ÄÃ£ cÃ³ lá»—i xáº£y ra khi láº¥y thÃ´ng tin ngÆ°á»i dÃ¹ng", ex);
            }
        }
    }
}
