using Application.IService;
using Domain.Entities.Enum;
using Domain.Extension;
using Domain.Payload.Base;
using Domain.Share.Common;
using Domain.Share.Util;
using Infrastructure.GenericRepository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employee.Command.Create
{
    public class CreateEmployeeCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                            ILogger<CreateEmployeeCommandHandle> _logger,
                                            IJWTService _jwtService,
                                            IGenerateCodeService generateCodeService,
                                            IQueueRepository _queueRepository)
        : IRequestHandler<CreateEmployeeCommand, ApiResponse<string>>
    {
        /*
            1. Validate: Kiểm tra trùng tên đăng nhập, email, số điện thoại
            2. Nếu có trùng lặp trả về lỗi Conflict tương ứng
            3. Tạo User mới (hash password, set các giá trị mặc định)
            4. Tạo Employee mới, liên kết với User vừa tạo
            5. Sinh mã nhân viên tự động
            6. Gửi message xuống Queue để thêm User và Employee vào database
            7. Trả về kết quả thành công
        */
        public async Task<ApiResponse<string>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {

                bool isUserNameExists = await _unitOfWork.GetRepository<Domain.Entities.User>()
                    .AnyAsync(x => x.UserName == request.UserName);

                if (isUserNameExists)
                {
                    return new ApiResponse<string> { StatusCode = StatusCode.Conflict, Message = "Tên đăng nhập đã tồn tại" };
                }

                bool isEmailExists = await _unitOfWork.GetRepository<Domain.Entities.Employee>()
                    .AnyAsync(x => x.Email == request.Email);

                if (isEmailExists)
                {
                    return new ApiResponse<string> { StatusCode = StatusCode.Conflict, Message = "Email nhân viên đã tồn tại" };
                }

                bool isPhoneExists = await _unitOfWork.GetRepository<Domain.Entities.Employee>()
                    .AnyAsync(x => x.Phone == request.Phone);

                if (isPhoneExists)
                {
                    return new ApiResponse<string> { StatusCode = StatusCode.Conflict, Message = "Số điện thoại nhân viên đã tồn tại" };
                }



                var newUser = new Domain.Entities.User
                {
                    Id = Guid.NewGuid(),
                    UserName = request.UserName,
                    Password = PasswordUtil.HashPassword(request.Password),
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    IsLock = false,
                    RoleId = request.RoleId,
                    IsUseSoftWare = request.IsUseSoftWare
                };

                var newEmployee = new Domain.Entities.Employee
                {
                    Id = Guid.NewGuid(),
                    Code = generateCodeService.GenerateEmployeeCode(),
                    UserId = newUser.Id,
                    Avatar = request.Avatar,
                    Gender = request.Gender.ToString(),
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Address = request.Address,
                    DBO = request.DBO,
                    Bank = request.Bank,
                    BankNo = request.BankNo,
                    BankAccountHolder = request.BankAccountHolder,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Guid.Parse(_jwtService.GetUser()!),
                    IsDeleted = false,
                    Status = EmployeeStatusEnum.Active.GetEnumDescription(),

                    PositionId = request.PositionId,
                    DepartmentId = request.DepartmentId,
                };

                await _unitOfWork.GetRepository<Domain.Entities.User>().AddAsync(newUser);
                await _unitOfWork.GetRepository<Domain.Entities.Employee>().AddAsync(newEmployee);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.Created,
                    Message = "Tạo mới nhân viên thành công",
                    Data = newEmployee.Id.ToString()
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo nhân viên: {Message}", ex.Message);
                throw;
            }
        }
    }
}
