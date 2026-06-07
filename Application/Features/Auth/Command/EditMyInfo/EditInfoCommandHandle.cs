using Application.IService;
using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Command.EditMyInfo
{
    public class EditInfoCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                       ILogger<EditInfoCommand> _logger,
                                       IJWTService _jwtService,
                                       GenericCacheInvalidator<Domain.Entities.Employee> _employeeCache,
                                       IMemoryCache _cache)
        : IRequestHandler<EditInfoCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(EditInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Guid employeeId = Guid.Parse(_jwtService.GetUser());
                if (employeeId == Guid.Empty)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Không có thông tin",
                        Data = null
                    };
                }

                var employee = await _unitOfWork.GetRepository<Domain.Entities.Employee>()
                    .SingleOrDefaultAsync(predicate: x => x.Id == employeeId);

                if (employee == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.NotFound,
                        Message = "Không tìm thấy thông tin nhân viên",
                        Data = null
                    };
                }

                var isConflict = await _unitOfWork.GetRepository<Domain.Entities.Employee>()
                    .AnyAsync(predicate: x => x.Id != employeeId && x.IsDeleted == false && (
                        (!string.IsNullOrEmpty(request.Email) && x.Email == request.Email) ||
                        (!string.IsNullOrEmpty(request.Phone) && x.Phone == request.Phone) ||
                        (!string.IsNullOrEmpty(request.Bank) && !string.IsNullOrEmpty(request.BankNo) && x.Bank == request.Bank && x.BankNo == request.BankNo)
                    ));

                if (isConflict)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.Conflict,
                        Message = "Thông tin Email, Số điện thoại hoặc Tài khoản ngân hàng đã tồn tại",
                        Data = null
                    };
                }

                employee.FullName = request.FullName ?? employee.FullName;
                employee.Gender = request.Gender.HasValue ? request.Gender.ToString() : employee.Gender;
                employee.Phone = request.Phone ?? employee.Phone;
                employee.Email = request.Email ?? employee.Email;
                employee.Address = request.Address ?? employee.Address;
                employee.Avatar = request.Avatar ?? employee.Avatar;
                employee.Bank = request.Bank ?? employee.Bank;
                employee.BankNo = request.BankNo ?? employee.BankNo;
                employee.BankAccountHolder = request.BankAccountHolder ?? employee.BankAccountHolder;
                employee.DBO = (request.DBO != DateTime.MinValue) ? request.DBO : employee.DBO;

                employee.LastModifiedDate = DateTime.Now;
                employee.LastModifiedBy = employeeId;

                _unitOfWork.GetRepository<Domain.Entities.Employee>().Update(employee);

                await _unitOfWork.CommitAsync();

                _employeeCache.InvalidateEntity(employeeId);
                _employeeCache.InvalidateEntityList();
                var paramsCache = new ListParameters<Domain.Entities.Employee>(employeeId);
                _cache.Remove(_employeeCache.GetCacheKeyForList(paramsCache));


                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Cập nhật thông tin thành công",
                    Data = employee.Id.ToString()
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin: {Message}", ex.ToString());
                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.InternalServerError,
                    Message = "Đã có lỗi xảy ra",
                    Data = null
                };
            }
        }
    }
}
