using System.Globalization;
using System.Windows.Data;
using LocalizationService.Localization;

namespace LocalizationService.Converters;

public class LocalizedValueConverter : IValueConverter
{
    private const string DefaultDateFormat = "yyyyMMdd HH:mm:ss";
    private const string DefaultDecimalFormat = "C2";
    private const string DefaultFloatFormat = "N2";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch (value)
        {
            case null:
                return string.Empty;
            case DateTime dateTimeValue:
                var stringFormat = (null != parameter) ? parameter.ToString() : (LocalizationManager.Instance.GetValue("DateStringFormat", true) ?? DefaultDateFormat);
                var result = dateTimeValue.ToString(stringFormat, LocalizationManager.Instance.CurrentCulture);
                return result;
            case decimal decimalValue:
                stringFormat = (null != parameter) ? parameter.ToString() : (LocalizationManager.Instance.GetValue("DecimalStringFormat", true) ?? DefaultDecimalFormat);
                result = decimalValue.ToString(stringFormat, LocalizationManager.Instance.CurrentCulture);
                return result;
            case float floatValue:
                stringFormat = (null != parameter) ? parameter.ToString() : (LocalizationManager.Instance.GetValue("FloatStringFormat", true) ?? DefaultFloatFormat);
                result = floatValue.ToString(stringFormat, LocalizationManager.Instance.CurrentCulture);
                return result;
            default:
                return value?.ToString() ?? string.Empty;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}