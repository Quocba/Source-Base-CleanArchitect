using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Application.Common.Caching
{
    public abstract class BaseCacheInvalidator<TEntity> : ICacheInvalidator<TEntity>
    {
        private readonly IMemoryCache _cache;
        protected readonly TimeSpan _defaultExpiration;

        // Lớp 2: Quản lý CancellationTokenSource tĩnh theo Entity để hủy hàng loạt bằng CancellationChangeToken
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _entityCts = new();
        private static readonly object _ctsLock = new();

        // Lớp 3: Tracking tĩnh trong RAM bằng ConcurrentDictionary để không bị timeout ngầm và thread-safe tuyệt đối
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _trackedKeysPerEntity = new();

        protected BaseCacheInvalidator(IMemoryCache cache, TimeSpan? defaultExpiration = null)
        {
            _cache = cache;
            _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(30);
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

        /// <summary>
        /// Xóa triệt để danh sách cache của Entity theo kiến trúc 3 lớp:
        /// - Lớp 1: Quét MemoryCache.Keys (.NET 9) xóa mọi key có tiền tố Entity_
        /// - Lớp 2: Hủy CancellationTokenSource (CancellationChangeToken) để MemoryCache tự động thu hồi
        /// - Lớp 3: Duyệt ConcurrentDictionary tĩnh xóa toàn bộ tracked keys
        /// </summary>
        public void InvalidateEntityList()
        {
            // Lớp 1: Quét MemoryCache.Keys (.NET 9)
            PurgeKeysByPrefix();

            // Lớp 2: Kích hoạt hủy token
            CancelListChangeToken();

            // Lớp 3: Quét tracked keys tĩnh
            PurgeTrackedKeys();
        }

        #region Layer 1: MemoryCache.Keys Prefix Purge (.NET 9)

        private void PurgeKeysByPrefix()
        {
            try
            {
                if (_cache is MemoryCache memoryCache)
                {
                    var prefix = $"{typeof(TEntity).Name}_";
                    var exactName = typeof(TEntity).Name;

                    var keysToRemove = memoryCache.Keys
                        .OfType<string>()
                        .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(k, exactName, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    foreach (var key in keysToRemove)
                    {
                        _cache.Remove(key);
                    }
                }
            }
            catch (Exception)
            {
                // Fallback an toàn nếu thao tác duyệt keys gặp xung đột đồng thời
            }
        }

        #endregion

        #region Layer 2: CancellationChangeToken Pattern

        public IChangeToken GetListChangeToken()
        {
            var entityName = typeof(TEntity).Name;
            lock (_ctsLock)
            {
                var cts = _entityCts.GetOrAdd(entityName, _ => new CancellationTokenSource());
                return new CancellationChangeToken(cts.Token);
            }
        }

        private void CancelListChangeToken()
        {
            var entityName = typeof(TEntity).Name;
            CancellationTokenSource? oldCts = null;

            lock (_ctsLock)
            {
                if (_entityCts.TryGetValue(entityName, out oldCts))
                {
                    _entityCts[entityName] = new CancellationTokenSource();
                }
            }

            if (oldCts != null)
            {
                try
                {
                    if (!oldCts.IsCancellationRequested)
                    {
                        oldCts.Cancel();
                    }
                }
                catch (ObjectDisposedException) { }
                finally
                {
                    oldCts.Dispose();
                }
            }
        }

        #endregion

        #region Layer 3: Static ConcurrentDictionary Key Tracking

        public void AddToListCacheKeys(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey)) return;

            var entityName = typeof(TEntity).Name;
            var keys = _trackedKeysPerEntity.GetOrAdd(entityName, _ => new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase));
            keys.TryAdd(cacheKey, 0);
        }

        private void PurgeTrackedKeys()
        {
            var entityName = typeof(TEntity).Name;
            if (_trackedKeysPerEntity.TryRemove(entityName, out var keys))
            {
                foreach (var key in keys.Keys)
                {
                    _cache.Remove(key);
                }
            }
        }

        #endregion

        #region List Cache Helper Methods

        public void SetListCache(string cacheKey, object data, TimeSpan? absoluteExpire = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpire ?? TimeSpan.FromDays(1)
            };
            options.AddExpirationToken(GetListChangeToken());

            _cache.Set(cacheKey, data, options);
            AddToListCacheKeys(cacheKey);
        }

        public bool TryGetListCache<T>(string cacheKey, out T? value)
        {
            return _cache.TryGetValue(cacheKey, out value);
        }

        public T? GetListCache<T>(string cacheKey)
        {
            if (_cache.TryGetValue(cacheKey, out T? value))
            {
                return value;
            }
            return default;
        }

        #endregion

        #region Entity Cache Methods

        public void SetEntityCache(Guid entityId, object data, TimeSpan? absoluteExpire = null)
        {
            var key = GetEntityCacheKey(entityId);
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpire ?? _defaultExpiration
            };
            options.AddExpirationToken(GetListChangeToken());
            _cache.Set(key, data, options);
            AddToListCacheKeys(key);
        }

        public void SetEntityCache(string key, object data, TimeSpan? absoluteExpire = null)
        {
            var cacheKey = GetEntityCacheKey(key);
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpire ?? _defaultExpiration
            };
            options.AddExpirationToken(GetListChangeToken());
            _cache.Set(cacheKey, data, options);
            AddToListCacheKeys(cacheKey);
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

        #endregion

        #region Key Generation & Normalization

        public virtual string GetCacheKey(string paramName, object? paramValue)
        {
            if (paramValue == null || (paramValue is string s && string.IsNullOrWhiteSpace(s)))
            {
                return $"{typeof(TEntity).Name}_{paramName}_Null";
            }

            var filterValue = paramValue.ToString()?.Trim().Replace(" ", "_") ?? "Null";
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
                var valStr = parameters.ToString()?.Trim().Replace(" ", "_");
                if (string.IsNullOrWhiteSpace(valStr)) valStr = "Null";
                return $"{typeof(TEntity).Name}_{valStr}";
            }

            var paramType = parameters.GetType();

            // Xử lý SingleParameter<TEntity>
            if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(SingleParameter<>))
            {
                dynamic singleParam = parameters;
                var singleVal = singleParam.Value?.ToString()?.Trim().Replace(" ", "_");
                if (string.IsNullOrWhiteSpace(singleVal))
                    singleVal = "Null";
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
                        var singleVal = filter.Value?.ToString()?.Trim().Replace(" ", "_");
                        if (!string.IsNullOrWhiteSpace(singleVal))
                        {
                            return $"{typeof(TEntity).Name}_{filter.Key}_{singleVal}";
                        }
                    }
                }

                key += $"Page_{dynamicParams.PageNumber}_Size_{dynamicParams.PageSize}";

                // Sắp xếp filter theo Key và bỏ qua filter rỗng / khoảng trắng
                if (dynamicParams.Filters is IEnumerable<KeyValuePair<string, object>> filters)
                {
                    foreach (var filter in filters.OrderBy(f => f.Key))
                    {
                        if (filter.Value == null) continue;
                        if (filter.Value is string s && string.IsNullOrWhiteSpace(s)) continue;

                        var filterValue = filter.Value.ToString()?.Trim().Replace(" ", "_");
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            key += $"_{filter.Key}_{filterValue}";
                        }
                    }
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
                .OrderBy(p => p.Name)
                .Select(p =>
                {
                    var val = p.GetValue(parameters)?.ToString()?.Trim().Replace(" ", "_");
                    if (string.IsNullOrWhiteSpace(val)) val = "Null";
                    return $"{p.Name}_{val}";
                }));

            return key + paramString;
        }

        #endregion
    }
}
