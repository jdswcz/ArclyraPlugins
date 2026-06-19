# Arclyra Plugin Developer Guide

Arclyra plugins extend Arclyra Writing Studio with optional commands, menu items, settings pages, workspace panels, story UI integrations, Smart Builder tools, AI-provider configuration helpers, and host-mediated data workflows. Plugins are distributed as `.arcplugin` packages, installed into the user's writable app-data area, discovered at runtime, and initialized through `Arclyra.PluginSdk`.

This repository is the maintained plugin-authoring reference. The SDK package README is intentionally short and links here for the full documentation. For working examples, build and inspect the sample plugins under `SamplePlugins/`.

## First setup steps

1. **Install the .NET 8 SDK.** Use a Windows development environment when the plugin contributes WPF UI.
2. **Create a .NET 8 class library.** Use `net8.0-windows` and enable WPF when the plugin contributes settings pages, panels, custom editors, or other WPF UI.
3. **Install `Arclyra.PluginSdk` from NuGet.** Add the SDK package from [NuGet](https://www.nuget.org/packages/Arclyra.PluginSdk/) using either the .NET CLI or Package Manager Console:

   ```bash
   dotnet add package Arclyra.PluginSdk
   ```

   ```powershell
   Install-Package Arclyra.PluginSdk
   ```

   Do not copy `Arclyra.PluginSdk.dll` into the final package; Arclyra supplies the SDK assembly at runtime so host and plugin use the same contract types.
4. **Implement `IArclyraPlugin`.** Keep `Id` stable and make it match `plugin.json`.
5. **Declare a `plugin.json` manifest.** Include entry assembly/type, version, host compatibility, and the least set of capabilities your plugin needs.
6. **Build and stage files under a `plugin/` folder.** Include `plugin/plugin.json`, your entry DLL, non-host managed dependencies, and required assets.
7. **Zip the staging folder and rename it to `.arcplugin`.** The archive root must contain the `plugin` folder. It may also contain root-level `plugin.package.json` signature metadata.
8. **Install in Arclyra.** Open **Plugins**, choose **Install Plugin**, select the `.arcplugin`, and let Arclyra validate, install, and reload plugins.
9. **Update by installing a newer package.** Keep the same plugin `id`; use an increasing `version` value.

Minimal project file for a WPF-capable plugin:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Arclyra.PluginSdk" Version="1.0.0" PrivateAssets="all" ExcludeAssets="runtime" />
  </ItemGroup>
</Project>
```

Minimal entry point:

```csharp
using Arclyra.PluginSdk;

namespace Example.WordTools;

public sealed class WordToolsPlugin : IArclyraPlugin
{
    public string Id => "com.example.arclyra.wordtools";

    public string Name => "Example Word Tools";

    public Task InitializeAsync(IPluginContext context, CancellationToken cancellationToken = default)
    {
        context.Logger.LogInformation("Example Word Tools initialized.");

        context.CommandRegistry.RegisterCommand(new PluginCommandRegistration(
            "com.example.arclyra.wordtools.sayHello",
            "Say Hello",
            _ =>
            {
                context.Logger.LogInformation("Hello from Example Word Tools.");
                return Task.CompletedTask;
            },
            "Writes a test message to the plugin log."));

        context.UiRegistry.RegisterMenuItem(new PluginMenuItemRegistration(
            "Tools/Plugins",
            "Example Word Tools",
            "com.example.arclyra.wordtools.sayHello"));

        return Task.CompletedTask;
    }

    public Task ShutdownAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
```

Minimal manifest:

```json
{
  "id": "com.example.arclyra.wordtools",
  "name": "Example Word Tools",
  "version": "1.0.0",
  "entryAssembly": "Example.WordTools.dll",
  "entryType": "Example.WordTools.WordToolsPlugin",
  "minHostVersion": "1.0.0",
  "description": "Adds example word-count utilities to Arclyra.",
  "author": "Example Studio",
  "website": "https://example.com/arclyra-wordtools",
  "capabilities": []
}
```

Build a sample plugin from this repository with:

```powershell
dotnet build SamplePlugins/Arclyra.PluginTemplate/Arclyra.PluginTemplate.csproj
```

## Documentation map

Read these pages for the API details that were split out of this root guide:

- [Terminology](terminology.md)
- [Capabilities](capabilities.md)
- [Story data API](story-data-api.md)
- [Prompt context API](prompt-context-api.md)
- [Smart Builder API](smart-builder-api.md)
- [AI generation API](ai-generation-api.md)
- [UI extension API](ui-extension-api.md)
- [Host services API](host-services-api.md)
- [Events API](events.md)
- [Lifecycle events](lifecycle-events.md)
- [Packaging](packaging.md)
- [Plugin signing tool](signtool.md)

## Runtime and lifecycle model

- Plugins are managed .NET assemblies loaded in-process into collectible plugin load contexts.
- Arclyra creates the plugin type named by `entryType`, calls `InitializeAsync(IPluginContext, CancellationToken)`, and later calls `ShutdownAsync(CancellationToken)` before unload, reload, uninstall, or replacement.
- Initialize quickly. Register commands/UI/services, then return. Move long-running work to cancellable plugin-owned tasks.
- Release long-lived resources in `ShutdownAsync`: event subscriptions, timers, background tasks, cancellation sources, WPF controls, delegates, static references, and unmanaged resources.
- Unload is best-effort. Any remaining strong reference to plugin-defined types can keep the assembly loaded until references are cleared or the process exits.

## Install, data, and package locations

Arclyra stores plugins in writable application data rather than under the app install directory.

| Distribution | Plugin install root | Plugin data root |
| --- | --- | --- |
| Classic installer / unpackaged app | `%LocalAppData%\Arclyra\Plugins` | `%LocalAppData%\Arclyra\PluginData` |
| MSIX / Store packaged app | `ApplicationData.Current.LocalFolder.Path\Plugins` | `ApplicationData.Current.LocalFolder.Path\PluginData` |

Each plugin is installed in a safe directory name derived from its manifest `id`, such as `%LocalAppData%\Arclyra\Plugins\com.example.arclyra.wordtools`. Plugin data should use a matching folder under `PluginData`.

Do not write plugins into `Program Files`, the executable folder, the MSIX package install directory, or `AppContext.BaseDirectory`. These locations may be read-only, replaced by updates, or unavailable for packaged apps.

## `.arcplugin` package format and validation

A `.arcplugin` is a ZIP archive. The archive root must contain:

- `plugin/plugin.json`;
- the plugin entry assembly named by `entryAssembly`;
- copy-local managed dependencies not supplied by Arclyra;
- assets and content files required at runtime;
- optionally, `plugin.package.json` at the archive root for package signature metadata.

Arclyra validates packages before installation. Packages are rejected when they:

- exceed 100 MB compressed;
- exceed 250 MB after extraction;
- contain more than 2,000 files;
- contain duplicate, absolute, invalid, traversal, or symbolic-link entries;
- include root entries other than `plugin/` and optional `plugin.package.json`;
- put `plugin.json` anywhere except `plugin/plugin.json`;
- declare invalid entry assembly/type paths;
- request reserved capabilities that are not accepted in production manifests.

See [Packaging](packaging.md) for complete packaging, validation, and distribution details.

## Manifest reference

`plugin.json` is a UTF-8 JSON object.

| Field | Required | Notes |
| --- | --- | --- |
| `id` | Yes | Stable plugin identifier. Reverse-DNS style is recommended. Used for install/data folder names, so avoid path separators and invalid Windows file-name characters. |
| `name` | Yes | Human-readable display name. |
| `version` | Yes | Parseable by `System.Version`, for example `1.0.0`. |
| `entryAssembly` | Yes | Relative path from the installed plugin folder to the plugin DLL. Absolute paths and paths escaping the plugin folder are rejected. |
| `entryType` | Yes | Fully qualified .NET type name implementing `Arclyra.PluginSdk.IArclyraPlugin`. |
| `minHostVersion` | Recommended | Minimum Arclyra host version, parseable by `System.Version`. |
| `description` | Optional | Short user-facing description. |
| `author` | Optional | Plugin author or organization. |
| `website` | Optional | Support, documentation, or project URL. |
| `license` | Optional | License metadata displayed during review/distribution. |
| `capabilities` | Recommended | Array of stable capability names requested by the plugin. Request only what you use. |

Legacy aliases may be accepted for older packages (`assembly`, `entryPoint`, and `minimumArclyraVersion`), but new plugins should use `entryAssembly`, `entryType`, and `minHostVersion`.

## Capabilities and security

> [!WARNING]
> Arclyra plugins are trusted code. They are .NET DLLs loaded in-process by Arclyra and are not sandboxed, isolated by permissions, or run in a separate security boundary. A plugin can execute arbitrary code with the same Windows user privileges as Arclyra, including reading and writing user-accessible files, starting processes, loading native code, using the network, and interacting with Arclyra process memory.

Capabilities are a host-facing declaration and a service gate for Arclyra SDK APIs. They help users and the host understand intended access, but they are not an operating-system sandbox. Follow least privilege, explain why each capability is needed, and avoid surprising background behavior.

The stable capability list, reserved capabilities, and guidance for least-privilege manifests live in [Capabilities](capabilities.md).

## Plugin signatures

Arclyra packages may include root-level `plugin.package.json` signature metadata for the contents of the `plugin/` folder. Signing is intended to let Arclyra associate a package with a known developer identity and detect package tampering.

If you wish to sign your plugins, contact Arclyra at **developers@arclyra.app**. We will provide the current signing requirements and developer onboarding details. Do not invent your own signature metadata format for public distribution.

For signing-tool usage, see [Plugin signing tool](signtool.md).

## Native dependencies and Store/MSIX considerations

Managed dependencies can usually live beside the plugin DLL. Native DLLs require extra caution:

- declare `nativeDependencies` in the manifest;
- match Arclyra's process architecture;
- test classic and MSIX/Store builds early;
- account for Windows packaged-app policy and code-integrity behavior;
- document prerequisites such as VC++ runtime components;
- prefer pure managed dependencies for Store-compatible plugins when possible.

## Compatibility and versioning guidance

- Keep plugin ids stable forever. Changing `id` makes Arclyra treat the package as a different plugin.
- Use semantic-ish `System.Version` values such as `1.2.3`.
- Set `minHostVersion` when you depend on newer SDK APIs or host behavior.
- Keep plugin-owned configuration ids stable so updates modify existing AI providers instead of adding duplicates.
- Treat host DTOs as snapshots. Re-read data when acting on stale UI or event context.
- Prefer additive changes to your own plugin data files to keep user data upgradeable.

## Developer checklist before distribution

- [ ] Manifest id, entry assembly, entry type, version, and minimum host version are correct.
- [ ] `IArclyraPlugin.Id` matches `plugin.json` `id`.
- [ ] The package does not include `Arclyra.PluginSdk.dll`.
- [ ] Capabilities are minimal and documented.
- [ ] Startup does not block the UI thread.
- [ ] Shutdown disposes subscriptions, background work, timers, and unmanaged resources.
- [ ] Package installs cleanly from the Plugins screen.
- [ ] Reload/uninstall/reinstall flows work.
- [ ] Plugin data is written only to a user-writable plugin-specific location.
- [ ] Browser automation scripts, network behavior, and native dependencies are disclosed to users.
- [ ] If signing is desired, you have contacted **developers@arclyra.app** for signing requirements.

## Validation command

For documentation-only changes, build the public SDK to catch signature drift that would make examples stale:

```bash
dotnet build src/Arclyra.PluginSdk/Arclyra.PluginSdk.csproj
```
