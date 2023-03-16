using System.ComponentModel;

namespace LocalizationService.Localization;

/// <summary> Specifies a localized description for a property or event </summary>
public class LocalizedDescriptionAttribute : DescriptionAttribute
{
    private readonly string _key;

    public LocalizedDescriptionAttribute(string key)
    {
        _key = key;
    }

    /// <summary> Gets the localized description stored in this attribute. </summary>
    public override string Description
    {
        get
        {
            var description = LocalizationManager.Instance.GetValue(_key);
            var result = string.IsNullOrWhiteSpace(description) ? _key : description;
            return result;
        }
    }
}