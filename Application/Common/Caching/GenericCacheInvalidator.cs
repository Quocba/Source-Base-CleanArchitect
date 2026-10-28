using Microsoft.Extensions.Caching.Memory;
using System;

namespace Application.Common.Caching
{
    public class GenericCacheInvalidator<TEntity> : BaseCacheInvalidator<TEntity>
    {
        public GenericCacheInvalidator(IMemoryCache cache) : base(cache)
        {
        }

        protected override string GetEntityCacheKey(Guid entityId)
        {
            return $"{typeof(TEntity).Name}_{entityId}";
        }

        public string GetCacheKeyForList(object? parameters)
        {
            return GetCacheKey(parameters);
        }
    }
}
