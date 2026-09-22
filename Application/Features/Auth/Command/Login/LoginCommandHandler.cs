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
                                 GenericCacheInvalidator<Role> _roleCache,
                                 IJWTService _jwtService,
                                 ILogger<LoginCommandHandler> _logger,
                                 IMemoryCache _cache)
    : IRequestHandler<LoginCommand, ApiResponse<LoginResposne>>
{
    public async Task<ApiResponse<LoginResposne>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = _userCache.GetCacheKeyForSingle("UserName", request.UserName);

            if (!_cache.TryGetValue(cacheKey, out User? user) || user == null)
            {
                user = await _unitOfWork.GetRepository<User>()
                                        .SingleOrDefaultAsync(
                                            predicate: x => x.UserName == request.UserName,
                                            include: q => q.Include(x => x.Role)
                                        );

                if (user != null)
                {
                    _cache.Set(cacheKey, user, TimeSpan.FromDays(1));
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
            if (user.Password != hashedInput)
            {
                return new ApiResponse<LoginResposne>
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = "Mật khẩu không chính xác",
                    Data = null
                };
            }

            var token = _jwtService.GenerateToken(user);

            return new ApiResponse<LoginResposne>
            {
                StatusCode = StatusCode.OK,
                Message = "Đăng nhập thành công",
                Data = new LoginResposne
                {
                    UserName = user.UserName,
                    IsDeleted = user.IsDeleted,
                    IsLock = user.IsLock,
                    Role = user.Role?.Name,
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
