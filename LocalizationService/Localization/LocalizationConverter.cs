using System.Globalization;
using System.Windows.Data;

namespace LocalizationService.Localization
{
    public class LocalizationConverter : IMultiValueConverter
    {

        #region Fields

        private string _key;
        private string _alternativeKey;

        #endregion

        public LocalizationConverter(string key, string alternativeKey)
        {
            _key = key;
            _alternativeKey = alternativeKey;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string? result;

            object? key;
            int count;

            switch (values.Length)
            {
                default:
                    // (1) CultureInfo
                    result = LocalizationManager.Instance.GetValue(_key, _alternativeKey, false);
                    break;

                case 2:
                    // (2) CultureInfo + KeySource or CultureInfo + CountSource
                    key = values.FirstOrDefault(v => v is string);

                    if (key == null)
                    {
                        // CultureInfo + CountSource
                        count = System.Convert.ToInt32(values.First(v => !(v is CultureInfo)));
                        result = LocalizationManager.Instance.GetValue(_key, _alternativeKey, count, false);
                    }
                    else
                    {
                        // CultureInfo + KeySource
                        result = LocalizationManager.Instance.GetValue(key.ToString(), _alternativeKey, false);
                    }
                    break;

                case 3:
                    // (3) CultureInfo + KeySource + CountSource
                    key = values.FirstOrDefault(v => v is string);
                    count = System.Convert.ToInt32(values.First(v => v != key && !(v is CultureInfo)));
                    result = LocalizationManager.Instance.GetValue(key?.ToString() ?? "", _alternativeKey, count, false);
                    break;
            }

            return result ?? string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
