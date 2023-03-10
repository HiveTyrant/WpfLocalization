using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LocalizationService.Localization;
using System.Threading;
using LocalizationService.Reader;

namespace LocalizationWpfDemo
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        private string _sliderValueKey = "Key22";
        private int _sliderValue = 0;
        private float _floatValue = (float)12345.67;
        private CultureInfo? _currentCulture;

        #endregion

        #region Properties

        public float FloatValue 
        {
            get => _floatValue;
            set
            {
                _floatValue = value;
                OnPropertyChanged();
            }
        }

        public DateTime DateValue => DateTime.Now;

        public decimal CurrencyValue => (decimal)1234.56;

        public CultureInfo? CurrentCulture
        {
            get => _currentCulture;
            set
            {
                if (_currentCulture == value) return;
                _currentCulture = value;
                OnPropertyChanged();
            }
        }
        
        public string SliderValueKey
        {
            get => _sliderValueKey;
            set
            {
                if (_sliderValueKey == value)
                    return;

                _sliderValueKey = value;
                OnPropertyChanged();
            }
        }

        public int SliderValue
        {
            get => _sliderValue;
            set
            {
                if (_sliderValue == value)
                    return;

                _sliderValue = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            LocalizationManager.Instance.PropertyChanged += OnCultureChanged;

            // Read base translations from MultiCulture file - this file could be constructed from the output from a translation bureau  
            LocalizationManager.Instance.AddCultures(new MultiCultureXmlFileReader("LocalizationFiles/OrigoMulticultureLocalization.xml"));

            // Add special/extra translations to already loaded MultiCulture translations as 3 culture files in CSV, XML and Json formats.
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("en-GB"), new SingleCultureCsvFileReader("LocalizationFiles/Extra.en-GB.txt"), true);  // true => Default culture
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("da-DK"), new SingleCultureXmlFileReader("LocalizationFiles/Extra.da-DK.xml"));
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("fr"), new SingleCultureJsonFileReader("LocalizationFiles/Extra.fr.json"));

            // Add ValueFormat specifiers.  These are added in separate files here, so they don't have to be sent to translators
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("en-GB"), new SingleCultureCsvFileReader("LocalizationFiles/ValueFormatSpecifiers.en-GB.txt"));
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("da-DK"), new SingleCultureCsvFileReader("LocalizationFiles/ValueFormatSpecifiers.da-DK.txt"));
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("fr"), new SingleCultureCsvFileReader("LocalizationFiles/ValueFormatSpecifiers.fr.txt"));
        }

        private void OnCultureChanged(object? sender, PropertyChangedEventArgs e)
        {
            CurrentCulture = LocalizationManager.Instance.CurrentCulture;

            // When culture is changed, call PropertyChanged for all value fields to update bindings reflecting ValueFormat specifiers for selected culture
            OnPropertyChanged(nameof(FloatValue));
            OnPropertyChanged(nameof(DateValue));
            OnPropertyChanged(nameof(CurrencyValue));
         }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
