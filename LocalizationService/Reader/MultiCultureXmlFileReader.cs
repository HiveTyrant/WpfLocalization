using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml;
using LocalizationService.Localization;

namespace LocalizationService.Reader;

public class MultiCultureXmlFileReader : FileReader<Dictionary<string, Dictionary<string, LocalizationEntry>>>
{

    public MultiCultureXmlFileReader(string path) : base(path) { }

    /// <summary>
    /// Returns content of MultiCultureLocalization file.  Notice, does not support pluralization of string values as SingleCulture readers does
    /// </summary>
    /// <returns>
    /// Dictionary[CultureName, Dictionary[Lookup_Key, LocalizationEntry]]
    /// </returns>
    /// <exception cref="FileFormatException"></exception>
    internal override Dictionary<string, Dictionary<string, LocalizationEntry>> GetEntries()
    {
        var doc = new XmlDocument();
        doc.Load(Path);

        var languages = BuildLanguageList(doc);
        var result = BuildTranslationList(doc, languages);

        return result;
    }

    private Dictionary<string, Dictionary<string, LocalizationEntry>> BuildTranslationList(XmlDocument doc, Dictionary<string, XmlNode> languages)
    {
        var result = new Dictionary<string, Dictionary<string, LocalizationEntry>>();

        var translationNodes = doc.SelectNodes("//localization/translations/tran");
        if (null == translationNodes || 0 == translationNodes.Count)
            throw new FileFormatException(@$"Invalid xml file - missing /localization/translations/tran nodes. File: {Path}");

        foreach (XmlNode tran in translationNodes)
        {
            var keyAttr = tran.Attributes?["key"];
            if (keyAttr == null || string.IsNullOrWhiteSpace(keyAttr.InnerText))
                throw new FileFormatException(@$"All /localization/translations/tran nodes must have a 'key' attribute. File: {Path}");

            foreach (var cultureName in languages.Keys)
            {
                var valueAttr = tran.Attributes?[cultureName];
                if (null != valueAttr && !string.IsNullOrWhiteSpace(valueAttr.InnerText))
                {
                    if (!result.ContainsKey(cultureName)) result[cultureName] = new Dictionary<string, LocalizationEntry>();

                    var cultureTranslations = result[cultureName];
                    cultureTranslations[keyAttr.InnerText] = new LocalizationEntry(valueAttr.InnerText);
                }
            }
        }

        return result;
    }

    private Dictionary<string, XmlNode> BuildLanguageList(XmlDocument doc)
    {
        var result = new Dictionary<string, XmlNode>();

        var languagesNodes = doc.SelectNodes("//localization/languages/language");
        if (null == languagesNodes || 0 == languagesNodes.Count)
            throw new FileFormatException(@$"Invalid xml file - missing /localization/languages/language nodes. File: {Path}");

        foreach (XmlNode language in languagesNodes)
        {
            var cultureAttr = language.Attributes?["culture"];
            if (cultureAttr == null) throw new FileFormatException(@$"All /localization/languages/language nodes must have a 'culture' attribute. File: {Path}");

            result[cultureAttr.InnerText] = language;
        }

        return result;
    }
}