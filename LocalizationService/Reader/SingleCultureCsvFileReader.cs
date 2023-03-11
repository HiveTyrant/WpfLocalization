using System.IO;
using LocalizationService.Localization;

namespace LocalizationService.Reader
{
    public class SingleCultureCsvFileReader : FileReader<Dictionary<string, LocalizationEntry>>
    {
        #region Fields

        private readonly char _separator;

        #endregion

        public SingleCultureCsvFileReader(string path, char separator = ';') : base(path)
        {
            _separator = separator;
        }

        internal override Dictionary<string, LocalizationEntry> GetEntries()
        {
            var entries = new Dictionary<string, LocalizationEntry>();

            using (var sr = File.OpenText(Path))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine() ?? "";
                    var splits = line.Split(_separator);

                    // Each line needs to have at least 2 values (key, value)
                    if (splits.Length == 1) throw new FileFormatException($"Each line needs to have at least 2 values (line: {line})");

                    switch (splits.Length)
                    {
                        default:
                            entries.Add(splits[0], new LocalizationEntry(splits[1]));
                            break;
                        case 3:
                            entries.Add(splits[0], new LocalizationEntry(splits[1], splits[2]));
                            break;
                        case 4:
                            entries.Add(splits[0], new LocalizationEntry(splits[1], splits[2], splits[3]));
                            break;
                    }
                }
            }

            return entries;
        }
    }
}