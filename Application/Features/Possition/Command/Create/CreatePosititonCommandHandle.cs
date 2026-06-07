using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Possition.Command.Create
{
    public class CreatePosititonCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                              ILogger<CreatePositionCommand> _logger) : IRequestHandler<CreatePositionCommand, ApiResponse<string>>
    {
        /*
            1. Kiểm tra chức vụ đã tồn tại (theo Tên)
            2. Nếu tồn tại, trả về lỗi Conflict
            3. Tạo object Position mới
            4. Thêm vào database và lưu thay đổi
            5. Trả về kết quả thành công
        */
        public async Task<ApiResponse<string>> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var isAlReady = await _unitOfWork.GetRepository<Domain.Entities.Position>()
                    .SingleOrDefaultAsync(
                    predicate: x => x.Name == request.Name && x.IsDeleted == false
                    );

                if (isAlReady != null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.Conflict,
                        Message = "Chức vụ đã tồn tại",
                        Data = null
                    };
                }

                var positon = new Domain.Entities.Position
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    IsDeleted = false
                };

                await _unitOfWork.GetRepository<Domain.Entities.Position>().AddAsync(positon);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.Created,
                    Message = "Tạo chức vụ thành công",
                    Data = null
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.Message, "[Create Positon]");
                throw;
            }
        }
    }
}
