# Arclyra Plugin SDK

Current plugin documentation and sample plugins are maintained at [jdswcz/ArclyraPlugins](https://github.com/jdswcz/ArclyraPlugins).

`Arclyra.PluginSdk` contains the contracts Arclyra Writing Studio uses to discover, validate, load, and run plugins. Reference this package from a .NET plugin project, implement `IArclyraPlugin`, add a `plugin.json` manifest, and build/package the plugin as a `.arcplugin` archive.

Plugins can extend Arclyra with optional commands, menu items, settings pages, workspace panels, story UI integrations, Smart Builder tools, AI-provider configuration helpers, and host-mediated data workflows.

## Quick start

1. **Create a .NET 8 class library.** Use `net8.0-windows` when the plugin contributes WPF UI.
2. **Reference `Arclyra.PluginSdk`.** Do not copy `Arclyra.PluginSdk.dll` into the final package; Arclyra supplies the SDK assembly at runtime so host and plugin use the same contract types.
3. **Implement `IArclyraPlugin`.** Keep `Id` stable and make it match `plugin.json`.
4. **Declare a `plugin.json` manifest.** Include entry assembly/type, version, host compatibility, and the least set of capabilities your plugin needs.
5. **Build and stage files under a `plugin/` folder.** Include `plugin/plugin.json`, your entry DLL, non-host managed dependencies, and required assets.
6. **Zip the staging folder and rename it to `.arcplugin`.** The archive root must contain the `plugin` folder. It may also contain root-level `plugin.package.json` signature metadata.
7. **Install in Arclyra.** Open **Plugins**, choose **Install Plugin**, select the `.arcplugin`, and let Arclyra validate, install, and reload plugins.
8. **Update by installing a newer package.** Keep the same plugin `id`; use an increasing `version` value.

A minimal WPF-capable plugin project looks like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <ArclyraPluginPackageName>MyPlugin.arcplugin</ArclyraPluginPackageName>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Arclyra.PluginSdk" Version="0.1.0" />
    <None Include="plugin.json" CopyToOutputDirectory="PreserveNewest" />
  </ItemGroup>
</Project>
```

Repository samples can also reference the SDK project directly during local development:

```xml
<ProjectReference Include="..\Arclyra.PluginSdk\Arclyra.PluginSdk.csproj" Private="false" />
```

## Minimal plugin

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

## Manifest format

`plugin.json` is a UTF-8 JSON object. A minimal manifest looks like this:

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

| Field | Required | Notes |
| --- | --- | --- |
| `id` | Yes | Stable plugin identifier. Reverse-DNS style is recommended. Used for install/data folder names, so avoid path separators and invalid Windows file-name characters. |
| `name` | Yes | Human-readable display name. |
| `version` | Yes | Parseable by `System.Version`, for example `1.0.0`. |
| `entryAssembly` | Yes | Relative path from the installed plugin folder to the plugin DLL. Absolute paths and paths escaping the plugin folder are rejected. |
| `entryType` | Yes | Fully qualified .NET type name implementing `Arclyra.PluginSdk.IArclyraPlugin`. |
| `minHostVersion` | No | Minimum Arclyra host version, parseable by `System.Version`. |
| `description` | No | Short user-facing description. |
| `author` | No | Plugin author or organization. |
| `website` | No | Support, documentation, or project URL. |
| `capabilities` | No | Array of stable capability names requested by the plugin. Request only what you use. |

Legacy aliases may be accepted for older packages (`assembly`, `entryPoint`, and `minimumArclyraVersion`), but new plugins should use `entryAssembly`, `entryType`, and `minHostVersion`.

## Runtime and lifecycle model

Arclyra loads plugins from installed `.arcplugin` archives. At runtime the host:

1. Reads and validates `plugin.json`.
2. Checks `minHostVersion` and declared `capabilities`.
3. Loads `entryAssembly` into a collectible plugin load context and creates the `entryType` class.
4. Calls `InitializeAsync(IPluginContext, CancellationToken)`.
5. Keeps registered commands, UI, event subscriptions, AI providers, and Smart Builder hooks active while the plugin is enabled.
6. Calls `ShutdownAsync(CancellationToken)` before unload, reload, uninstall, replacement, or host exit.

Keep initialization quick. Register commands/UI/services, then return. Move long-running work to cancellable plugin-owned tasks. Release long-lived resources in `ShutdownAsync`, including event subscriptions, timers, background tasks, cancellation sources, WPF controls, delegates, static references, and unmanaged resources.

Unload is best-effort. Any remaining strong reference to plugin-defined types can keep the assembly loaded until references are cleared or the process exits.

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

## Capabilities and security

> [!WARNING]
> Arclyra plugins are trusted code. They are .NET DLLs loaded in-process by Arclyra and are not sandboxed, isolated by permissions, or run in a separate security boundary. A plugin can execute arbitrary code with the same Windows user privileges as Arclyra, including reading and writing user-accessible files, starting processes, loading native code, using the network, and interacting with Arclyra process memory.

Capabilities are a host-facing declaration and a service gate for Arclyra SDK APIs. They help users and the host understand intended access, but they are not an operating-system sandbox. Follow least privilege, explain why each capability is needed, and avoid surprising background behavior.

Stable capabilities include:

| Category | Capability | Enables |
| --- | --- | --- |
| Story data | `story.read` | Read story, chapter, character, item, custom entry, global entry, and generation-selection snapshots. |
| Story data | `story.write` | Add host-owned instructions/custom entries and write plugin metadata through explicit SDK methods. |
| Prompt context | `prompt.read` | Read Smart Builder prompt templates, prompt details, and rendered prompts. |
| Prompt context | `prompt.write` | Add/update/remove runtime and saved prompt details through explicit SDK methods; required with `prompt.read` for prompt transforms. |
| Settings | `settings.read` | Read plugin-safe user preference snapshots and receive settings-change events when event requirements are also met. |
| AI providers | `aiProviders.read` | Read AI provider configuration snapshots. |
| AI providers | `aiProviders.write` | Add, update, or remove AI provider configurations owned by the current plugin. |
| AI generation | `ai.generation` | Register a plugin-owned generator that receives rendered Smart Builder prompts and returns chapter drafts directly. |
| Events | `events.subscribe` | Subscribe to stable host events. |
| Events | `events.story` | Subscribe to story lifecycle events. |
| Events | `events.chapter` | Subscribe to chapter lifecycle/content events. |
| Events | `events.generatedDraft` | Subscribe to generated draft events. |
| Events | `events.promptDetail` | Subscribe to prompt-detail events. |
| Events | `events.aiProvider` | Subscribe to AI provider events. |
| Events | `events.transfer` | Subscribe to import/export completion events. |
| UI | `ui.settings` | Contribute settings pages. |
| UI | `ui.workspacePanel` | Contribute dockable or hosted workspace panels. |
| UI | `ui.smartBuilder` | Contribute Smart Builder toolbar actions, panels, below-prompt content, chapter actions, and row actions. |
| UI | `ui.editors` | Replace supported editing surfaces with plugin-provided editor controls. |
| UI | `ui.storyOverview` | Contribute story overview panels. |
| UI | `ui.storyListActions` | Add action buttons to story-list rows. |
| UI | `ui.dialogs` | Show simple host-owned dialogs from user-initiated plugin flows. |
| UI | `ui.newStoryWizard` | Add optional new-story wizard steps and completion contributions. |
| UI | `ui.entryManagement` | Contribute prompt-detail entry-management panels. |
| Files/native | `fileSystem.pluginDirectory` | Access the installed plugin directory path through `IPluginContext.PluginDirectory`. |
| Files/native | `nativeDependencies` | Declare that the plugin loads native/unmanaged dependencies from its install directory. |

`network` is reserved for future host-mediated network access. Production manifests cannot request it until the SDK exposes the corresponding service. A plugin can technically use .NET networking because it runs as trusted in-process code, but do not represent such use as host-mediated or capability-isolated.

## Categorized SDK API reference

The SDK namespace is `Arclyra.PluginSdk`.

### Direct AI generation

Plugins can bridge Arclyra to an AI backend without using the embedded Chromium browser. Declare the `ai.generation` capability, then register a generator with `IPluginAiProviderService.AddOrUpdateGenerator`. The registered provider appears in the AI generation provider selector. When selected, Arclyra shows a direct generation panel, sends the current story guide and rendered chapter prompt to the plugin, displays progress messages reported by the plugin, and applies the returned chapter draft to the generated-draft field.

```csharp
context.AiProviderService.AddOrUpdateGenerator(
    new PluginAiConfiguration(
        Id: "example-direct-ai",
        Name: "Example Direct AI",
        Url: string.Empty,
        PromptPasteScript: string.Empty,
        ReadGeneratedChapterScript: string.Empty,
        PluginOwnerId: null,
        PluginConfigurationId: "example-direct-ai"),
    async (request, progress, cancellationToken) =>
    {
        progress.Report(new PluginAiGenerationProgress("AI is generating a chapter draft…"));
        string draft = await GenerateDraftAsync(request.ChapterPrompt, cancellationToken);
        return new PluginAiGenerationResult(draft, "Generated chapter draft received.");
    });
```

### Lifecycle and context

- `IArclyraPlugin`: plugin entry contract with stable `Id`, display `Name`, `InitializeAsync`, and `ShutdownAsync`.
- `IPluginContext`: host metadata and services: `HostVersion`, `PluginDirectory`, `Logger`, `CommandRegistry`, `UiRegistry`, `DialogService`, `EventBus`, `AiProviderService`, `StoryDataService`, `PromptContextService`, `SmartBuilderService`, and `PreferencesService`.
- `IPluginLogger`: writes information, warning, and error messages through the host plugin log.

### Commands and menus

- `IPluginCommandRegistry.RegisterCommand(PluginCommandRegistration)`: registers an executable plugin command.
- `PluginCommandRegistration`: command id, display name, async execute callback, optional description.
- `IPluginUiRegistry.RegisterMenuItem(PluginMenuItemRegistration)`: places a registered command in a host menu path.
- `PluginMenuItemRegistration`: menu path, display name, command id, sort order, and optional tool tip.

Use globally unique command ids, preferably prefixed with your plugin id.

### General UI extensions

- `IPluginUiRegistry.RegisterSettingsPage(PluginSettingsPageRegistration)`: settings pages; requires `ui.settings`.
- `IPluginUiRegistry.RegisterPanel(PluginPanelRegistration)`: dockable/hosted panels; requires `ui.workspacePanel`.
- `IPluginUiRegistry.RegisterStoryOverviewPanel(PluginStoryOverviewPanelRegistration)`: story overview panels; requires `ui.storyOverview`.
- `IPluginUiRegistry.RegisterStoryListItemButton(PluginStoryListItemButtonRegistration)`: story-list row buttons; requires `ui.storyListActions`.
- `IPluginUiRegistry.RegisterNewStoryWizardStep(PluginNewStoryWizardStepRegistration)`: optional wizard steps; requires `ui.newStoryWizard`.
- `IPluginUiRegistry.RegisterEntryManagementPanel(PluginEntryManagementPanelRegistration)`: prompt-detail entry-management panels; requires `ui.entryManagement`.
- `IPluginEditorRegistry.RegisterEditor(PluginEditorRegistration)`: custom editors for `ChapterOutline`, `GeneratedChapterDraft`, `PromptDetail`, and `StoryOverview`; requires `ui.editors`.

Plugin UI is WPF `FrameworkElement` content. Keep it responsive, avoid blocking the UI thread, and match Arclyra's theme-aware WPF conventions where possible.

### Dialogs

`IPluginDialogService` requires `ui.dialogs` and provides:

- `ShowInfo`;
- `ShowConfirmation`;
- `ShowInput` returning `PluginDialogInputResult`;
- `ShowYesNoCancel` returning `PluginThreeButtonDialogResult`.

Use dialogs for user-initiated flows. Avoid showing modal UI during startup or from background event handlers.

### Story data and metadata

`IPluginStoryDataService` provides capability-gated snapshots and limited host-owned mutations:

- `GetStories`, `GetStory`;
- `GetGlobalInstructions`, `GetGlobalCharacters`, `GetGlobalItems`, `GetGlobalCustomEntries`;
- `AddInstruction`, `AddCustomEntry` with optional story id; requires `story.write`;
- `GetPluginMetadata`, `SetPluginMetadata` for plugin-owned key/value metadata.

DTOs include `PluginStoryDto`, `PluginChapterDto`, `PluginCharacterDto`, `PluginItemDto`, `PluginCustomEntryDto`, `PluginGenerationSelectionDto`, and template/scope enums. Treat DTOs as snapshots, not live objects.

### Prompt and Smart Builder APIs

`IPluginPromptContextService` provides:

- template reads: `GetChapterPromptTemplates`, `GetChapterPromptTemplate`;
- rendered prompt reads: `RenderChapterPrompt`, `GetPromptDetails`;
- runtime prompt details: `AddOrUpdateRuntimePromptDetail`, `RemoveRuntimePromptDetail`;
- saved prompt-detail mutations: `CreatePromptDetail`, `UpdatePromptDetail`, `SetPromptDetailEnabledForChapter`, `DeletePromptDetail`, `ReorderPromptDetails`.

`IPluginSmartBuilderService` provides:

- `RegisterToolbarAction`;
- `RegisterSidebarPanel`;
- `RegisterBelowPromptContent`;
- `RegisterChapterAction`;
- `RegisterPromptRowAction`;
- `RegisterPromptValidator`;
- `RegisterPromptTransform`.

Validators inspect composed prompt details and return `PluginPromptValidationMessage` values. Transforms receive `PluginPromptTransformContext` and return a `PluginPromptTransformResult`; transforms require both `prompt.read` and `prompt.write`.

### AI provider APIs

`IPluginAiProviderService` exposes:

- `ConfigurationsChanged` event;
- `GetConfigurations`; requires `aiProviders.read`;
- `AddOrUpdateConfiguration`; requires `aiProviders.write` and only creates/updates configurations owned by the current plugin;
- `RemovePluginConfiguration`; requires `aiProviders.write` and only removes configurations owned by the current plugin.

`PluginAiConfiguration` includes host id, display name, URL, prompt-paste script, generated-content readback script, plugin owner id, and stable plugin configuration id. Be transparent with users about browser automation scripts and external services.

### Preferences APIs

`IPluginPreferencesService.GetPreferences` requires `settings.read` and returns `PluginUserPreferencesDto` with appearance theme, large-text state, screenshot-prevention state, global settings layout preference, and workflow window preference.

### Events

`IPluginEventBus.Subscribe<TEvent>` requires `events.subscribe` and the event-family capability for the events you consume. Dispose the returned `PluginEventRegistration` during shutdown.

Stable event names are grouped as:

- story: `story.opened`, `story.closed`, `story.selected`, `story.created`;
- chapter: `chapter.created`, `chapter.duplicated`, `chapter.opened`, `chapter.closed`, `chapter.deleted`, `chapter.content.saved`, `chapter.outline.changed`, `chapter.content.changed`;
- generated drafts: `generatedDraft.added`, `generatedDraft.edited`, `generatedDraft.deleted`, `generatedDraft.exported`;
- prompt details: `promptDetail.added`, `promptDetail.edited`, `promptDetail.deleted`, `promptDetail.enabled`, `promptDetail.disabled`, `promptDetail.reordered`, `promptDetail.scoped`;
- AI providers: `aiProvider.added`, `aiProvider.changed`, `aiProvider.removed`;
- settings: `settings.changed`;
- transfer: `export.completed`, `import.completed`;
- editors: `editor.opened`, `editor.saved`, `editor.canceled`, `editor.validationError`.

Event DTOs include `PluginStoryEvent`, `PluginChapterEvent`, `PluginChapterContentEvent`, `PluginGeneratedDraftEvent`, `PluginPromptDetailEvent`, `PluginAiProviderEvent`, `PluginUserPreferencesChangedEvent`, `PluginTransferEvent`, and `PluginEditorEvent`.

## Packaging and signing workflow

The SDK ships MSBuild targets that package plugin projects automatically. When the SDK is consumed as a NuGet package, its `buildTransitive` target creates `$(TargetDir)$(ArclyraPluginPackageName)` after build.

Useful MSBuild properties:

| Property | Default | Purpose |
| --- | --- | --- |
| `ArclyraPluginPackageOnBuild` | `true` | Set to `false` to disable `.arcplugin` creation. |
| `ArclyraPluginPackageName` | `$(AssemblyName).arcplugin` | Output archive name. |
| `ArclyraPluginManifestFile` | `$(MSBuildProjectDirectory)\plugin.json` | Manifest copied to `plugin/plugin.json` in the archive. |
| `ArclyraPluginPowerShellExe` | `powershell` on Windows, `pwsh` elsewhere | PowerShell executable used by the pack script. |
| `ArclyraPluginPackageScript` | SDK pack script | Override only for advanced custom packaging. |
| `ArclyraPluginSignToolPath` | empty | Optional path to `Arclyra.PluginSignTool`; when set, the package script signs the archive after packaging. |
| `ArclyraPluginDeveloperName` | empty | Developer name passed to the sign tool. |
| `ArclyraPluginDeveloperPrivateKey` | empty | Developer private key PEM path used by the sign tool. |
| `ArclyraPluginDeveloperPublicKey` | empty | Developer public key PEM path embedded in `plugin.package.json`. |
| `ArclyraPluginDeveloperSignature` | empty | Master-generated developer signature path embedded in `plugin.package.json`. |

Recommended release workflow:

1. Build in Release mode.
2. Verify the `.arcplugin` archive contents and manifest metadata.
3. Generate a developer key pair with `Arclyra.PluginSignTool generate-developer-key --developer-name "Name" --private-key developer-private.pem --public-key developer-public.pem`, then submit only the public key to Arclyra to receive a master-generated developer signature.
4. Configure the signing properties above, or run `Arclyra.PluginSignTool sign-package` after packaging, to add the root `plugin.package.json` signature sidecar.
5. Authenticode-sign plugin assemblies if your distribution policy requires signed binaries.
6. Generate a checksum for the final `.arcplugin`.
7. Publish the archive, checksum, version, release notes, and support contact together.

If you wish to sign your plugins, contact Arclyra at **developers@arclyra.app** for current signing requirements and developer onboarding details. Do not invent your own signature metadata format for public distribution.

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

- [ ] Manifest id, entry assembly, entry type, version, and min host version are correct.
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
