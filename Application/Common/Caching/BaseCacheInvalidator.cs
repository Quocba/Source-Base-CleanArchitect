using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Common.Caching
{
    public abstract class BaseCacheInvalidator<TEntity> : ICacheInvalidator<TEntity>
    {
        private readonly IMemoryCache _cache;
        protected readonly TimeSpan _defaultExpiration;
        private readonly string _listCacheKeysSetKey;

        protected BaseCacheInvalidator(IMemoryCache cache, TimeSpan? defaultExpiration = null)
        {
            _cache = cache;
            _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(30);
            _listCacheKeysSetKey = $"{typeof(TEntity).Name}_ListCacheKeys";
        }

        protected abstract string GetEntityCacheKey(Guid entityId);

        public virtual string GetEntityCacheKey(string key)
        {
            return $"{typeof(TEntity).Name}_{key}";
        }

        public void InvalidateEntity(Guid entityId)
        {
            _cache.Remove(GetEntityCacheKey(entityId));
        }

        public void InvalidateEntity(string key)
        {
            _cache.Remove(GetEntityCacheKey(key));
        }

        public void InvalidateEntityList()
        {
            if (_cache.TryGetValue(_listCacheKeysSetKey, out HashSet<string>? cacheKeys) && cacheKeys != null)
            {
                foreach (var key in cacheKeys)
                {
                    _cache.Remove(key);
                }
                _cache.Remove(_listCacheKeysSetKey);
            }
        }

        public void SetEntityCache(Guid entityId, object data, TimeSpan? absoluteExpire = null)
        {
            var key = GetEntityCacheKey(entityId);
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpire ?? _defaultExpiration
            };
            _cache.Set(key, data, options);
        }

        public void SetEntityCache(string key, object data, TimeSpan? absoluteExpire = null)
        {
            var cacheKey = GetEntityCacheKey(key);
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpire ?? _defaultExpiration
            };
            _cache.Set(cacheKey, data, options);
        }

        public bool TryGetEntityCache<T>(Guid entityId, out T? value)
        {
            var key = GetEntityCacheKey(entityId);
            if (_cache.TryGetValue(key, out value))
            {
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetEntityCache<T>(string key, out T? value)
        {
            var cacheKey = GetEntityCacheKey(key);
            if (_cache.TryGetValue(cacheKey, out value))
            {
                return true;
            }
            value = default;
            return false;
        }

        public T? GetEntityCache<T>(Guid entityId)
        {
            if (TryGetEntityCache(entityId, out T? value))
            {
                return value;
            }
            return default;
        }

        public T? GetEntityCache<T>(string key)
        {
            if (TryGetEntityCache(key, out T? value))
            {
                return value;
            }
            return default;
        }

        public virtual string GetCacheKey(string paramName, object? paramValue)
        {
            var filterValue = paramValue?.ToString()?.Replace(" ", "_") ?? "Null";
            return $"{typeof(TEntity).Name}_{paramName}_{filterValue}";
        }

        public virtual string GetCacheKeyForSingle(string paramName, object? paramValue)
        {
            return GetCacheKey(paramName, paramValue);
        }

        public virtual string GetCacheKeyForSingle(object? paramValue)
        {
            return GetCacheKey(paramValue);
        }

        public virtual string GetCacheKey(object? parameters)
        {
            var key = $"{typeof(TEntity).Name}_List_";

            if (parameters == null)
                return key + "Default";

            // Xử lý trực tiếp các kiểu dữ liệu đơn (string, Guid, số, primitive)
            if (parameters is string or ValueType)
            {
                var valStr = parameters.ToString()?.Replace(" ", "_") ?? "Null";
                return $"{typeof(TEntity).Name}_{valStr}";
            }

            var paramType = parameters.GetType();

            // Xử lý SingleParameter<TEntity>
            if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(SingleParameter<>))
            {
                dynamic singleParam = parameters;
                var singleVal = singleParam.Value?.ToString()?.Replace(" ", "_") ?? "Null";
                return $"{typeof(TEntity).Name}_{singleParam.Key}_{singleVal}";
            }

            // Xử lý ListParameters<TEntity>
            if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(ListParameters<>))
            {
                dynamic dynamicParams = parameters;

                // Nếu là parameter đơn không phân trang
                if (dynamicParams.PageNumber == 0 && dynamicParams.PageSize == 0 && dynamicParams.Filters.Count == 1)
                {
                    foreach (var filter in dynamicParams.Filters)
                    {
                        var singleVal = filter.Value?.ToString()?.Replace(" ", "_") ?? "Null";
                        return $"{typeof(TEntity).Name}_{filter.Key}_{singleVal}";
                    }
                }

                key += $"Page_{dynamicParams.PageNumber}_Size_{dynamicParams.PageSize}";

                foreach (var filter in dynamicParams.Filters)
                {
                    var filterValue = filter.Value?.ToString()?.Replace(" ", "_") ?? "Null";
                    key += $"_{filter.Key}_{filterValue}";
                }

                return key;
            }

            // Fallback: Reflection properties
            var properties = paramType.GetProperties();
            if (properties.Length == 0)
            {
                return $"{typeof(TEntity).Name}_{parameters}";
            }

            var paramString = string.Join("_", properties
                .Select(p => $"{p.Name}_{p.GetValue(parameters)}"));

            return key + paramString;
        }

        public void AddToListCacheKeys(string cacheKey)
        {
            var cacheKeys = _cache.Get<HashSet<string>>(_listCacheKeysSetKey) ?? new HashSet<string>();
            cacheKeys.Add(cacheKey);
            _cache.Set(_listCacheKeysSetKey, cacheKeys, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _defaultExpiration
            });
        }
    }
}
