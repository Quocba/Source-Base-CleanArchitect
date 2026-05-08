using Application.IService;
using Domain.Payload.Base;
using Domain.Share.Common;
using Domain.Share.Util;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Auth.Command.ChangePassword
{
    public class ChangePasswordCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                             ILogger<ChangePasswordCommandHandle> _logger)
        : IRequestHandler<ChangePasswordCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!request.NewPassword.Equals(request.ConfirmNewPassword))
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Mật khẩu xác nhận không khớp"
                    };
                }

                var user = await _unitOfWork.GetRepository<Domain.Entities.User>()
                     .SingleOrDefaultAsync(predicate: x => x.Id == request.UserId && x.IsDeleted == false);

                if (user == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.NotFound,
                        Message = "Tài khoản không tồn tại"
                    };
                }

                user.Password = PasswordUtil.HashPassword(request.NewPassword);
                user.LastModifiedDate = DateTime.Now;

                await _unitOfWork.CommitAsync();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Đổi mật khẩu thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi mật khẩu cho User {UserId}", request.UserId);
                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.InternalServerError,
                    Message = "Có lỗi xảy ra trong quá trình xử lý"
                };
            }
        }
    }
}
