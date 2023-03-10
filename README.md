# WpfLocalization

A simple library that makes WPF Localization easier.

## Features

- An easy extension to use `{l:Localize Title1}`
- Handles bindable Key `{l:Localize Key={Binding TitleKey}}`
- Handles alternative keys when the Key is not provided or not found (ControlName_PropertyName, e.g. Button1_Content)
- Handles singular, zero and plural values `{l:Localize Key=Sentence1, CountSource={Binding Value}}`
  - The zero form is automatically used when the Count value is equal to 0
  - The plural form is automatically used when the Count value is greater than 0
- Ability to change the current culture without having to restart the application.
- Supports ability to change string format specified at runtime

## Demo

![Demo GIF](https://i.imgur.com/fEO8Fjp.gif)

## How to use

### Things that you should know

- **WpfLocalization** doesn't use resources.resx files.
- **WpfLocalization** works with cultures to identify languages (`CultureInfo` class)
- There are 4 types of readers for now:
  - SingleCultureCsvFileReader: Each line is considered to have at least a key and a value seperated by a ;-char (e.g. TODO: Add link)
  - SingleCultureJsonFileReader: A json file of a key-object format (e.g. TODO: Add link)
  - SingleCultureXmlFileReader: A xml file with an `<Entries>` root and `<Entry>` elements, each must have a key attribute (e.g. TODO: Add link)
  - MultiCultureXmlFileReader: A xml file with an more complex format, that supports loading multible cultures from one file, eg. received from a translation bureau(e.g. TODO: Add link)

    Notice, only the SingleCulture readers support loading zero and plural string values

### Adding cultures (languages)

On startup (for example in App.xaml.cs or MainWindow constructor) you'll have to register the cultures you want your applications to have:
```
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);

            // Read base translations from MultiCulture file - this file could be constructed from the output from a translation bureau  
            LocalizationManager.Instance.AddCultures(new MultiCultureXmlFileReader("LocalizationFiles/OrigoMulticultureLocalization.xml"));

            // Add special/extra translations to already loaded MultiCulture translations as single culture files in CSV, XML or Json formats.
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("en-GB"), new SingleCultureCsvFileReader("LocalizationFiles/Extra.en-GB.txt"), true);
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("da-DK"), new SingleCultureXmlFileReader("LocalizationFiles/Extra.da-DK.xml"));
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("fr"), new SingleCultureJsonFileReader("LocalizationFiles/Extra.fr.json"));

            // Add ValueFormat specifiers.  These are added in separate files here, so they don't have to be sent to translators
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("en-GB"), new SingleCultureCsvFileReader("LocalizationFiles/ValueFormatSpecifiers.en-GB.txt"));
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("da-DK"), new SingleCultureCsvFileReader("LocalizationFiles/ValueFormatSpecifiers.da-DK.txt"));
            LocalizationManager.Instance.AddCulture(CultureInfo.GetCultureInfo("fr"), new SingleCultureCsvFileReader("LocalizationFiles/ValueFormatSpecifiers.fr.txt"));}
```

The `true` in the first `AddCulture` call tells the `LocalizationManager` to choose this language for now.

### Simple localization

    <TextBlock Margin="4" Text="{_:Localize Key1}" />
    <TextBlock Margin="4" Text="{_:Localize Key=Key1_1}" />

### Bindable Key

    <TextBlock Margin="4" Text="{localization:Localize KeySource={Binding Key} />

### Alterternative Key

    <TextBlock Margin="4" Name="LblTitle" Text="{localization:Localize}" />

Since the Key is not provided, the `LocalizationManager` will use the alternative key, in this case it's **LblTitle_Text**.

### Handling singular, zero and plural

    <TextBlock Margin="4"
               Text="{localization:Localize KeySource={Binding Key}, 
                                            CountSource={Binding Value}}" />

The LocalizationManager will adapt the Text property whenever the Key or Count change:
 - If the Count value is 0, the `ZeroValue` is chosen.
 - If the Count value is 1, the `Value` is chosen.
 - Otherwise the PluralValue is chosen `string.Format(PluralValue, Count)`

### Changing the culture (language)

    LocalizationManager.Instance.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

Whenever the CurrentCulture changes, the whole application is automatically updated without the need to restart it.

For a list of the available/added cultures:

    LocalizationManager.Instance.AvailableCultures

### Credits

This project is based on the EasyLocalization repository made by zHaytam (https://github.com/zHaytam/EasyLocalization), but has been extended with new features and updated to .Net7