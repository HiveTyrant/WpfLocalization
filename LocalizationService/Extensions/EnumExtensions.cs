using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationService.Extensions
{
    public static class EnumExtensions
    {
        public static bool HasDescriptionAttribute(this Enum value)
        {
            var attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault();

            return (attribute != null);
        }
    }
}
