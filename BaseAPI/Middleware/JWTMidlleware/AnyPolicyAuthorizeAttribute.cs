using Microsoft.AspNetCore.Authorization;

namespace BaseAPI.Middleware.JWTMidlleware
{
    public class AnyPolicyAuthorizeAttribute : AuthorizeAttribute
    {
        public AnyPolicyAuthorizeAttribute(params string[] policies)
        {
            Policy = string.Join("|", policies); // nối bằng dấu | để biểu thị OR
        }
    }
}
