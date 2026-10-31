# SDK samples and usage recipes

The source examples now reference SDK **1.34.0**. `Arclyra.PluginTemplate` uses a deliberately minimal host **78.0** baseline, while `Arclyra.SamplePlugin` requires host **92.0** because its demo direct generator opts into source-bound cancellation. [SDK recipes](Arclyra.SdkExamples/README.md) include opt-in helpers for host 86 summary progress, host 92 source-bound generation and host 104 trusted-only review result progress. Always choose the minimum host required by APIs actually called; SDK package and host versions are independent.

Build the local SDK package first if 1.34.0 is unavailable from your configured NuGet feed. From the repository root on Windows with .NET 8:

```powershell
dotnet pack src/Arclyra.PluginSdk/Arclyra.PluginSdk.csproj -c Release -o artifacts/nuget
dotnet restore SamplePlugins/Arclyra.PluginTemplate/Arclyra.PluginTemplate.csproj --source ./artifacts/nuget --source https://api.nuget.org/v3/index.json
dotnet build SamplePlugins/Arclyra.PluginTemplate/Arclyra.PluginTemplate.csproj -c Release --no-restore
dotnet restore SamplePlugins/Arclyra.SamplePlugin/Arclyra.SamplePlugin.csproj --source ./artifacts/nuget --source https://api.nuget.org/v3/index.json
dotnet build SamplePlugins/Arclyra.SamplePlugin/Arclyra.SamplePlugin.csproj -c Release --no-restore
dotnet build SamplePlugins/Arclyra.SdkExamples/Arclyra.SdkExamples.csproj -c Release
```

The template and sample use the SDK's bundled packaging helper. Package references exclude the SDK's runtime assembly, and the helper excludes host-shared DLLs from `.arcplugin` staging. Template source/manifest versions are **0.1.2**, and SamplePlugin source/manifest versions are **1.6.2**. Customize the ID, metadata, minimum host and capabilities for your own plugin before distribution.

The signed/unsigned archives under `packaged/` are historical **sample 1.6.0** binaries; the folder and every archive remain untouched. They do not contain the updated source or demonstrate the current SDK. Build a fresh unsigned development package from source; obtain your own developer identity for signing. Existing signatures must not be reused for modified content.
