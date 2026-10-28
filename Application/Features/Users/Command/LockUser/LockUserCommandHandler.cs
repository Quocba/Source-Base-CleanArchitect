using Application.Common.Caching;
using Application.Common.Share.Common;
using Application.Interfaces;
using Application.IService;
using Domain.Entities;
using Domain.Payload.Base;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Command.LockUser
{
    public class LockUserCommandHandler(IUnitOfWork _unitOfWork,
                                        IJWTService _jwtService,
                                        ILogger<LockUserCommandHandler> _logger,
                                        GenericCacheInvalidator<User> _userCache)
        : IRequestHandler<LockUserCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>()
                    .SingleOrDefaultAsync(predicate: x => x.Id == request.UserId && !x.IsDeleted);

                if (user == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "Người dùng không tồn tại trên hệ thống",
                        Data = null
                    };
                }

                user.IsLock = request.IsLock ?? !user.IsLock;
                user.LastModifiedBy = _jwtService.GetUser();
                user.LastModifiedDate = DateTime.Now;

                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.CommitAsync();

                _userCache.InvalidateEntity(user.Id);
                if (!string.IsNullOrWhiteSpace(user.UserName))
                {
                    _userCache.InvalidateEntity(user.UserName);
                }
                _userCache.InvalidateEntityList();

                var message = user.IsLock ? "Khóa người dùng thành công" : "Mở khóa người dùng thành công";

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = message,
                    Data = user.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[LockUserCommandHandler] Đã có lỗi xảy ra khi cập nhật trạng thái khóa người dùng");
                throw new Exception("Đã có lỗi xảy ra khi cập nhật trạng thái khóa người dùng", ex);
            }
        }
    }
}
