# Host services API

`IPluginContext` is the root object Arclyra passes to `IArclyraPlugin.InitializeAsync`. It exposes host metadata and capability-gated services. Keep a reference only while your plugin is running, and release subscriptions or UI resources during `ShutdownAsync`.

## Context properties

| Property | Capability | Purpose |
| --- | --- | --- |
| `HostVersion` | None | Current Arclyra host version for compatibility checks and diagnostics. |
| `PluginDirectory` | `fileSystem.pluginDirectory` | Full path to the installed plugin directory. Use it for packaged plugin assets, not for user data unless your plugin explicitly owns the files. |
| `Logger` | None | Writes plugin diagnostics through Arclyra. |
| `CommandRegistry` | None | Registers executable commands that menu items and UI contributions can invoke. |
| `UiRegistry` | UI-specific capabilities | Registers menus, panels, settings pages, editors, story UI contributions, wizard steps, entry-management panels, and AI paste actions. |
| `DialogService` | `ui.dialogs` | Shows host-owned informational, confirmation, text-input, and yes/no/cancel dialogs. |
| `EventBus` | `events.subscribe` plus event-domain capabilities | Subscribes to stable host events. See [Events API](events.md). |
| `AiProviderService` | `aiProviders.read`, `aiProviders.write`, or `ai.generation` | Reads and manages AI provider configurations, including plugin-owned direct generators. |
| `StoryDataService` | `story.read` or `story.write` | Reads story snapshots and performs explicit host-owned story mutations. |
| `PromptContextService` | `prompt.read`, `prompt.write`, `story.read`, or `story.write` depending on method | Reads rendered prompt context, manages prompt details, and manages prompt source entries. |
| `SmartBuilderService` | `ui.smartBuilder`, `prompt.read`, or `prompt.write` depending on registration | Registers guided chapter setup UI actions, validators, and transforms. |
| `PreferencesService` | `settings.read` | Reads plugin-safe user preference snapshots. |
| `LicenseService` | None | Reads coarse premium/unlimited access state. |
| `AiGenerationWindowService` | `aiGeneration.browserAccess` for automation methods | Inspects and automates the AI generation window through audited host operations. |

## Logging

```csharp
context.Logger.LogInformation("Plugin initialized.");
context.Logger.LogWarning("Optional configuration is missing.");
context.Logger.LogError("Failed to refresh plugin cache.", exception);
```

The logger accepts plain messages. If you need structured values, format them before passing them to the SDK logger.

## Dialogs

Capability: `ui.dialogs`.

```csharp
context.DialogService.ShowInfo("Example", "The action completed.");

bool confirmed = context.DialogService.ShowConfirmation(
    "Delete generated draft?",
    "This removes the selected draft from the story.",
    confirmText: "Delete",
    cancelText: "Keep",
    isDestructive: true);

PluginDialogInputResult input = context.DialogService.ShowInput(
    "Rename label",
    "Enter the new label:",
    defaultValue: "Scene goal");

PluginThreeButtonDialogResult choice = context.DialogService.ShowYesNoCancel(
    "Save changes?",
    "Apply the plugin changes before closing?");
```

Use host dialogs instead of unmanaged modal windows so prompts stay themed and owned by Arclyra.

## Preferences

Capability: `settings.read`.

```csharp
PluginUserPreferencesDto preferences = context.PreferencesService.GetPreferences();

if (preferences.UseLargeText)
{
    context.Logger.LogInformation("Large text is enabled.");
}
```

The snapshot includes `AppearanceTheme`, `UseLargeText`, `PreventScreenshots`, `UseFullWidthGlobalSettings`, and `WorkflowWindowPreference`. Subscribe to [`PluginEventNames.SettingsChanged`](events.md) with `PluginUserPreferencesChangedEvent` when your plugin needs to refresh UI after preferences are saved.

## License state

`IPluginLicenseService` exposes only `IsPremiumActive`. Trial access is treated as active. The service intentionally does not expose serial numbers, license owner data, expiration dates, or activation operations.

```csharp
if (context.LicenseService.IsPremiumActive)
{
    context.Logger.LogInformation("Premium host features are active.");
}
```
