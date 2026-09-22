using System;
using System.ComponentModel;

namespace Application.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();
            var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute != null ? attribute.Description : value.ToString();
        }

        public static TEnum? ParseEnum<TEnum>(object? value) where TEnum : struct, Enum
        {
            if (value == null) return null;

            var str = value.ToString()?.Trim();

            if (string.IsNullOrEmpty(str)) return null;

            if (Enum.TryParse<TEnum>(str, true, out var result))
            {
                return result;
            }

            return null;
        }
    }
}
