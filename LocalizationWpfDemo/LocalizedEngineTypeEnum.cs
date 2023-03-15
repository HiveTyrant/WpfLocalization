using LocalizationService.Converters;
using LocalizationService.Localization;
using System.ComponentModel;

namespace LocalizationWpfDemo
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum LocalizedEngineTypeEnum
    {
        [LocalizedDescription("Key_EngineInternalCombustion")]
        InternalCombustion,

        [LocalizedDescription("Key_EngineElectric")]
        Electric,

        [LocalizedDescription("Key_EngineHybrid")]
        Hybrid,
    
        [Description("Steam engine (Non localized)")]
        Steam,
    
        // Enum value without description
        RubberBand
    }
}
