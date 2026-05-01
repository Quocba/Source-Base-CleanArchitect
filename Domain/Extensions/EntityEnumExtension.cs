using Domain.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class EntityEnumExtension
    {
        public static bool TryParseName(string? name, out EntityEnum entity)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                entity = EntityEnum.Unknown;
                return false;
            }

            if (Enum.TryParse<EntityEnum>(name.Trim(), true, out var parsed))
            {
                entity = parsed;
                return true;
            }

            entity = EntityEnum.Unknown;
            return false;
        }
    }
}
