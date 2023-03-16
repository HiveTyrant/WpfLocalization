using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using LocalizationService.Localization;

namespace LocalizationService.MarkupExtensions
{
    public class LocalizedTextExtension : MarkupExtension
    {
        #region Properties

        public string? Key { get; set; }

        public Binding? KeySource { get; set; }

        public Binding? CountSource { get; set; }

        #endregion

        #region Initialization

        public LocalizedTextExtension() { }

        public LocalizedTextExtension(string key)
        {
            Key = key;
        }

        public LocalizedTextExtension(string key, Binding countSource) : this(key)
        {
            CountSource = countSource;
        }

        public LocalizedTextExtension(Binding keySource, Binding countSource)
        {
            KeySource = keySource;
            CountSource = countSource;
        }

        #endregion

        #region Public Methods

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // hmm, this is also called at design time and serviceProvider may be null.... is that handled ok?

            var provideValueTarget = serviceProvider as IProvideValueTarget;
            var targetObject = provideValueTarget?.TargetObject as FrameworkElement;
            var targetProperty = provideValueTarget?.TargetProperty as DependencyProperty;
            var alternativeKey = $"{targetObject?.Name}_{targetProperty?.Name}";

            var multiBinding = new MultiBinding
            {
                Converter = new LocalizationConverter(Key, alternativeKey),
                NotifyOnSourceUpdated = true
            };

            multiBinding.Bindings.Add(new Binding
            {
                Source = LocalizationManager.Instance,
                Path = new PropertyPath("CurrentCulture")
            });

            if (KeySource != null) multiBinding.Bindings.Add(KeySource);
            if (CountSource != null) multiBinding.Bindings.Add(CountSource);

            var result = multiBinding.ProvideValue(serviceProvider);
            return result;
        }

        #endregion
    }
}