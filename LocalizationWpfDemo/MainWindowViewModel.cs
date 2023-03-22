using LocalizationService.Localization;
using LocalizationService.Reader;
using LocalizationWpfDemo.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ForeignEnumAssembly;
using NLog.Extensions.Logging;

namespace LocalizationWpfDemo
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        private string _sliderValueKey = "PluralText";
        private int _sliderValue = 2;
        private float _floatValue = (float)12345.67;
        private CultureInfo? _currentCulture;
        private LocalizedEngineTypeEnum _selectedEngineType;
        private ObservableCollection<LocalizedEngineTypeEnum>? _engineTypes;
        private ICommand? _changeCultureCommand;

        #endregion

        #region Properties

        #region Commands

        public ICommand ChangeCultureCommand => _changeCultureCommand ??= new RelayCommand(arg => HandleChangeCulture(), null);

        #endregion

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
                if (_sliderValue == value) return;

                _sliderValue = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LocalizedEngineTypeEnum>? EngineTypes
        {
            get => _engineTypes;
            set
            {
                if (_engineTypes == value) return;
                _engineTypes = value;
                OnPropertyChanged(nameof(EngineTypes));
            }
        }

        public LocalizedEngineTypeEnum SelectedEngineType
        {
            get => _selectedEngineType;
            set
            {
                if (_selectedEngineType == value) return;

                _selectedEngineType = value;
                OnPropertyChanged();
            }
        }

        public ForeignAssemblyDefinedEnum ForeignEnum => ForeignAssemblyDefinedEnum.FieldNumber1;

        #endregion

        public MainWindowViewModel()
        {
            // Logging
            var loggerFactory = LoggerFactory.Create(builder => builder.AddNLog("nlog.config"));
            LocalizationManager.Instance.Logger = loggerFactory.CreateLogger<App>();

            // Update CurrentCulture property (used in TextBlock on view) when culture is changed
            LocalizationManager.Instance.PropertyChanged += (o, args) => CurrentCulture = LocalizationManager.Instance.CurrentCulture;

            // Read base translations from MultiCulture file - this file could be constructed from the output from a translation bureau  
            LocalizationManager.Instance.AddCultures(new MultiCultureXmlFileReader("LocalizationFiles/MultiCultureLocalization.xml"));

            // Add special/extra translations to already loaded MultiCulture translations as 3 culture files in CSV, XML and Json formats.
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("en-GB"), new SingleCultureCsvFileReader("LocalizationFiles/Extra.en-GB.txt")); 
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("da-DK"), new SingleCultureXmlFileReader("LocalizationFiles/Extra.da-DK.xml"));
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("fr"), new SingleCultureJsonFileReader("LocalizationFiles/Extra.fr.json"));

            // Add ValueFormat specifiers.  These are added in separate files here, so they don't have to be sent to translators
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("en-GB"), new SingleCultureCsvFileReader("LocalizationFiles/ValueFormatSpecifiers.en-GB.txt"));
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("da-DK"), new SingleCultureCsvFileReader("LocalizationFiles/ValueFormatSpecifiers.da-DK.txt"));
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("fr"), new SingleCultureCsvFileReader("LocalizationFiles/ValueFormatSpecifiers.fr.txt"));

            LocalizationManager.Instance.CurrentCulture = new CultureInfo("en-GB");

            EngineTypes = new ObservableCollection<LocalizedEngineTypeEnum>
            {
                LocalizedEngineTypeEnum.InternalCombustion,
                LocalizedEngineTypeEnum.Electric,
                LocalizedEngineTypeEnum.Hybrid,
                LocalizedEngineTypeEnum.Steam,
                LocalizedEngineTypeEnum.RubberBand
            };
        }

        #region Helper methods

        private void HandleChangeCulture()
        {
            var availableCultures = LocalizationManager.Instance.AvailableCultures;

            var index = (null == CurrentCulture) ? -1 : availableCultures.IndexOf(CurrentCulture);
            index = (index == availableCultures.Count) ? 0 : index + 1;
            if (index >= availableCultures.Count) index = 0;

            LocalizationManager.Instance.CurrentCulture = availableCultures[index];
        }

        #endregion


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
