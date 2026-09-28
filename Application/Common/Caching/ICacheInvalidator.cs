using Microsoft.Extensions.Primitives;
using System;

namespace Application.Common.Caching
{
    public interface ICacheInvalidator<TEntity>
    {
        void InvalidateEntity(Guid entityId);
        void InvalidateEntity(string key);
        void InvalidateEntityList();
        void SetEntityCache(Guid entityId, object data, TimeSpan? absoluteExpire = null);
        void SetEntityCache(string key, object data, TimeSpan? absoluteExpire = null);
        void SetListCache(string cacheKey, object data, TimeSpan? absoluteExpire = null);
        IChangeToken GetListChangeToken();
        T? GetEntityCache<T>(Guid entityId);
        T? GetEntityCache<T>(string key);
        bool TryGetListCache<T>(string cacheKey, out T? value);
        T? GetListCache<T>(string cacheKey);
        string GetCacheKeyForSingle(string paramName, object? paramValue);
        string GetCacheKeyForSingle(object? paramValue);
    }
}
