using Application.Common.Caching;
using Application.Common.Share.Common;
using Application.Interfaces;
using Application.IService;
using Domain.Entities;
using Domain.Payload.Base;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Command.EditProfile
{
    public class EditProfileCommandHandler(IUnitOfWork _unitOfWork,
                                           IJWTService _jwtService,
                                           ILogger<EditProfileCommandHandler> _logger,
                                           GenericCacheInvalidator<User> _userCache)
        : IRequestHandler<EditProfileCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(EditProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userIdStr = _jwtService.GetUser();
                if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var currentUserId))
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.Unauthorized,
                        Message = "Không tìm thấy thông tin xác thực của người dùng",
                        Data = null
                    };
                }

                var user = await _unitOfWork.GetRepository<User>()
                    .SingleOrDefaultAsync(predicate: x => x.Id == currentUserId && !x.IsDeleted);

                if (user == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Người dùng không tồn tại trên hệ thống",
                        Data = null
                    };
                }

                if (user.IsLock)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Tài khoản của bạn đã bị khóa",
                        Data = null
                    };
                }

                var trimmedEmail = request.Email?.Trim();
                var trimmedPhone = request.Phone?.Trim();

                if (!string.IsNullOrWhiteSpace(trimmedEmail) && !string.Equals(trimmedEmail, user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var isDuplicate = await _unitOfWork.GetRepository<User>()
                        .SingleOrDefaultAsync(predicate: x => x.Id != currentUserId && !x.IsDeleted && x.Email == trimmedEmail);

                    if (isDuplicate != null)
                    {
                        return new ApiResponse<string>
                        {
                            StatusCode = StatusCode.BadRequest,
                            Message = "Email đã được sử dụng bởi người dùng khác",
                            Data = null
                        };
                    }

                    user.Email = trimmedEmail;
                }
                if (request.Phone != null) user.Phone = trimmedPhone ?? string.Empty;
                if (request.Avatar != null) user.Avatar = request.Avatar.Trim();

                user.LastModifiedBy = currentUserId.ToString();
                user.LastModifiedDate = DateTime.Now;

                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.CommitAsync();

                _userCache.InvalidateEntity(user.Id);
                _userCache.InvalidateEntity(user.Id.ToString());
                if (!string.IsNullOrWhiteSpace(user.UserName))
                {
                    _userCache.InvalidateEntity(user.UserName);
                }
                _userCache.InvalidateEntityList();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Cập nhật thông tin cá nhân thành công",
                    Data = user.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EditProfileCommandHandler] Đã có lỗi xảy ra khi cập nhật thông tin cá nhân");
                throw new Exception("Đã có lỗi xảy ra khi cập nhật thông tin cá nhân", ex);
            }
        }
    }
}
