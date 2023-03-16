using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using LocalizationService.Localization;

namespace LocalizationService.Converters;

public class LocalizedTextValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (null == value) return string.Empty;

        var result = LocalizedValue(value);
        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private object LocalizedValue(object value)
    {
        var type = value.GetType();

        if (type.IsEnum) return GetLocalizedEnumDescription(type, value);
        if (value is string valueStr) return GetLocalizedString(valueStr);
        if (value is IEnumerable collection) return GetLocalizeCollection(collection);
        return GetLocalizedString(value.ToString());
    }

    private static string GetLocalizedEnumDescription(Type enumType, object? enumValue)
    {
        var descriptionAttribute = enumType.
                                   GetField(enumValue.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).
                                   FirstOrDefault() as DescriptionAttribute;
        var key = (null == descriptionAttribute) ? enumValue.ToString() : descriptionAttribute.Description;
        var result = LocalizationManager.Instance.GetValue(key, false);
        return result;
    }

    private IEnumerable<string> GetLocalizeCollection(IEnumerable collection)
    {
        var result = new List<string>();
        foreach (var value in collection)
        {
            var str = LocalizedValue(value).ToString(); // TODO: Handle nested collections here
            result.Add(str);
        }

        return result;
    }

    private string GetLocalizedString(string? value)
    {
        var result = LocalizationManager.Instance.GetValue(value, false);
        return result;
    }

}