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
        /// <summary> Returns true if Enum value has a Description </summary>
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
