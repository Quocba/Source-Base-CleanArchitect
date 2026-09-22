using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

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

    /// <summary>
    /// Tham số đơn cho Cache
    /// </summary>
    public class SingleParameter<TEntity>
    {
        public string Key { get; set; } = string.Empty;
        public object? Value { get; set; }

        public SingleParameter() { }

        public SingleParameter(string key, object? value)
        {
            Key = key;
            Value = value;
        }

        public SingleParameter(object? value)
        {
            Key = "Param";
            Value = value;
        }
    }

    /// <summary>
    /// Tham số danh sách / filter cho Cache
    /// </summary>
    public class ListParameters<TEntity>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();

        public ListParameters()
        {
        }

        public ListParameters(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public ListParameters(Guid id)
        {
            PageNumber = 0;
            PageSize = 0;
            Filters.Add("Id", id);
        }

        /// <summary>
        /// Khởi tạo cache với 1 parameter đơn (Key - Value)
        /// </summary>
        public ListParameters(string key, object? value)
        {
            PageNumber = 0;
            PageSize = 0;
            if (value != null)
            {
                Filters[key] = value;
            }
        }

        /// <summary>
        /// Thêm parameter đơn dạng Fluent API
        /// </summary>
        public ListParameters<TEntity> AddParameter(string key, object? value)
        {
            if (value != null)
            {
                Filters[key] = value;
            }
            return this;
        }

        public void AddFilter<T>(string key, T? value)
        {
            if (value != null)
            {
                Filters[key] = value;
            }
        }
    }
}
