using Application.IService;
using Domain.Entities.Base;
using Domain.Entities.Enum;
using Domain.Extension;
using Domain.Payload.Base;
using Domain.Share.Common;
using Infrastructure.GenericRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Command.LockAndUnlock
{
    public class LockAndUnLockCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                            ILogger<LockAndUnLockCommandHandle> _logger)
        : IRequestHandler<LockAndUnLockCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(LockAndUnLockCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _unitOfWork.GetRepository<Domain.Entities.Employee>()
                    .FirstOrDefaultAsync(
                        predicate: x => x.Id == request.EmployeeId && x.IsDeleted == false,
                        include: x => x.Include(u => u.User)
                    );

                if (employee == null || employee.User == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.NotFound,
                        Message = "Nhân viên không tồn tại hoặc đã bị xóa",
                        Data = string.Empty
                    };
                }

                bool isCurrentlyLocked = employee.User.IsLock ?? false;
                employee.User.IsLock = !isCurrentlyLocked;
                
                // Gán trực tiếp chuỗi để tránh Reflection từ GetEnumDescription()
                employee.Status = employee.User.IsLock.Value ? "Ngừng Hoạt Động" : "Đang hoạt động";

                // EF Core tự động batch các thay đổi vào 1 lần gọi DB duy nhất
                await _unitOfWork.CommitAsync();

                string statusMessage = employee.User.IsLock.Value
                                       ? "Đã khóa tài khoản thành công"
                                       : "Đã mở khóa tài khoản thành công";

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = statusMessage,
                    Data = string.Empty
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi Lock/Unlock User: {Message}", ex.Message);
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
