using System;
using System.Threading.Tasks;

namespace RedisService.IService
{
    public interface IRedisService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiry);
        Task RemoveAsync(string key);

        Task<T?> GetAsync<T>(Enum cacheKey);
        Task SetAsync<T>(Enum cacheKey, T value, TimeSpan expiry);
        Task RemoveAsync(Enum cacheKey);
        Task RemoveByPrefixAsync(string prefix);
    }
}
