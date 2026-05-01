using Domain.Share.Util;
using Microsoft.Extensions.Configuration;

namespace Domain.KeyHandle
{
    public class ApiKeyValidator : IApiKeyValidator
    {
        private readonly string _md5KeyFromConfig;

        public ApiKeyValidator(IConfiguration configuration)
        {
            _md5KeyFromConfig = configuration["ApiSettings:ApiKey"];
        }



        public bool IsValid(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            var hashedKey = PasswordUtil.ToMD5(key);

            return string.Equals(hashedKey, _md5KeyFromConfig, StringComparison.OrdinalIgnoreCase);
        }
    }
}
