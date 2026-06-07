using Application.IService;
using Azure.Core;
using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.Features.Department.Command.Edit
{
    public class EditDepartmentCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                             ILogger<EditDepartmentCommandHandle> _logger,
                                             IJWTService _jwtService) :
        IRequestHandler<EditDepartmentCommand, ApiResponse<string>>
    {
        /*
            1. Tìm phòng ban theo Id, nếu không thấy hoặc đã xóa thì trả về lỗi NotFound
            2. Kiểm tra trùng tên với các phòng ban khác (trừ chính nó)
            3. Nếu trùng tên, trả về lỗi Conflict
            4. Cập nhật tên và mô tả mới
            5. Lưu thay đổi vào database
            6. Trả về kết quả thành công
        */
        public async Task<ApiResponse<string>> Handle(EditDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var department = await _unitOfWork.GetRepository<Domain.Entities.Department>()
                                                  .SingleOrDefaultAsync(

                                                    predicate: x => x.Id == request.Id &&
                                                                     x.IsDeleted == false
                                                   );
                if (department == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.NotFound,
                        Message = "Phòng ban không tồn tại hoặc đã bị xóa trước đó",
                        Data = null
                    };
                }

                bool isAlreadyExists = await _unitOfWork.GetRepository<Domain.Entities.Department>()
                                                   .AnyAsync(predicate: x => x.Name == request.Name &&
                                                                      x.Id != request.Id &&
                                                                      x.IsDeleted == false);

                if (isAlreadyExists)
                {
                    return new ApiResponse<string> { StatusCode = StatusCode.Conflict, Message = "Tên phòng ban đã tồn tại, vui lòng sử dụng tên khác" };
                }

                department.Name = request.Name ?? department.Name;
                department.Description = request.Description ?? department.Description;
                department.LastModifiedBy = Guid.Parse(_jwtService.GetUser()!);
                department.LastModifiedDate = DateTime.Now;
                _unitOfWork.GetRepository<Domain.Entities.Department>().Update(department);
                await _unitOfWork.CommitAsync();
                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Cập nhật phòng ban thành công",
                    Data = null!
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("[Edit Department]" + ex.InnerException?.Message);
                throw;
            }
        }
    }
}
