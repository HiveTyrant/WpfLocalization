using LocalizationService.Converters;
using LocalizationService.Localization;
using System.ComponentModel;

namespace ForeignEnumAssembly
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ForeignAssemblyDefinedEnum
    {
        [LocalizedDescription("Key_FieldNumber1")]
        FieldNumber1,

        [LocalizedDescription("Key_FieldNumber2")]
        FieldNumber2,
    }
}
