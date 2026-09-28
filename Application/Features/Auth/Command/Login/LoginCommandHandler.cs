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
                    Message = "Tài khoản và mật khẩu không được để trống",
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
                                                RoleName = x.Role != null ? x.Role.Name : null
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
                    Message = "Tài khoản không tồn tại trên hệ thống",
                    Data = null
                };
            }

            if (user.IsLock)
            {
                return new ApiResponse<LoginResposne>
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = "Tài khoản của bạn đã bị khoá",
                    Data = null
                };
            }

            var hashedInput = PasswordUtil.HashPassword(request.Password);
            if (!string.Equals(user.PasswordHash, hashedInput, StringComparison.Ordinal))
            {
                return new ApiResponse<LoginResposne>
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = "Mật khẩu không chính xác",
                    Data = null
                };
            }

            var token = _jwtService.GenerateToken(user.Id, user.UserName ?? string.Empty, user.RoleName);

            return new ApiResponse<LoginResposne>
            {
                StatusCode = StatusCode.OK,
                Message = "Đăng nhập thành công",
                Data = new LoginResposne
                {   
                    UserName = user.UserName,
                    IsDeleted = user.IsDeleted,
                    IsLock = user.IsLock,
                    Role = user.RoleName,
                    Token = token
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LoginCommandHandler] Đăng nhập không thành công, hệ thống đang gặp sự cố");
            throw new Exception("Đăng nhập không thành công hệ thống đang gặp sự cố, vui lòng thử lại sau");
        }
    }
}
