using RedisService.IService;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedisService.Service
{
    public class RedisServices : IRedisService
    {
        private readonly IDatabase _db;

        public RedisServices(IConnectionMultiplexer conn)
        {
            _db = conn.GetDatabase();
        }

        // =================
        // Base methods
        // =================
        public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _db.StringGetAsync(key);
            if (json.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        // =================
        // Overload Enum
        // =================
        public async Task SetAsync<T>(Enum cacheKey, T value, TimeSpan expiry)
        {
            string key = cacheKey.ToString().ToLower();
            await SetAsync(key, value, expiry);
        }

        public async Task<T?> GetAsync<T>(Enum cacheKey)
        {
            string key = cacheKey.ToString().ToLower();
            return await GetAsync<T>(key);
        }

        public async Task RemoveAsync(Enum cacheKey)
        {
            string key = cacheKey.ToString().ToLower();
            await RemoveAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            var endpoints = _db.Multiplexer.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _db.Multiplexer.GetServer(endpoint);
                var keys = server.Keys(pattern: prefix + "*");
                foreach (var key in keys)
                {
                    await _db.KeyDeleteAsync(key);
                }
            }
        }
    }
}
