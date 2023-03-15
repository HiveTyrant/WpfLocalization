using System.Collections;
using System.Windows.Markup;
using LocalizationService.Extensions;

namespace LocalizationService.MarkupExtensions;

/// <summary> MarkupExtension that returns the values (fields) of an Enum, where the value has a declared Description attribute </summary>
public class EnumBindingSourceExtension : MarkupExtension
{
    #region Fields

    private Type _enumType;

    #endregion

    #region Properties

    public Type EnumType
    {
        get => _enumType;
        set
        {
            if (value != _enumType)
            {
                if (null != value)
                {
                    var enumType = Nullable.GetUnderlyingType(value) ?? value;
                    if (!enumType.IsEnum)
                        throw new ArgumentException("Type must be for an Enum.");
                }

                _enumType = value;
            }
        }
    }

    #endregion

    #region Initialization

    public EnumBindingSourceExtension() { }

    public EnumBindingSourceExtension(Type enumType)
    {
        EnumType = enumType;
    }

    #endregion

    #region Public Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        object result;

        if (null == _enumType) throw new InvalidOperationException("The EnumType must be specified.");

        var actualEnumType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
        var enumValues = Enum.GetValues(actualEnumType);

        if (actualEnumType == _enumType)
        {
            result = SortEnumValuesByIndex(enumValues);
        }
        else
        {
            var tmpArr = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tmpArr, 1);
            result = SortEnumValuesByIndex(tmpArr);
        }

        return result;
    }

    #endregion

    #region Helpers

    private static object SortEnumValuesByIndex(IEnumerable enumValues)
    {
        var values = enumValues.Cast<Enum>().ToList();
        var indexed = new Dictionary<int, Enum>();

        foreach (var value in values)
        {
            var index = (int)Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
            indexed.Add(index, value);
        }

        var result = indexed.OrderBy(x => x.Key).Select(x => x.Value).Where(x => x.HasDescriptionAttribute()).Cast<Enum>();
        return result;
    }

    #endregion
}