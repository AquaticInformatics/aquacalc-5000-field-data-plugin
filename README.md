# AquaCalc 5000 Field Data Plugin

[![Build status](https://ci.appveyor.com/api/projects/status/sw9ylnimsh01dv75/branch/master?svg=true)](https://ci.appveyor.com/project/SystemsAdministrator/aquacalc-5000-field-data-plugin/branch/master)

An AQTS field data plugin for AQTS 2019.2-or-newer systems, which can read discharge summary measurements from [AquaCalc 5000](http://aquacalc.com/Instruments/Products/AquaCalc_5000/aquacalc_5000.html) CSV files.

## Want to install this plugin?

- Download the latest release of the plugin [here](../../releases/latest)
- Install it on AQTS 2019.2-or-newer via the System Configuration page

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
