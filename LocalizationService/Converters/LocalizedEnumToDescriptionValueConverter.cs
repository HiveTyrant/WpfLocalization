using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using LocalizationService.Localization;

namespace LocalizationService.Converters;

public class LocalizedEnumToDescriptionValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (null == value) return string.Empty;

        var type = value.GetType();

        return !type.IsEnum ? string.Empty : GetLocalizedDescription(type, value);
    }

    private static string? GetLocalizedDescription(Type enumType, object? enumValue)
    {
        var descriptionAttribute = enumType.
                                   GetField(enumValue.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).
                                   FirstOrDefault() as DescriptionAttribute;
        var key = (null == descriptionAttribute) ? enumValue.ToString() : descriptionAttribute.Description;
        var result = LocalizationManager.Instance.GetValue(key, false);
        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}