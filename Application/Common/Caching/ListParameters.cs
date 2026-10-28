using System;
using System.Collections.Generic;

namespace Application.Common.Caching
{
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
            AddFilter(key, value);
        }

        /// <summary>
        /// Thêm parameter đơn dạng Fluent API
        /// </summary>
        public ListParameters<TEntity> AddParameter(string key, object? value)
        {
            AddFilter(key, value);
            return this;
        }

        public void AddFilter<T>(string key, T? value)
        {
            if (value != null)
            {
                if (value is string str)
                {
                    if (string.IsNullOrWhiteSpace(str))
                    {
                        return;
                    }
                    Filters[key] = str.Trim();
                    return;
                }
                Filters[key] = value;
            }
        }
    }
}
