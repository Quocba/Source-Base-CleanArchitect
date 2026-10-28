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

namespace Application.Features.Users.Command.DeleteUser
{
    public class DeleteUserCommandHandler(IUnitOfWork _unitOfWork,
                                          IJWTService _jwtService,
                                          ILogger<DeleteUserCommandHandler> _logger,
                                          GenericCacheInvalidator<User> _userCache)
        : IRequestHandler<DeleteUserCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _unitOfWork.GetRepository<User>()
                    .SingleOrDefaultAsync(predicate: x => x.Id == request.UserId && !x.IsDeleted);

                if (user == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new ApiResponse<string>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = "NgÆ°á»i dÃ¹ng khÃ´ng tá»“n táº¡i trÃªn há»‡ thá»‘ng",
                        Data = null
                    };
                }

                user.IsDeleted = true;
                user.LastModifiedBy = _jwtService.GetUser();
                user.LastModifiedDate = DateTime.Now;
                _unitOfWork.GetRepository<User>().Update(user);

                await _unitOfWork.CommitTransactionAsync();

                _userCache.InvalidateEntity(user.Id);
                if (!string.IsNullOrWhiteSpace(user.UserName))
                {
                    _userCache.InvalidateEntity(user.UserName);
                }
                _userCache.InvalidateEntityList();

                return new ApiResponse<string>
                {
                    StatusCode = StatusCode.OK,
                    Message = "XÃ³a ngÆ°á»i dÃ¹ng thÃ nh cÃ´ng",
                    Data = user.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "[DeleteUserCommandHandler] ÄÃ£ cÃ³ lá»—i xáº£y ra khi xÃ³a ngÆ°á»i dÃ¹ng");
                throw new Exception("ÄÃ£ cÃ³ lá»—i xáº£y ra khi xÃ³a ngÆ°á»i dÃ¹ng", ex);
            }
        }
    }
}
