# AquaCalc 5000 Field Data Plugin

[![Build status](https://ci.appveyor.com/api/projects/status/sw9ylnimsh01dv75/branch/master?svg=true)](https://ci.appveyor.com/project/SystemsAdministrator/aquacalc-5000-field-data-plugin/branch/master)

An AQTS field data plugin for AQTS 2019.2-or-newer systems, which can read discharge summary measurements from [AquaCalc 5000](http://aquacalc.com/Instruments/Products/AquaCalc_5000/aquacalc_5000.html) CSV files.

## Want to install this plugin?

- Install it on AQTS 2019.2-or-newer via the System Configuration page

### Plugin Compatibility Matrix

Choose the appropriate version of the plugin for your AQTS app server.

| AQTS Version | Latest compatible plugin Version |
| --- | --- |
| AQTS 2020.2 | [v20.2.0](https://github.com/AquaticInformatics/aquacalc-5000-field-data-plugin/releases/download/v20.2.0/AquaCalc5000.plugin) |
| AQTS 2020.1<br/>AQTS 2019.4<br/>AQTS 2019.3<br/>AQTS 2019.2| [v19.2.10](https://github.com/AquaticInformatics/aquacalc-5000-field-data-plugin/releases/download/v19.2.10/AquaCalc5000.plugin) |

## Configuring the plugin

The plugin has one configurable setting. The configuration settings are stored in different places, depending on the version of the plugin.

| Version | Configuration location |
| --- | --- |
| 20.2.x | Use the Settings page of the System Config app to change the settings.<br/><br/>**Group**: `FieldDataPluginConfig-AquaCalc5000`<br/>**Key**: `AssumeUsgsSiteIdentifiers`<br/>**Value**: Either `true` or `false`. Defaults to `true` if not set.|
| 19.2.x | Read from the INI file in the plugin folder, at `%ProgramData%\Aquatic Informatics\AQUARIUS Server\FieldDataPlugins\AquaCalc5000\Config.ini`<br/><br/>`AssumeUsgsSiteIdentifiers=true` |

### `AssumeUsgsSiteIdentifiers`
`AssumeUsgsSiteIdentifiers` defaults to true, and controls whether USGS-style 8-digit site identifiers should be used when the `GAGE ID#` is numeric.

When `AssumeUsgsSiteIdentifiers` is true, leading zeros will be added to site identifier so that they conform to the 8-digit identifier format.
When `AssumeUsgsSiteIdentifiers` is false, no leading zeros will be added, and the `GAGE ID#` value will be used as-is.

## Requirements for building the plugin from source

- Requires Visual Studio 2017 (Community Edition is fine)
- .NET 4.7.2 runtime

## Building the plugin

- Load the `src\AquaCalc5000Plugin.sln` file in Visual Studio and build the `Release` configuration.
- The `src\AquaCalc5000\deploy\Release\AquaCalc5000.plugin` file can then be installed on your AQTS app server.

## Testing the plugin within Visual Studio

Use the included `PluginTester.exe` tool from the `Aquarius.FieldDataFramework` package to test your plugin logic on the sample files.

1. Open the AquaCalc5000 project's **Properties** page
2. Select the **Debug** tab
3. Select **Start external program:** as the start action and browse to `"src\packages\Aquarius.FieldDataFramework.19.2.2\tools\PluginTester.exe`
4. Enter the **Command line arguments:** to launch your plugin

```
/Plugin=AquaCalc5000.dll /Json=AppendedResults.json /Data=..\..\..\..\data\*.csv
```

The `/Plugin=` argument can be the filename of your plugin assembly, without any folder. The default working directory for a start action is the bin folder containing your plugin.

5. Set a breakpoint in the plugin's `ParseFile()` methods.
6. Select your plugin project in Solution Explorer and select **"Debug | Start new instance"**
7. Now you're debugging your plugin!

See the [PluginTester](https://github.com/AquaticInformatics/aquarius-field-data-framework/tree/master/src/PluginTester) documentation for more details.

## Installation of the plugin

Use the System Configuration page of AQUARIUS Time Series to install the plugin.
