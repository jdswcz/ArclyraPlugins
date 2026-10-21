# Compilable SDK recipes

[SdkUsageExamples.cs](SdkUsageExamples.cs) contains opt-in helpers for ordinary plugins against SDK **1.31.0**, synchronized from ArclyraWritingStudio **master**. It is a class library, not an installable plugin. Nothing registers or generates automatically; copy or call only the helpers your plugin needs. The project references the repository SDK source so recipes can be compiled before a NuGet release.

| Helper | Minimum host | Manifest capabilities | Caller responsibility |
| --- | --- | --- | --- |
| `ReadMemoryAsync` | 72.0 | `story.read` | Inspect `Status`; outdated text is for review, not current continuity. |
| `RefreshMemoryAsync` | 72.0 | `story.read`, `ai.summarize` | Invoke after an explicit request; outdated author edits require approval in host UI. |
| `SummarizeAsync` | 70.0 | `ai.summarize` | Supply complete prose; recheck source freshness and review the returned proposal. |
| `ReviewPromptFields` | 66.0 | `ui.dialogs`, `prompt.read`, `ui.aiGenerationToolbar` | Use in an owned toolbar invocation. After confirmation, revalidate the website before inserting either field. Browser insertion also needs `aiGeneration.browserAccess`. |
| `RegisterTrimAction` | 71.0 | `ui.editorActions`, `ui.dialogs` | Call once in `InitializeAsync`. The author reviews a selection or the whole document; saving stays with the host/user. |
| `BeginAutomation` | 77.0 | `ui.aiGenerationToolbar`, `prompt.read`, `aiGeneration.browserAccess` | Acquire only in an explicit owned-provider toolbar action. Own and dispose the lease and observe all background tasks. |

For memory, create `new PluginStoryMemoryRequest(storyId, BeforeChapterId: chapterId)` for preceding chapters, or supply both `ChapterId` and `DraftId` for one exact revision. Do not combine these modes. Whole-story mode has no boundary. The default word ceiling is 500.

The browser helper only acquires a lease. Return from the initiating toolbar action before awaiting DOM work: the host waits for that invocation to finish. Keep the lease and worker task in plugin-owned state; use the **session** cancellation token for later operations, await each `RunFrameScriptAsync`/`PublishPreviewAsync` result and stop on rejection. Publish complete current-chapter staging, not the cumulative story, and await acceptance before the next DOM cycle. Dispose the lease on completion, explicit Stop and shutdown; await owned work during shutdown. Do not retain the invocation callbacks or use its token as the session lifetime. See [active browser automation](../../ui-extension-api.md#active-browser-automation-sdk-131--host-77).

Build from the repository root on Windows with .NET 8:

```powershell
dotnet build SamplePlugins/Arclyra.SdkExamples/Arclyra.SdkExamples.csproj -c Release
```

For a Linux cross-build, add `-p:EnableWindowsTargeting=true`. That verifies compilation only; Windows WPF/CefSharp interactions and real local inference still need host validation. Recipes use no official plugin IDs, reserved UI placement or provider registration.
