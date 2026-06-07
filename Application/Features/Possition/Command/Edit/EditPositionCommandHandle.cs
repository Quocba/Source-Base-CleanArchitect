using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Possition.Command.Edit
{
    public class EditPositionCommandHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                           ILogger<EditPositionCommandHandle> _logger) : IRequestHandler<EditPositionCommand, ApiResponse<string>>
    {
        /*
            1. Tìm chức vụ theo Id, kiểm tra xem có tồn tại hay đã bị xóa không
            2. Nếu tìm thấy, kiểm tra xem tên mới có trùng với chức vụ khác không
            3. Nếu trùng, trả về lỗi BadRequest
            4. Cập nhật thông tin (Tên, Mô tả)
            5. Lưu thay đổi vào database
            6. Trả về kết quả thành công
        */
        public async Task<ApiResponse<string>> Handle(EditPositionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var position = await _unitOfWork.GetRepository<Domain.Entities.Position>()
                                                .SingleOrDefaultAsync(
                                                 predicate: x => x.Id == request.Id && x.IsDeleted == false
                                                );
                if (position == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Chức vụ không tồn tại hoặc đã bị xóa trước đó",
                        Data = null
                    };
                }

                bool isAlreadyExists = await _unitOfWork.GetRepository<Domain.Entities.Position>()
                                                .AnyAsync(predicate: x => x.Name == request.Name && x.Id != request.Id && x.IsDeleted == false);
                if (isAlreadyExists)
                {
                    return new ApiResponse<string> { StatusCode = StatusCode.BadRequest, Message = "Chức vụ đã tồn tại" };
                }

                position.Name = request.Name;
                position.Description = request.Description;

                _unitOfWork.GetRepository<Domain.Entities.Position>().Update(position);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = "Cập nhật chức vụ thành công",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("[Edit Position]" + ex.InnerException.Message);
                throw;
            }
        }
    }
}
