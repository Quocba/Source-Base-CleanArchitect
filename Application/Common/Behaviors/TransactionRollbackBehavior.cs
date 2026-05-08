using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Application.IUnitOfWork;

namespace Application.Common.Behaviors
{
    public class TransactionRollbackBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionRollbackBehavior(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var rollbackHeader = _httpContextAccessor.HttpContext?.Request.Headers["X-Rollback"].ToString();
            var requestName = typeof(TRequest).Name;

            // Chỉ thực hiện Rollback nếu:
            // 1. Có Header X-Rollback = true
            // 2. Tên Request chứa chữ "Command" (Thêm/Sửa/Xóa). Các Query (Lấy dữ liệu) thì bỏ qua để tránh lỗi Dapper.
            if (!string.IsNullOrEmpty(rollbackHeader) && 
                rollbackHeader.ToLower() == "true" && 
                requestName.Contains("Command"))
            {
                // Sử dụng TransactionScope để cả EF Core và Dapper cùng hiểu
                using (var scope = new System.Transactions.TransactionScope(
                    System.Transactions.TransactionScopeOption.Required,
                    new System.Transactions.TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                    System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var response = await next();
                        
                        // KHÔNG gọi scope.Complete() để thực hiện Rollback tự động khi kết thúc khối using
                        return response;
                    }
                    catch (Exception)
                    {
                        // Mặc định sẽ rollback nếu có exception
                        throw;
                    }
                }
            }

            return await next();
        }
    }
}
