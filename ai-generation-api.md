# AI generation API

Plugins can integrate with AI generation in three ways: direct generation handlers, paste actions, and audited browser automation.

SDK 1.13 / host 51 browser providers can set `ManualBrowserWorkflow=true`.
It disables host automatic content capture and `TriggerGenerationAsync`; the
author must use the visible website. The default is false for compatibility.
This is runtime provider metadata, not a mutation of the user's unrelated providers.
For ordinary scoped top-toolbar replacement, see [UI extension API](ui-extension-api.md).

## Direct AI generation

Capability: `ai.generation`.

```csharp
context.AiProviderService.AddOrUpdateGenerator(
    new PluginAiConfiguration(
        Id: "com.example.generator",
        Name: "Example Generator",
        Url: "https://example.invalid",
        PromptPasteScript: string.Empty,
        ReadGeneratedChapterScript: string.Empty,
        PluginConfigurationId: "example.generator"),
    async (request, progress, cancellationToken) =>
    {
        progress.Report(new PluginAiGenerationProgress("Generating draft…"));
        string draft = await GenerateDraftAsync(request.ChapterPrompt, cancellationToken);
        return new PluginAiGenerationResult(draft, "Draft generated.");
    });
```

## AI provider configurations

Capabilities: `aiProviders.read` for snapshots and `aiProviders.write` for plugin-owned configuration mutations.

`IPluginContext.AiProviderService` exposes the current provider list, raises `ConfigurationsChanged` when provider settings change, and lets a plugin add, update, or remove only the configurations it owns. Host/user-owned configurations can be read with `aiProviders.read` but not removed by plugins.

```csharp
IReadOnlyList<PluginAiConfiguration> providers =
    context.AiProviderService.GetConfigurations();

PluginAiConfiguration saved = context.AiProviderService.AddOrUpdateConfiguration(
    new PluginAiConfiguration(
        Id: string.Empty,
        Name: "Example Browser Provider",
        Url: "https://example.invalid",
        PromptPasteScript: "/* host-approved paste script */",
        ReadGeneratedChapterScript: "/* host-approved read script */",
        PluginConfigurationId: "example.browserProvider"));

bool removed = context.AiProviderService.RemovePluginConfiguration(saved.Id);
```

Use `PluginAiConfiguration.PluginConfigurationId` as your stable provider id. Arclyra populates `Id` and `PluginOwnerId` when it persists the plugin-owned configuration.

## Paste actions

Capability: `ui.aiGenerationPasteActions`.

```csharp
context.UiRegistry.RegisterAiGenerationPasteAction(
    new PluginAiGenerationPasteActionRegistration(
        "example.copyPromptSummary",
        "Copy prompt summary",
        "Copies a short summary of the current rendered prompt.",
        0,
        async pasteContext => await CopySummaryAsync(pasteContext.CurrentRenderedPrompt, pasteContext.CancellationToken)));
```

The paste-action context exposes provider/story/prompt snapshots and a cancellation token. It does not expose the live browser.

## Browser automation

Capability: `aiGeneration.browserAccess`.

Use `IPluginContext.AiGenerationWindowService` for audited, host-mediated operations such as `IsWindowOpen`, `SelectedProvider`, `ReadSelectedProviderAsync`, `PasteCurrentPromptAsync`, `TriggerGenerationAsync`, `ReadGeneratedContentAsync`, and `RunApprovedScriptAsync`.

AI provider and AI generation window notifications are documented in the [Events API](events.md).


## Story conversation transactions (SDK 1.1)

The host supplies stable story/chapter IDs and an optional `request.Conversation`.
Call `PluginAiConversation.GenerateAsync` with a scope containing the profile,
endpoint and model, and a transport callback that receives the ordered messages.
The callback must return only a completed result; progress previews are not history.
The shared implementation handles atomic success commits, guide invalidation,
concurrent requests, persistence and trimming. See [conversation contracts](conversations.md).

## Separate thinking progress (SDK 1.2 / host 39)

Use the four-argument `PluginAiGenerationProgress(status, isGenerating, draft, thinkingTrace)`
constructor for cumulative thinking snapshots. Pass a null draft while only reasoning
is available. Null trace means no update; an empty trace clears the preview. Existing
two/three-argument constructors remain available. `PluginAiThinkingTrace` bounds
provider-supplied reasoning to 131,072 characters with a truncation notice while
allowing generation to continue. Never merge reasoning into the generated chapter
or conversation turns. The host provides a separate collapsible panel and Stop
action for direct generation; honor the supplied cancellation token.

## Transient generation actions (SDK 1.11 / host 49)

A direct generator may attach short-lived actions to a progress update through
`PluginAiGenerationProgress.UserActions`. Use these only for operations owned by the
currently running generation, such as interrupting one internal reasoning stage without
cancelling the whole request.

```csharp
progress.Report(new PluginAiGenerationProgress(
    "Planning scene…",
    true,
    null,
    currentThinking)
{
    UserActions =
    [
        new PluginAiGenerationUserAction(
            "example.retryStage",
            "Retry this step",
            token => RetryCurrentStageAsync(token),
            "Interrupt and retry only the current planning step.")
    ]
});
```

`UserActions = null` leaves the current actions unchanged, while an empty collection
clears them. A new non-null collection replaces the previous set. The host also clears
actions automatically when direct generation ends. The callback receives the
whole-generation cancellation token and must still stop if the user invokes the normal
**Stop generation** action. Persistent plugin commands belong in the ordinary UI
extension APIs instead.


## One-time generation menus (SDK 1.8 / host 45)

A direct generator may set `PluginAiConfiguration.GenerationMenu` to a
`PluginAiGenerationMenu` containing a display name and unique named
`PluginAiGenerationOption` choices. The host shows it beside the normal generator
item under Generate with AI → Use plugin. The normal item supplies
`request.GenerationOptionId = null`; submenu items supply their opaque choice ID
for that request only. Providers without this metadata are unchanged.

These are runtime registrations removed on plugin unload, not saved provider
configuration or user preferences. The host snapshots the menu, checks ownership
and current registration on invocation, and rejects unavailable choices. The
plugin resolves null to its saved default and applies an explicit choice without
persisting it. All choices use the same registered generation handler.


## Scoped manual browser frames (SDK 1.14 / host 52)

`RunApprovedFrameScriptAsync(PluginBrowserFrameScriptRequest, token)` requires
`aiGeneration.browserAccess`, an active user toolbar invocation, the caller's
owned manual provider/configuration and a uniquely identified visible direct
iframe. The host checks exact configured main origin/path, frame source/native
identity and allowed HTTPS subdomain/path before and after execution. Navigation,
provider/context/workspace changes, window close and service disposal invalidate
operations. Pass serialized data, never concatenate manuscript into JavaScript.

Toolbar action snapshots optionally include `SharedFieldLabels`,
`ImportTargetDescription` and `ReviewAndImportAsNewDraftAsync`. The latter accepts
full source text and an async source-verification callback. It is single-use,
opens a full host-owned result preview, requires explicit Save new revision,
revalidates source and target, then appends through existing draft storage.
`RequiresChapterTarget` disables actions without a saved chapter. The callback
preserves previous/final revisions and unsaved preview, rejects duplicates and
bounds result text to 262,144 characters. Older hosts provide no import callback.

## Manual-browser automatic acquisition (SDK 1.23 / host 63)

ManualBrowserWorkflow continues to forbid host-generated website inference. An owned
toolbar may now prepare automatic observation and full-story loading through the
optional [host-owned browser contracts](ui-extension-api.md#automatic-browser-observation-and-story-loading-sdk-123--host-63).
Acquisition never implies acceptance, revision saving or canon changes. Other plugins
keep their existing provider/generator contracts and toolbar defaults.

## Source-bound generation (SDK 1.33 / host 92)

A direct-generator runtime registration may set
`PluginAiConfiguration.RequireCurrentSourceSnapshot = true`. This flag is not a
persisted provider setting; default false preserves existing providers. The host requires a current story/chapter for the lease; legacy anonymous
requests receive no lease, so engines requiring one must fail closed.

The host supplies `request.IsSourceSnapshotCurrent` and
`request.SourceSnapshotInvalidationToken`. The predicate reads only an atomic
validity flag and workspace epoch, and is safe in streaming callbacks. Link the
invalidation token with caller cancellation for runtime preparation and inference.
The lease is terminal and expires when the handler exits. Never retain it for a
later request, treat it as author approval or read mutable host collections from
inference callbacks.

Observable active-story/global source edits, chapter/revision changes, reorder,
deletion and workspace switching invalidate/cancel the lease. Source graph access
and exact fingerprint checks happen on the dispatcher at capture and before
conversation access/commit and result return, including legacy non-notifying data
changes. No fingerprint or inference runs on every keystroke. Conversation-state
writes, derived summaries and story display preferences are excluded. Existing
capability checks still apply; the lease exposes no content or write capability.

A provider may throw `PluginAiGenerationIncompleteException(message, exactPreview,
wasCanceled, innerException)` for a bounded, explicitly unapproved partial draft.
This is an exception, never a `PluginAiGenerationResult`. Conversation generation
propagates it without committing a turn. Smart Builder and the integrated generator
window show the partial preview and status without applying it to a chapter or
marking it as a completed response. Its text must fit the existing conversation
content bound. Providers should distinguish cancellation from a failed generation
for truthful diagnostics; no hidden restart, partial approval or cross-beat rewrite
is authorized.

Host 93 asks for an explicit decision before replacing an interrupted preview with
a new direct generation. Keep preview (also the default and Escape choice) retains
the exact editable text and starts no generation. Discard and restart creates a fresh
whole-chapter request from current sources; it never resumes the interrupted beat,
approves partial text or inserts it into conversation history. Both host surfaces
recheck the source, cancellation and exact preview across the modal decision.
The integrated window retains interrupted previews when changing providers or
clearing conversation history. Normal completed results keep their existing flow.

```csharp
if (request.IsSourceSnapshotCurrent is not { } current || !current())
    throw new InvalidOperationException("A current host source snapshot is required.");
using var linked = CancellationTokenSource.CreateLinkedTokenSource(
    cancellationToken, request.SourceSnapshotInvalidationToken);
// Recheck current() at stage/stream boundaries, and return completed prose only.
```
