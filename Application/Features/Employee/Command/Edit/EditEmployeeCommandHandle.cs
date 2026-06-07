using Application.IService;
using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Extension;
using Domain.Payload.Base;
using Domain.Share.Common;
using Infrastructure.GenericRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Employee.Command.Edit
{
    public class EditEmployeeCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                           ILogger<EditEmployeeCommandHandle> _logger,
                                           IQueueRepository _queueRepository) :
        IRequestHandler<EditEmployeeCommand, ApiResponse<string>>
    {
        /*
            1. Tìm nhân viên theo Id, nếu không thấy trả về lỗi NotFound
            2. Kiểm tra Phòng ban, Chức vụ, Kho (nếu có cập nhật) xem có tồn tại không
            3. Cập nhật các thông tin cá nhân và thông tin việc làm
            4. Gửi message xuống Queue để cập nhật database
            5. Trả về kết quả thành công
        */
        public async Task<ApiResponse<string>> Handle(EditEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _unitOfWork.GetRepository<Domain.Entities.Employee>()
                                                  .SingleOrDefaultAsync(
                                                    predicate: x => x.Id == request.Id,
                                                    include: x => x.Include(x => x.User)
                                                   );
                if (employee == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.NotFound,
                        Message = "Nhân viên không tồn tại",
                        Data = null
                    };
                }

                if (request.DepartmentId.HasValue)
                {
                    bool isDepartmentExists = await _unitOfWork
                        .GetRepository<Domain.Entities.Department>()
                        .AnyAsync(x => x.Id == request.DepartmentId.Value);

                    if (!isDepartmentExists)
                    {
                        return new ApiResponse<string> { StatusCode = StatusCode.NotFound, Message = "Phòng ban không tồn tại" };
                    }
                    employee.DepartmentId = request.DepartmentId.Value;
                }

                if (request.PositionId.HasValue)
                {
                    bool isPositionExists = await _unitOfWork
                        .GetRepository<Position>()
                        .AnyAsync(x => x.Id == request.PositionId.Value);

                    if (!isPositionExists)
                    {
                        return new ApiResponse<string> { StatusCode = StatusCode.NotFound, Message = "Chức vụ không tồn tại" };
                    }
                    employee.PositionId = request.PositionId.Value;
                }



                employee.FullName = request.FullName ?? employee.FullName;
                employee.Gender = request.Gender.HasValue ? request.Gender.Value.ToString() : employee.Gender;
                employee.Phone = request.Phone ?? employee.Phone;
                employee.Email = request.Email ?? employee.Email;
                employee.Address = request.Address ?? employee.Address;
                employee.DBO = request.DBO ?? employee.DBO;
                employee.Avatar = request.Avatar ?? employee.Avatar;
                employee.Status = request.Status.GetEnumDescription() ?? employee.Status;
                employee.Bank = request.Bank ?? employee.Bank;
                employee.BankNo = request.BankNo ?? employee.BankNo;
                employee.BankAccountHolder = request.BankAccountHolder ?? employee.BankAccountHolder;
                employee.LastModifiedDate = DateTime.UtcNow;
                employee.User.RoleId = request.RoleId ?? employee.User.RoleId;
                _unitOfWork.GetRepository<Domain.Entities.Employee>().Update(employee);
                _unitOfWork.GetRepository<Domain.Entities.User>().Update(employee.User);

                await _unitOfWork.CommitAsync();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Cập nhật nhân viên thành công",
                    Data = null!
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Edit Employee] {Message}", ex.Message);
                throw;
            }
        }
    }
}
