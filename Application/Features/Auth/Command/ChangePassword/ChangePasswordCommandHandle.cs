using Application.IService;
using Domain.Payload.Base;
using Domain.Share.Common;
using Domain.Share.Util;
using Infrastructure.GenericRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Command.ChangePassword
{
    public class ChangePasswordCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                             IQueueRepository _queueRepository,
                                             ILogger<ChangePasswordCommandHandle> _logger)
        : IRequestHandler<ChangePasswordCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Validate Password Validation
                if (!request.NewPassword.Equals(request.ConfirmNewPassword))
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Mật khẩu xác nhận không khớp",
                        Data = string.Empty
                    };
                }

                // 2. Determine UserId from EmployeeId
                var employee = await _unitOfWork.GetRepository<Domain.Entities.Employee>()
                     .SingleOrDefaultAsync(
                         predicate: x => x.Id == request.EmployeeId && x.IsDeleted == false,
                         include: x => x.Include(u => u.User)
                     );

                if (employee == null || employee.User == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.NotFound,
                        Message = "Nhân viên hoặc tài khoản không tồn tại",
                        Data = string.Empty
                    };
                }

                // 3. Hash Password & Update
                employee.User.Password = PasswordUtil.HashPassword(request.NewPassword);
                employee.User.LastModifiedDate = DateTime.Now;

                // 4. Save Changes
                await _queueRepository.EnqueueUpdateAsync(employee.User);

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Đổi mật khẩu thành công",
                    Data = string.Empty
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi mật khẩu cho nhân viên {EmployeeId}: {Message}", request.EmployeeId, ex.Message);
                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.InternalServerError,
                    Message = "Có lỗi xảy ra trong quá trình xử lý",
                    Data = string.Empty
                };
            }
        }
    }
}
