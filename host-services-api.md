# Host services API

`IPluginContext` is the root object Arclyra passes to `IArclyraPlugin.InitializeAsync`. It exposes host metadata and capability-gated services. Keep a reference only while your plugin is running, and release subscriptions or UI resources during `ShutdownAsync`.

## Context properties

| Property | Capability | Purpose |
| --- | --- | --- |
| `HostVersion` | None | Current Arclyra host version for compatibility checks and diagnostics. |
| `PluginDirectory` | `fileSystem.pluginDirectory` | Full path to the installed plugin directory. Use it for packaged plugin assets, not for mutable user/plugin data. |
| `PluginDataDirectory` | None | Host-resolved writable directory dedicated to the current plugin. Use it for technical caches, downloaded components and non-sensitive plugin settings. Do not persist plaintext workspace/user content here. |
| `ProtectedStorageService` | None | Host-owned, plugin-isolated and workspace-scoped encrypted blob storage for prompts, generated text, feedback, memories, learned corrections and other user-content-derived data. Available in host 47+ / SDK 1.10. |
| `Logger` | None | Writes plugin diagnostics through Arclyra. |
| `CommandRegistry` | None | Registers executable commands that menu items and UI contributions can invoke. |
| `FirstPartyUiService` | Dedicated trusted signature + allowlisted identity only | Optional AI-only panel/settings registration and open/focus; null for ordinary plugins and older hosts. No unrestricted host access. |
| `UiRegistry` | UI-specific capabilities | Registers menus, panels, settings pages, editors, story UI contributions, wizard steps, entry-management panels, and AI paste actions. |
| `DialogService` | `ui.dialogs` | Shows host-owned informational, confirmation, text-input, and yes/no/cancel dialogs. |
| `EventBus` | `events.subscribe` plus event-domain capabilities | Subscribes to stable host events. See [Events API](events.md). |
| `AiProviderService` | `aiProviders.read`, `aiProviders.write`, or `ai.generation` | Reads and manages AI provider configurations, including plugin-owned direct generators. |
| `StoryDataService` | `story.read` or `story.write` | Reads story snapshots and performs explicit host-owned story mutations. |
| `PromptContextService` | `prompt.read`, `prompt.write`, `story.read`, or `story.write` depending on method | Reads rendered prompt context, manages prompt details, and manages prompt source entries. |
| `SmartBuilderService` | `ui.smartBuilder`, `prompt.read`, or `prompt.write` depending on registration | Registers guided chapter setup UI actions, validators, and transforms. |
| `PreferencesService` | `settings.read` | Reads plugin-safe user preference snapshots. |
| `LicenseService` | None | Legacy compatibility service. `IsPremiumActive` always returns `true`; new plugins must not use it for feature gating. |
| `AiGenerationWindowService` | `aiGeneration.browserAccess` for automation methods | Inspects and automates the AI generation window through audited host operations. |

## Protected workspace storage

Host 47+ exposes `IPluginContext.ProtectedStorageService`. The service is scoped to
the calling plugin and the currently open Arclyra data file. Each data file has a
stable opaque workspace id and a random workspace protection secret stored inside
that data file. The host derives isolated per-plugin/per-blob keys and persists
authenticated AES-GCM ciphertext. When the Arclyra data file is password-encrypted,
the workspace protection secret is itself inside that encrypted file; plugin code
never receives the password, the main data-file encryption key, or the workspace
secret.

Use a stable plugin-relative logical name:

```csharp
IPluginProtectedStorageService storage = context.ProtectedStorageService
    ?? throw new NotSupportedException("Host 47 or newer is required.");

byte[] payload = Encoding.UTF8.GetBytes(json);
try
{
    await storage.WriteAtomicAsync("learning/session.json", payload, cancellationToken);
}
finally
{
    CryptographicOperations.ZeroMemory(payload);
}

byte[]? restored = await storage.ReadAsync("learning/session.json", cancellationToken);
```

`WriteAtomicAsync` encrypts before opening its temporary destination, so plaintext
is never written to the protected blob or its atomic-write temporary file. Blob
authentication is bound to the plugin id, workspace id and logical name, preventing
a ciphertext copied to another plugin, workspace or name from authenticating there.
Switching workspaces changes the scope dynamically.

`PluginDataDirectory` is still the correct place for model/runtime downloads,
rebuildable caches and non-sensitive settings. Plugins that handle story text,
prompts, generated answers, correction examples, feedback, learned facts or similar
user content must not leave plaintext copies, diagnostic sidecars or crash-recovery
files there. The protected-storage API is a host confidentiality feature, not an OS
sandbox: Arclyra plugins remain trusted in-process code with the current Windows
user's permissions.

## Logging

```csharp
context.Logger.LogInformation("Plugin initialized.");
context.Logger.LogWarning("Optional configuration is missing.");
context.Logger.LogError("Failed to refresh plugin cache.", exception);
```

The logger accepts plain messages. If you need structured values, format them before passing them to the SDK logger.

## Dialogs

Capability: `ui.dialogs`.

SDK 1.18 / host 56 exposes optional read-only `IsSnapshotCurrent` on chapter-action
contexts. It validates an active captured document without calling/consuming a writer;
see [chapter editor actions](editor-actions.md#read-only-snapshot-preflight-sdk-118--host-56).

SDK 1.17 / host 55 additionally exposes a source-bound `CreateAlternativeVersion`
on eligible entry-action contexts, not a new global service. It uses existing
`ui.entryManagement` + `prompt.write` gates, explicit approval and the current
single-use editor lifetime; see [guarded creation](ui-extension-api.md#source-bound-alternative-creation-sdk-117--host-55).

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

`ShowTextReview` supports bounded full-text review (SDK 1.14 / host 52). SDK 1.16 /
host 54 optionally adds `OriginalText` for a host-owned read-only inline diff of the
current editable proposal, with explicit re-review after edits. Null keeps ordinary
review unchanged. See [full-text review](ui-extension-api.md#full-text-review-sdk-114--host-52).

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

## Legacy license state compatibility

`IPluginLicenseService` is obsolete and retained only so plugins compiled against older SDK versions continue to load. Arclyra no longer has a Premium tier, so `IsPremiumActive` always returns `true` and the host implementation does not contact a licensing service.

New or updated plugins must not use `LicenseService` to gate functionality. The interface may be removed in a future major Plugin SDK version.

```csharp
#pragma warning disable CS0618
bool legacyCompatibilityValue = context.LicenseService.IsPremiumActive; // Always true.
#pragma warning restore CS0618
```


## Structured task setup (SDK 1.22 / host 62)

`DialogService.ShowTextTaskSetup(request, cancellationToken)` uses `ui.dialogs` to collect a task choice, output language, additional instructions and complete request in one host-owned form. `PluginTextTaskSetupRequest` supplies `Title`, `Description`, `Choices` (`PluginTextTaskChoice` ID/name pairs), `DefaultTaskId`, `DefaultLanguage` and `ComposePrompt(taskId, language, notes)`. The result exposes `IsConfirmed`, `TaskId`, `Language` and `Prompt`. Cancel, close and older default implementations return an unconfirmed result; no request should be sent unless confirmed.

For long-running detail-to-browser workflows, host 62 also supplies optional `MarkPreparationComplete` and `SelectBrowserProviderAsync` on the entry action context. These do not grant a general browser opener or save callback. See [detail task preparation](ui-extension-api.md#detail-task-preparation-sdk-122--host-62).
