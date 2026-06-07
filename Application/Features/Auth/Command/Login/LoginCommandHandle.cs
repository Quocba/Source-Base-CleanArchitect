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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable
namespace Application.Features.Auth.Command.Login
{
    public class LoginCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                    ILogger<LoginCommand> _logger,
                                    IJWTService _jwtService,
                                    Microsoft.Extensions.Caching.Memory.IMemoryCache _cache)
        : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
    {
        private static readonly System.Threading.SemaphoreSlim _semaphore = new(1, 1);

        async Task<ApiResponse<LoginResponse>> IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>.Handle(LoginCommand request, CancellationToken cancellationToken)
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
                                    predicate: x => x.UserName == request.UserName,
                                    selector: x => new
                                    {
                                        x.UserName,
                                        x.Password,
                                        x.IsLock,
                                        RoleName = x.Role.Name,
                                        Employee = x.Employees.Select(e => new { e.Id, e.Avatar, e.FullName }).FirstOrDefault()
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
                        Message = "Tài khoản của bạn đã bị khóa",
                        Data = null
                    };
                }

                var userEntity = new Domain.Entities.User
                {
                    UserName = userDto.UserName,
                    Role = new Domain.Entities.Role { Name = userDto.RoleName },
                    Employees = userDto.Employee != null
                        ? new List<Domain.Entities.Employee> { new Domain.Entities.Employee { Id = userDto.Employee.Id } }
                        : new List<Domain.Entities.Employee>()
                };

                var token = _jwtService.GenerateToken(userEntity);

                return new ApiResponse<LoginResponse>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Đăng nhập thành công",
                    Data = new LoginResponse
                    {
                        UserName = userDto.UserName,
                        Avatar = userDto.Employee?.Avatar,
                        FullName = userDto.Employee?.FullName,
                        Token = token,
                        Role = userDto.RoleName
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Login Feature]");
                throw;
            }
        }
    }
}
