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

namespace Application.Features.Users.Command.EditUser
{
    public class EditUserCommandHandler(IUnitOfWork _unitOfWork,
                                        IJWTService _jwtService,
                                        ILogger<EditUserCommandHandler> _logger,
                                        GenericCacheInvalidator<User> _userCache)
        : IRequestHandler<EditUserCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>()
                    .SingleOrDefaultAsync(predicate: x => x.Id == request.UserId && !x.IsDeleted);

                if (user == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Người dùng không tồn tại trên hệ thống",
                        Data = null
                    };
                }

                var trimmedUserName = request.UserName?.Trim();
                var trimmedEmail = request.Email?.Trim();
                var trimmedCode = request.Code?.Trim();

                var isDuplicate = await _unitOfWork.GetRepository<User>()
                    .SingleOrDefaultAsync(predicate: x => x.Id != request.UserId && !x.IsDeleted &&
                        ((trimmedUserName != null && x.UserName == trimmedUserName) ||
                         (trimmedEmail != null && x.Email == trimmedEmail) ||
                         (trimmedCode != null && x.Code == trimmedCode)));

                if (isDuplicate != null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Mã người dùng, tài khoản hoặc email đã được sử dụng bởi người dùng khác",
                        Data = null
                    };
                }

                var oldUserName = user.UserName;

                if (!string.IsNullOrWhiteSpace(trimmedCode)) user.Code = trimmedCode;
                if (!string.IsNullOrWhiteSpace(trimmedUserName)) user.UserName = trimmedUserName;
                if (!string.IsNullOrWhiteSpace(trimmedEmail)) user.Email = trimmedEmail;
                if (request.Phone != null) user.Phone = request.Phone.Trim();
                if (request.Avatar != null) user.Avatar = request.Avatar;
                user.Role = user.Role;
                if (!string.IsNullOrWhiteSpace(request.Password)) user.Password = PasswordUtil.HashPassword(request.Password);
                if (request.StorageCapacity.HasValue && request.StorageCapacity.Value > 0) user.StorageCapacity = request.StorageCapacity.Value;

                user.LastModifiedBy = _jwtService.GetUser();
                user.LastModifiedDate = DateTime.Now;

                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.CommitAsync();

                _userCache.InvalidateEntity(user.Id);
                if (!string.IsNullOrWhiteSpace(oldUserName))
                {
                    _userCache.InvalidateEntity(oldUserName);
                }
                if (!string.IsNullOrWhiteSpace(user.UserName) && user.UserName != oldUserName)
                {
                    _userCache.InvalidateEntity(user.UserName);
                }
                _userCache.InvalidateEntityList();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Cập nhật thông tin người dùng thành công",
                    Data = user.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EditUserCommandHandler] Đã có lỗi xảy ra khi cập nhật thông tin người dùng");
                throw new Exception("Đã có lỗi xảy ra khi cập nhật thông tin người dùng", ex);
            }
        }
    }
}
