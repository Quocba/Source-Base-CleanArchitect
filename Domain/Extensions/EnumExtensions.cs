    using System.ComponentModel;

    namespace Domain.Extension
    {
        public static class EnumExtensions
        {
            public static string GetEnumDescription(this Enum Value)
            {
                var filed = Value.GetType().GetField(Value.ToString());
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(filed, typeof(DescriptionAttribute));
                return attribute != null ? attribute.Description : Value.ToString();
            }

            public static TEnum? ParseEnum<TEnum>(object value) where TEnum : struct, Enum
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
