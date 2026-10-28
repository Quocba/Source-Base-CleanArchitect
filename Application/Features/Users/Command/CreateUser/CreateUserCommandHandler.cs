using Application.Common.Caching;
using Application.Common.Share.Common;
using Application.Common.Util;
using Application.Interfaces;
using Application.IService;
using Domain.Entities;
using Domain.Payload.Base;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Command.CreateUser
{
    public class CreateUserCommandHandler(IUnitOfWork _unitOfWork,
                                          IJWTService _jwtService,
                                          IGenerateCodeService _generateCodeService,
                                          ILogger<CreateUserCommandHandler> _logger,
                                          GenericCacheInvalidator<User> _userCache)
        : IRequestHandler<CreateUserCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserName) ||
                    string.IsNullOrWhiteSpace(request.Password) ||
                    string.IsNullOrWhiteSpace(request.Email))
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Tài khoản, mật khẩu và email không được để trống",
                        Data = null
                    };
                }

                var trimmedUserName = request.UserName.Trim();
                var trimmedEmail = request.Email.Trim();

                var isExist = await _unitOfWork.GetRepository<User>()
                    .SingleOrDefaultAsync(predicate: x => (x.UserName == trimmedUserName || x.Email == trimmedEmail) && !x.IsDeleted);

                if (isExist != null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Tài khoản hoặc email đã tồn tại trên hệ thống",
                        Data = null
                    };
                }

                var userCode = _generateCodeService.GenerateEmployeeCode();
                while (await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.Code == userCode && !x.IsDeleted) != null)
                {
                    userCode = _generateCodeService.GenerateEmployeeCode();
                }

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Code = userCode,
                    UserName = trimmedUserName,
                    Password = PasswordUtil.HashPassword(request.Password),
                    Email = trimmedEmail,
                    Phone = request.Phone?.Trim() ?? string.Empty,
                    Avatar = request.Avatar ?? string.Empty,
                    Role = request.Role,
                    IsLock = false,
                    IsDeleted = false,
                    StorageCapacity = request.StorageCapacity.HasValue && request.StorageCapacity.Value > 0
                        ? request.StorageCapacity.Value
                        : 5368709120L,
                    CreatedBy = _jwtService.GetUser(),
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = _jwtService.GetUser(),
                    LastModifiedDate = DateTime.Now
                };

                await _unitOfWork.GetRepository<User>().AddAsync(newUser);
                await _unitOfWork.CommitAsync();

                _userCache.InvalidateEntityList();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.Created,
                    Message = "Tạo người dùng mới thành công",
                    Data = newUser.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateUserCommandHandler] Đã có lỗi xảy ra khi tạo người dùng mới");
                throw new Exception("Đã có lỗi xảy ra khi tạo người dùng mới", ex);
            }
        }
    }
}
