using Application.IService;
using Application.Payload.Response.Employee;
using Domain.Entities.Enum;
using Domain.Payload.Base;
using Domain.Share.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Queries.GetMe
{
    public class GetMeQueryHandle(IUnitOfWork.IUnitOfWork _unitOfWork,
                                  GenericCacheInvalidator<Domain.Entities.Employee> _employeeCache,
                                  IMemoryCache _cache,
                                  IJWTService _jwtService)
        : IRequestHandler<GetMeQuery, ApiResponse<GetMeResponse>>
    {
        public async Task<ApiResponse<GetMeResponse>> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            Guid employeeId = Guid.Parse(_jwtService.GetUser());
            var param = new ListParameters<Domain.Entities.Employee>(employeeId);
            var cacheKey = _employeeCache.GetCacheKeyForList(param);

            if (_cache.TryGetValue(cacheKey, out ApiResponse<GetMeResponse> response))
            {
                return response;
            }

            var employee = await _unitOfWork.Context.Set<Domain.Entities.Employee>()
                                            .Where(x => x.Id == employeeId)
                                            .Select(x => new GetMeResponse
                                            {
                                                Id = x.Id,
                                                FullName = x.FullName!,
                                                Email = x.Email!,
                                                Gender = x.Gender == "Male" ? GenderEnum.Male : (x.Gender == "Female" ? GenderEnum.Female : GenderEnum.Other),
                                                Phone = x.Phone!,
                                                Address = x.Address!,
                                                Avatar = x.Avatar!,
                                                BankNo = x.BankNo!,
                                                Bank = x.Bank!,
                                                BankAccountHolder = x.BankAccountHolder!,
                                                DBO = x.DBO ?? DateTime.MinValue
                                            })
                                            .SingleOrDefaultAsync(cancellationToken);

            if (employee == null)
            {
                return new ApiResponse<GetMeResponse>
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = "Không có dữ liệu",
                    Data = null
                };
            }

            var result = new ApiResponse<GetMeResponse>
            {
                StatusCode = StatusCode.OK,
                Message = "Lấy thông tin thành công",
                Data = employee
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            _cache.Set(cacheKey, result, options);
            _employeeCache.AddToListCacheKeys(cacheKey);

            return result;
        }
    }
}
