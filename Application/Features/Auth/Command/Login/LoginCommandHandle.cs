using Application.IService;
using Application.IUnitOfWork;
using Application.Payload.Response.Auth;
using Domain.Payload.Base;
using Domain.Share.Common;
using Domain.Share.Util;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Auth.Command.Login
{
    public class LoginCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                    ILogger<LoginCommandHandle> _logger,
                                    IJWTService _jwtService,
                                    IMemoryCache _cache)
        : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
    {
        private static readonly System.Threading.SemaphoreSlim _semaphore = new(1, 1);

        public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string cacheKey = $"User_{request.UserName}";
                
                if (!_cache.TryGetValue(cacheKey, out object userObj))
                {
                    await _semaphore.WaitAsync(cancellationToken);
                    try
                    {
                        if (!_cache.TryGetValue(cacheKey, out userObj))
                        {
                            userObj = await _unitOfWork
                                .GetRepository<Domain.Entities.User>()
                                .SingleOrDefaultAsync(
                                    predicate: x => x.UserName == request.UserName && x.IsDeleted == false,
                                    selector: x => new
                                    {
                                        x.Id,
                                        x.UserName,
                                        x.Password,
                                        x.IsLock,
                                        RoleName = x.Role.Name
                                    });

                            if (userObj != null)
                            {
                                _cache.Set(cacheKey, userObj, TimeSpan.FromMinutes(5));
                            }
                        }
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }

                dynamic userDto = userObj;

                if (userDto == null)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Tên đăng nhập không tồn tại"
                    };
                }

                var hashedInput = PasswordUtil.HashPassword(request.Password);
                if (hashedInput != userDto.Password)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Mật khẩu không đúng"
                    };
                }

                if (userDto.IsLock == true)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Tài khoản của bạn đã bị khóa"
                    };
                }

                var userEntity = new Domain.Entities.User
                {
                    Id = userDto.Id,
                    UserName = userDto.UserName,
                    Role = new Domain.Entities.Role { Name = userDto.RoleName }
                };

                var token = _jwtService.GenerateToken(userEntity);

                return new ApiResponse<LoginResponse>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Đăng nhập thành công",
                    Data = new LoginResponse
                    {
                        UserName = userDto.UserName,
                        Token = token,
                        Role = userDto.RoleName
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Login Feature Error]");
                return new ApiResponse<LoginResponse>
                {
                    StatusCode = StatusCode.InternalServerError,
                    Message = "Có lỗi xảy ra trong quá trình đăng nhập"
                };
            }
        }
    }
}
