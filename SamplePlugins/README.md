# SDK samples and usage recipes

The sample and template sources target SDK **1.31.0** and the checked host **78.0** baseline. They are examples for plugin developers. [SDK recipes](Arclyra.SdkExamples/README.md) show newer contracts as opt-in compilable helpers.

Build the local SDK package first if 1.31.0 is unavailable from your configured NuGet feed. From the repository root on Windows with .NET 8:

```powershell
dotnet pack src/Arclyra.PluginSdk/Arclyra.PluginSdk.csproj -c Release -o artifacts/nuget
dotnet restore SamplePlugins/Arclyra.PluginTemplate/Arclyra.PluginTemplate.csproj --source ./artifacts/nuget --source https://api.nuget.org/v3/index.json
dotnet build SamplePlugins/Arclyra.PluginTemplate/Arclyra.PluginTemplate.csproj -c Release --no-restore
dotnet restore SamplePlugins/Arclyra.SamplePlugin/Arclyra.SamplePlugin.csproj --source ./artifacts/nuget --source https://api.nuget.org/v3/index.json
dotnet build SamplePlugins/Arclyra.SamplePlugin/Arclyra.SamplePlugin.csproj -c Release --no-restore
dotnet build SamplePlugins/Arclyra.SdkExamples/Arclyra.SdkExamples.csproj -c Release
```

The template and sample use the SDK's bundled packaging helper. Package references exclude the SDK's runtime assembly, and the helper excludes host-shared DLLs from `.arcplugin` staging. Sample/template assembly and manifest versions are synchronized. Customize the ID, metadata, minimum host and capabilities for your own plugin before distribution.

The signed/unsigned archives under `packaged/` are historical **sample 1.6.0** binaries. They do not contain the updated source or demonstrate the current SDK. Build a fresh unsigned development package from source; obtain your own developer identity for signing. Existing signatures must not be reused for modified content.
