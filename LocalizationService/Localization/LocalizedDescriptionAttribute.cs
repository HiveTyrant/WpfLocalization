using System.ComponentModel;

namespace LocalizationService.Localization;

public class LocalizedDescriptionAttribute : DescriptionAttribute
{
    string _key;

    public LocalizedDescriptionAttribute(string key)
    {
        _key = key;
    }

    public override string Description
    {
        get
        {
            var description = LocalizationManager.Instance.GetValue(_key);
            var result = $"{(string.IsNullOrWhiteSpace(description) ? ("[[" + _key +"]]") : description)}";
            return result;
        }
    }
}