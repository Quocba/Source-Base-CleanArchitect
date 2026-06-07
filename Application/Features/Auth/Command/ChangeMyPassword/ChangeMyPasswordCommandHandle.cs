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

namespace Application.Features.Auth.Command.ChangeMyPassword
{
    public class ChangeMyPasswordCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                               IQueueRepository _queueRepository,
                                               IJWTService _jwtService,
                                               ILogger<ChangeMyPasswordCommandHandle> _logger)
        : IRequestHandler<ChangeMyPasswordCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(ChangeMyPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userIdStr = _jwtService.GetUser();
                if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid employeeId))
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.Unauthorized,
                        Message = "Không tìm thấy thông tin người dùng",
                        Data = string.Empty
                    };
                }

                var employee = await _unitOfWork.GetRepository<Domain.Entities.Employee>()
                     .SingleOrDefaultAsync(
                         predicate: x => x.Id == employeeId && x.IsDeleted == false,
                         include: x => x.Include(u => u.User)
                     );

                if (employee == null || employee.User == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.NotFound,
                        Message = "Không tìm thấy tài khoản",
                        Data = string.Empty
                    };
                }

                if (!PasswordUtil.HashPassword(request.OldPassword).Equals(employee.User.Password))
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Mật khẩu cũ không chính xác",
                        Data = string.Empty
                    };
                }

                employee.User.Password = PasswordUtil.HashPassword(request.NewPassword);
                employee.User.LastModifiedDate = DateTime.Now;

                _unitOfWork.GetRepository<Domain.Entities.User>().Update(employee.User);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Đổi mật khẩu thành công",
                    Data = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi mật khẩu cá nhân: {Message}", ex.Message);
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
