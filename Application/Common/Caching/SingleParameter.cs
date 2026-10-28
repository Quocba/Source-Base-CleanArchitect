using System;

namespace Application.Common.Caching
{
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
            if (value is string str)
            {
                Value = string.IsNullOrWhiteSpace(str) ? null : str.Trim();
            }
            else
            {
                Value = value;
            }
        }

        public SingleParameter(object? value)
        {
            Key = "Param";
            if (value is string str)
            {
                Value = string.IsNullOrWhiteSpace(str) ? null : str.Trim();
            }
            else
            {
                Value = value;
            }
        }
    }
}
