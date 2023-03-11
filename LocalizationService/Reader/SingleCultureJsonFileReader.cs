using System.Collections.Generic;
using System.IO;
using LocalizationService.Localization;
using System.Text.Json;

namespace LocalizationService.Reader;

public class SingleCultureJsonFileReader : FileReader<Dictionary<string, LocalizationEntry>>
{
    public SingleCultureJsonFileReader(string path) : base(path) { }

    internal override Dictionary<string, LocalizationEntry> GetEntries()
    {
        var result = JsonSerializer.Deserialize<Dictionary<string, LocalizationEntry>>(File.ReadAllText(Path));
        return result ?? new Dictionary<string, LocalizationEntry>();
    }
}