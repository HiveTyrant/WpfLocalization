using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using LocalizationService.Reader;

namespace LocalizationService.Localization
{
    public class LocalizationManager : INotifyPropertyChanged
    {
        #region Singleton

        private static LocalizationManager? _instance;

        public static LocalizationManager Instance => _instance ??= new LocalizationManager();

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Fields

        private readonly Dictionary<CultureInfo, Dictionary<string, LocalizationEntry>> _languageEntries = new();
        private CultureInfo? _currentCulture;

        #endregion

        #region Properties

        public CultureInfo? CurrentCulture
        {
            get => _currentCulture;
            set
            {
                if (null == value) return;
                if (_currentCulture?.Equals(value) == true) return;
                if (!_languageEntries.ContainsKey(value)) return;

                _currentCulture = value;
                OnPropertyChanged();
            }
        }

        public List<CultureInfo> AvailableCultures => _languageEntries.Keys.ToList();

        #endregion

        #region Public Methods


        public void AddCulture(CultureInfo culture, FileReader<Dictionary<string, LocalizationEntry>> reader, bool setAsCurrentCulture = false)
        {
            var cultureEntry = reader.GetEntries();
            UpdateCultureEntry(culture, cultureEntry);

            if (setAsCurrentCulture) CurrentCulture = culture;
        }

        public void AddCultures(FileReader<Dictionary<string, Dictionary<string, LocalizationEntry>>> reader)
        {
            var cultureEntries = reader.GetEntries();
            foreach (var cultureEntry in cultureEntries)
            {
                var culture = CultureInfo.GetCultureInfo(cultureEntry.Key);
                UpdateCultureEntry(culture, cultureEntry.Value);
            }
        }

        public string? GetValue(string? key, bool nullWhenUnfound = true)
        {
            if (_languageEntries == null || CurrentCulture == null)
                return key;

            var entries = _languageEntries[CurrentCulture];

            if (key == null || !entries.ContainsKey(key))
                return nullWhenUnfound ? null : key;

            return entries[key].Value;
        }

        public string? GetValue(string? key, int count, bool nullWhenNotfound = true)
        {
            if (_languageEntries == null || CurrentCulture == null)
                return key;

            var entries = _languageEntries[CurrentCulture];

            if (key == null || !entries.ContainsKey(key))
                return nullWhenNotfound ? null : key;

            var entry = entries[key];
            return count == 0 ? entry.ZeroValue : count == 1 ? entry.Value : string.Format(entry.PluralValue ?? "", count);
        }

        #endregion
        private void UpdateCultureEntry(CultureInfo culture, Dictionary<string, LocalizationEntry> cultureEntry)
        {
            if (!_languageEntries.ContainsKey(culture)) 
                // If no translation list exist for culture, then add as new translation list
                _languageEntries[culture] = cultureEntry;
            else
                // Update existing translation list by adding new translations to existing (overriding entries with same LocalizationKey
                foreach (var localizationEntry in cultureEntry)
                    _languageEntries[culture][localizationEntry.Key] = localizationEntry.Value;
        }
    }
}
