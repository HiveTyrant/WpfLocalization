using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace LocalizationService.Converters;


/// <summary>
/// Converts an Enum value (field) to the content of its DescriptionAttribute as a string.
/// If the Enum value doesn't declare a DescriptionAttribute value.ToString() is returned.
/// </summary>
public class EnumDescriptionTypeConverter : EnumConverter
{
    public EnumDescriptionTypeConverter(Type type) : base(type)
    {
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType != typeof(string)) 
            return base.ConvertTo(context, culture, value, destinationType);

        if (value != null)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi != null)
            {
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                // var result = ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : null;
                var result = ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
                return result;
            }
        }

        return string.Empty;

    }
}