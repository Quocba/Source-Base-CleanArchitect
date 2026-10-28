using Application.Common.Caching;
using Application.Common.Util;
using Application.Interfaces;
using Application.IService;
using Application.Payload.Response.Auth;
using Domain.Entities;
using Domain.Payload.Base;
using Application.Common.Share.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Auth.Command.Login;

public class LoginCommandHandler(IUnitOfWork _unitOfWork,
                                 GenericCacheInvalidator<User> _userCache,
                                 IJWTService _jwtService,
                                 ILogger<LoginCommandHandler> _logger,
                                 IMemoryCache _cache)
    : IRequestHandler<LoginCommand, ApiResponse<LoginResposne>>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(1);

    public async Task<ApiResponse<LoginResposne>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new ApiResponse<LoginResposne>
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = "TÃ i khoáº£n vÃ  máº­t kháº©u khÃ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng",
                    Data = null
                };
            }

            var trimmedUserName = request.UserName.Trim();
            var cacheKey = _userCache.GetCacheKeyForSingle("UserName", trimmedUserName);

            if (!_cache.TryGetValue(cacheKey, out UserAuthCacheModel? user) || user == null)
            {
                user = await _unitOfWork.GetRepository<User>()
                                        .FirstOrDefaultAsync(
                                            predicate: x => x.UserName == trimmedUserName,
                                            selector: x => new UserAuthCacheModel
                                            {
                                                Id = x.Id,
                                                UserName = x.UserName,
                                                PasswordHash = x.Password,
                                                IsDeleted = x.IsDeleted,
                                                IsLock = x.IsLock,
                                                RoleName = x.Role != null ? x.Role.ToString() : null,
                                                StorageCapacity = x.StorageCapacity
                                            },
                                            disableTracking: true
                                        );

                if (user != null)
                {
                    _cache.Set(cacheKey, user, CacheDuration);
                    _userCache.AddToListCacheKeys(cacheKey);
                }
            }

            if (user == null || user.IsDeleted)
            {
                return new ApiResponse<LoginResposne>
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = "TÃ i khoáº£n khÃ´ng tá»“n táº¡i trÃªn há»‡ thá»‘ng",
                    Data = null
                };
            }

            if (user.IsLock)
            {
                return new ApiResponse<LoginResposne>
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = "TÃ i khoáº£n cá»§a báº¡n Ä‘Ã£ bá»‹ khÃ³a",
                    Data = null
                };
            }

            var hashedInput = PasswordUtil.HashPassword(request.Password);
            if (!string.Equals(user.PasswordHash, hashedInput, StringComparison.Ordinal))
            {
                return new ApiResponse<LoginResposne>
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = "Máº­t kháº©u khÃ´ng chÃ­nh xÃ¡c",
                    Data = null
                };
            }

            var token = _jwtService.GenerateToken(user.Id, user.UserName ?? string.Empty, user.RoleName);

            var usedBytes = 0L;
            var totalBytes = user.StorageCapacity > 0 ? user.StorageCapacity : 5368709120L;
            var freeBytes = Math.Max(0L, totalBytes - usedBytes);

            var totalGB = Math.Round(totalBytes / 1024d / 1024 / 1024, 2);
            var usedGB = Math.Round(usedBytes / 1024d / 1024 / 1024, 2);
            var freeGB = Math.Round(freeBytes / 1024d / 1024 / 1024, 2);

            return new ApiResponse<LoginResposne>
            {
                StatusCode = StatusCode.OK,
                Message = "ÄÄƒng nháº­p thÃ nh cÃ´ng",
                Data = new LoginResposne
                {   
                    UserName = user.UserName,
                    IsDeleted = user.IsDeleted,
                    IsLock = user.IsLock,
                    Role = user.RoleName,
                    Token = token,
                    TotalStorage = totalGB,
                    UsedStorage = usedGB,
                    FreeStorage = freeGB,
                    TotalStorageBytes = totalBytes,
                    UsedStorageBytes = usedBytes,
                    FreeStorageBytes = freeBytes
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LoginCommandHandler] ÄÄƒng nháº­p khÃ´ng thÃ nh cÃ´ng, há»‡ thá»‘ng Ä‘ang gáº·p sá»± cá»‘");
            throw new Exception("ÄÄƒng nháº­p khÃ´ng thÃ nh cÃ´ng, há»‡ thá»‘ng Ä‘ang gáº·p sá»± cá»‘. Vui lÃ²ng thá»­ láº¡i sau.", ex);
        }
    }
}
