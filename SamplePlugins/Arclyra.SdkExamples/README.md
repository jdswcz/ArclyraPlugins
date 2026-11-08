# Compilable SDK recipes

[SdkUsageExamples.cs](SdkUsageExamples.cs) contains opt-in SDK **1.39.0** helpers, verified against read-only ArclyraWritingStudio **dev** commit `520e219a77c708a362142c323a42a3c57e3bfe1b`. It is a class library, not an installable plugin. Nothing registers or generates automatically; copy or call only the helpers your plugin needs. The project references the repository SDK source so recipes can be compiled before a NuGet release.

| Helper | Minimum host | Manifest capabilities | Caller responsibility |
| --- | --- | --- | --- |
| `ReadMemoryAsync` | 72.0 | `story.read` | Inspect `Status`; outdated text is for review, not current continuity. |
| `ReadSavedWorkingRevision` | 115.0 | `story.read` | Read `CurrentDraftContent` and structured `CurrentDraftId` from the same fresh chapter snapshot; null ID may mean legacy-only text. Recheck the source before relying on it. |
| `RegisterPublicKittyHelp` | 150.0 | **None** | Register public, non-sensitive static usage topics during plugin initialization. This does not expose chat, story context or inference access. |
| `RefreshMemoryAsync` | 72.0 | `story.read`, `ai.summarize` | Invoke after an explicit request; outdated author edits require approval in host UI. |
| `SummarizeAsync` | 70.0 | `ai.summarize` | Supply complete prose; recheck source freshness and review the returned proposal. |
| `ReviewPromptFields` | 66.0 | `ui.dialogs`, `prompt.read`, `ui.aiGenerationToolbar` | Use in an owned toolbar invocation. After confirmation, revalidate the website before inserting either field. Browser insertion also needs `aiGeneration.browserAccess`. |
| `RegisterTrimAction` | 71.0 | `ui.editorActions`, `ui.dialogs` | Call once in `InitializeAsync`. The author reviews a selection or the whole document; saving stays with the host/user. |
| `BeginAutomation` | 77.0 | `ui.aiGenerationToolbar`, `prompt.read`, `aiGeneration.browserAccess` | Acquire only in an explicit owned-provider toolbar action. Own and dispose the lease and observe all background tasks. |
| `CreateSummaryProgress` | 86.0 | `ai.summarize` (plus `story.read` for native memory) | Bind UI callbacks before inference; preview text is provisional and must never be stored/logged. |
| `RequireCurrentSources`, `BeginSourceBoundGeneration`, `VerifyCurrentSources`, `IncompletePreview` | 92.0 | `ai.generation` | Opt in on the generator runtime registration; dispose the linked CTS; recheck at boundaries; never commit an unapproved preview. Host 93 supplies restart decisions. |
| `ReportValidatedPromptReviewSnapshot` | 104.0 | **First-party trust only**, not a manifest capability | Optional typed progress for a verified `IPluginPromptReviewService` registered on host 102+. Report complete validated findings only. Ordinary plugins cannot register a reviewer. |

For memory, create `new PluginStoryMemoryRequest(storyId, BeforeChapterId: chapterId)` for preceding chapters, or supply both `ChapterId` and `DraftId` for one exact revision. On host 115+, `CurrentDraftId` from a fresh `GetStory` snapshot can identify a canonical working revision; do not assume a legacy-only draft has an ID. Do not combine these modes. Whole-story mode has no boundary. The default word ceiling is 500.

The browser helper only acquires a lease. Return from the initiating toolbar action before awaiting DOM work: the host waits for that invocation to finish. Keep the lease and worker task in plugin-owned state; use the **session** cancellation token for later operations, await each `RunFrameScriptAsync`/`PublishPreviewAsync` result and stop on rejection. Publish complete current-chapter staging, not the cumulative story, and await acceptance before the next DOM cycle. Dispose the lease on completion, explicit Stop and shutdown; await owned work during shutdown. Do not retain the invocation callbacks or use its token as the session lifetime. See [active browser automation](../../ui-extension-api.md#active-browser-automation-sdk-131--host-77).

## Current saved working revision (SDK 1.35 / host 115)

`ReadSavedWorkingRevision` selects a chapter from a fresh `GetStory` result and returns the pair (`CurrentDraftId`, `CurrentDraftContent`). The ID identifies the structured working or displayed revision; when the host falls back to a legacy text entry, the ID is null even when content exists. A revision ID can stay the same after edits, so it is **not** a freshness token. This is a host 115 feature; do not invoke the helper from a plugin whose minimum host is older. Unsaved editor text is not part of a story snapshot. See [story data API](../../story-data-api.md#snapshot-reads).

## Kitty public help (SDK 1.39 / host 150)

Call `RegisterPublicKittyHelp` once from `InitializeAsync` on host 150+, before returning. The SDK accepts up to 24 unique, plugin-local slugs (lowercase ASCII alphanumeric with hyphen separators), with titles up to 120, keywords up to 500, and help text up to 2,000 characters. Supply only static public usage instructions and exact visible UI labels. The registration is revoked on unload, disable or reload. It does not launch inference or authorize Kitty's navigation tools. The example fails clearly if the optional host registry is absent; the minimal template does not call it and therefore retains its earlier minimum host.

`IPluginAssistantChatService` (SDK 1.38 / host 140) and `IPluginArclyraAiOnboarding` (SDK 1.36/1.37 / host 134/136) are available only to the verified Arclyra AI first-party component. An ordinary sample cannot register them. See [host services](../../host-services-api.md) and [trusted components](../../trusted-components.md).

## Source-bound generation and prompt review limitations

SDK 1.33's `RequireCurrentSourceSnapshot` is **runtime-only** metadata: it must be set when registering a direct generator. A source-bound request supplies a cheap validity predicate and an invalidation token. Link that token to the handler cancellation token, dispose the linked source after the request, and check validity at stage boundaries and before the final result. `PluginAiGenerationIncompleteException` contains only an unapproved exact preview; do not treat it as a successful result or history turn.

SDK 1.34 adds request/target/suggestion/result DTOs and a trusted-only `IPluginPromptReviewService`. Host 104 optionally passes a progress object implementing both `IProgress<string>` and `IProgress<PluginPromptReviewResult>`. The recipe demonstrates **only** reporting an already validated immutable result and intentionally has no `RegisterPromptReviewer` call: ordinary plugins are never granted that host-verified first-party registration. Prompt review never grants direct data-write permission or approval for proposed text.

Build from the repository root on Windows with .NET 8:

```powershell
dotnet build SamplePlugins/Arclyra.SdkExamples/Arclyra.SdkExamples.csproj -c Release
```

For a Linux cross-build, add `-p:EnableWindowsTargeting=true`. That verifies compilation only; Windows WPF/CefSharp interactions and real local inference still need host validation. Recipes use no official plugin IDs, reserved UI placement or provider registration.
