using Application.IService;
using Domain.Payload.Base;
using Domain.Share.Common;
using Domain.Share.Util;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Department.Command.Create
{
    public class CreateDepartmentCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                               ILogger<CreateDepartmentCommandHandle> _logger,
                                               IJWTService _jwtService,
                                               IGenerateCodeService generateCodeService)
        : IRequestHandler<CreateDepartmentCommand, ApiResponse<string>>
    {
        /*
            1. Kiểm tra phòng ban đã tồn tại hay chưa (theo Tên)
            2. Nếu đã tồn tại, trả về lỗi Conflict
            3. Sinh mã phòng ban (Code) tự động
            4. Tạo object Department mới
            5. Thêm vào database và lưu thay đổi
            6. Trả về kết quả thành công và mã phòng ban vừa tạo
        */
        public async Task<ApiResponse<string>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var isAlReady = await _unitOfWork.GetRepository<Domain.Entities.Department>()
                    .SingleOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsDeleted == false);

                if (isAlReady != null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.Conflict,
                        Message = "Phòng ban đã tồn tại",
                        Data = null
                    };
                }

                var department = new Domain.Entities.Department
                {
                    Id = Guid.NewGuid(),
                    Code = generateCodeService.GenerateDepartmentCode(request.Name, 1),
                    Name = request.Name,
                    Description = request.Description,
                    CreateDate = DateTime.UtcNow,
                    IsDeleted = false,
                    CreatedBy = Guid.Parse(_jwtService.GetUser()!),
                    LastModifiedDate = null,
                    LastModifiedBy = null
                };

                await _unitOfWork.GetRepository<Domain.Entities.Department>().AddAsync(department);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.Created,
                    Message = "Tạo phòng ban thành công",
                    Data = department.Code.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("[Create Department]" + ex.InnerException.Message);
                throw;
            }
        }
    }
}
