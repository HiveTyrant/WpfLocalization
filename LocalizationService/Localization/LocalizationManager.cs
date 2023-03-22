using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using LocalizationService.Reader;
using Microsoft.Extensions.Logging;

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
        public Microsoft.Extensions.Logging.ILogger? Logger { get; set; }

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

        public string? GetValue(string? key, string? alternativeKey, bool nullWhenNotFound = true)
        {
            var result = GetValue(key) ?? GetValue(alternativeKey);

            if (null == result && !nullWhenNotFound)
            {
                // If no localization was found and nullWhenNotFound==false, then return used key vale
                if (!string.IsNullOrWhiteSpace(key)) result = key;
                else if (!string.IsNullOrWhiteSpace(key)) result = alternativeKey;
            }

            return result;
        }

        public string? GetValue(string? key, string? alternativeKey, int count, bool nullWhenNotFound = true)
        {
            var result = GetValue(key, count) ?? GetValue(alternativeKey, count, nullWhenNotFound);

            if (null == result && !nullWhenNotFound)
            {
                // If no localization was found and nullWhenNotFound==false, then return used key vale
                if (!string.IsNullOrWhiteSpace(key)) result = key;
                else if (!string.IsNullOrWhiteSpace(key)) result = alternativeKey;
            }

            return result;
        }

        public string? GetValue(string? key, bool nullWhenNotFound = true)
        {
            var entry = LookupLocalizationEntry(key);
            var result = entry?.Value ?? (nullWhenNotFound ? null : key);
            return result;
        }

        public string? GetValue(string? key, int count, bool nullWhenNotFound = true)
        {
            var entry = LookupLocalizationEntry(key);
            var result = (null == entry)
                ? (nullWhenNotFound ? null : key)
                : (count == 0 ? entry.ZeroValue : count == 1 ? entry.Value : string.Format(entry.PluralValue ?? "", count));
            return result;
        }

        #endregion

        #region Helper methods

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

        private LocalizationEntry? LookupLocalizationEntry(string? key)
        {
            if (null == key)
            {
                Logger?.LogWarning("LocalizationManager lookup failed: No key specified");
                return null;
            }

            if (!_languageEntries.Any())
                Logger?.LogWarning("LocalizationManager lookup failed: No log-language entries loaded");
            else if (null == CurrentCulture)
                Logger?.LogWarning("LocalizationManager lookup failed: CurrentCulture not set");
            else
            {
                var entries = _languageEntries[CurrentCulture];
                if (entries.ContainsKey(key))
                    return entries[key];

                Logger?.LogWarning("LocalizationManager lookup failed: no text found for Key: [{}] for Culture: [{}]", key, CurrentCulture);
            }

            return null;
        }

        #endregion
    }
}
