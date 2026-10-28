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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Application.Features.Auth.Command.ChangePassword
{
    public class ChangePasswordCommandHandller(IUnitOfWork _unitOfWork,
                                               GenericCacheInvalidator<User> _userCache,
                                               ILogger<ChangePasswordCommandHandller> _logger,
                                               IJWTService _jwtService
                                               )
        : IRequestHandler<ChangePasswordCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>()
                                            .SingleOrDefaultAsync(predicate: x => x.Id == Guid.Parse(_jwtService.GetUser()));
                if (user == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Không có dữ liệu người dùng",
                        Data = null
                    };
                }

                else if (user.Password != PasswordUtil.HashPassword(request.OldPassword))
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Mật khẩu cũ không chính xác",
                        Data = null
                    };
                }

                user.Password = PasswordUtil.HashPassword(request.NewPassword);
                user.LastModifiedBy = _jwtService.GetUser();
                user.LastModifiedDate = DateTime.Now;

                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.CommitAsync();

                _userCache.InvalidateEntity(user.Id);
                if (!string.IsNullOrWhiteSpace(user.UserName))
                {
                    _userCache.InvalidateEntity(user.UserName);
                }
                _userCache.InvalidateEntityList();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Thay đổi mật khẩu thành công",
                    Data = null
                };
                
            }
            catch (Exception ex)
            {
                _logger.LogError("[Đã có lỗi xảy ra khi đổi mật khẩu]" + ex.InnerException);
                throw new Exception("Đã có lỗi xảy ra khi thay đổi mật khẩu");
            }
        }
    }
}
