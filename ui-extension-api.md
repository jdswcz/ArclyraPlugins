# UI extension API

`IPluginContext.UiRegistry` registers host UI contributions through `IPluginUiRegistry`. UI registrations should be user-initiated, theme-aware, and self-contained.

| Method | Capability | Purpose |
| --- | --- | --- |
| `RegisterMenuItem` | None beyond command/dialog needs | Add a plugin menu item that invokes a registered command. |
| `RegisterSettingsPage` | `ui.settings` | Add a settings page. |
| `RegisterPanel` / `OpenPanel` | `ui.workspacePanel` | Add and open workspace panels. |
| `RegisterStoryOverviewPanel` | `ui.storyOverview` | Add story overview UI. |
| `RegisterStoryListItemButton` | `ui.storyListActions` | Add buttons to story list rows. |
| `RegisterNewStoryWizardStep` | `ui.newStoryWizard` | Add optional new-story wizard steps. |
| `RegisterEntryManagementPanel` | `ui.entryManagement` | Add prompt setup UI. |
| `RegisterEntryManagementAction` | `ui.entryManagement` + `prompt.write` | Add a user-invoked action in Prompt setup’s AI tools menu beside Save. |
| `RegisterAiGenerationPasteAction` | `ui.aiGenerationPasteActions` | Add AI generation paste actions. |
| `RegisterAiGenerationToolbar` | `ui.aiGenerationToolbar` + `prompt.read` | Replace the entire top toolbar for an owned browser configuration. |
| `RegisterEditor` | `ui.editors` | Register custom editors through `IPluginEditorRegistry`. |

## Commands and menu items

```csharp
context.CommandRegistry.RegisterCommand(
    new PluginCommandRegistration(
        "example.openPanel",
        "Open Example Panel",
        cancellationToken =>
        {
            context.UiRegistry.OpenPanel("example.panel");
            return Task.CompletedTask;
        }));

context.UiRegistry.RegisterMenuItem(
    new PluginMenuItemRegistration("tools.examplePanel", "Example Panel", "example.openPanel"));
```

## Dialogs

Capability: `ui.dialogs`.

Use `IPluginContext.DialogService` for host-owned messages, confirmations, text prompts, and yes/no/cancel prompts instead of creating unmanaged modal windows.

## Settings pages and workspace panels

Capabilities: `ui.settings` and `ui.workspacePanel`.

```csharp
context.UiRegistry.RegisterSettingsPage(
    new PluginSettingsPageRegistration(
        "example.settings",
        "Example Plugin",
        () => new ExampleSettingsControl(),
        SortOrder: 100));

context.UiRegistry.RegisterPanel(
    new PluginPanelRegistration(
        "example.workspace",
        "Example Workspace",
        () => new ExampleWorkspaceControl(),
        DefaultDockLocation: "Right"));
```

Plugin WPF content should be self-contained and should prefer standard controls plus Arclyra `DynamicResource` references for theme-aware brushes and fonts. Arclyra wraps settings pages and panels in a host shell that provides the title, plugin owner metadata, and ambient theme resources.

## Story overview and story-list contributions

Capabilities: `ui.storyOverview` and `ui.storyListActions`.

```csharp
context.UiRegistry.RegisterStoryOverviewPanel(
    new PluginStoryOverviewPanelRegistration(
        "example.storyOverview",
        "Continuity Notes",
        overviewContext => new ContinuityNotesControl(overviewContext.CurrentStory)));

context.UiRegistry.RegisterStoryListItemButton(
    new PluginStoryListItemButtonRegistration(
        "example.auditStory",
        "Audit",
        async buttonContext => await AuditStoryAsync(buttonContext.Story, buttonContext.CancellationToken),
        ToolTip: "Run the plugin's story audit"));
```

`PluginStoryOverviewPanelContext.StoryOverview` is a host-mediated writer for built-in overview fields when the host supplies it. Do not retain the writer beyond the current UI interaction.

## New-story wizard steps

Capability: `ui.newStoryWizard`.

```csharp
context.UiRegistry.RegisterNewStoryWizardStep(
    new PluginNewStoryWizardStepRegistration(
        "example.toneStep",
        "Tone Notes",
        stepContext => new ToneStepControl(stepContext.Draft, stepContext.DraftEditor),
        ValidateAsync: validationContext => Task.FromResult(
            string.IsNullOrWhiteSpace(validationContext.Draft.Title)
                ? PluginWizardStepValidationResult.Failure("Enter a story title before applying plugin notes.")
                : PluginWizardStepValidationResult.Success),
        ApplyAsync: completionContext =>
        {
            completionContext.Contributions.AddWritingInstruction("Maintain a tense, lyrical tone.");
            return Task.CompletedTask;
        }));
```

Wizard content receives a draft snapshot and, when available, an `IPluginNewStoryWizardDraftEditor` for host-mediated changes to built-in draft fields. `ApplyAsync` runs after Arclyra creates the story and can add world details, writing guidance, and characters through `IPluginNewStoryWizardContributionWriter`.

## Entry-management panels

Capability: `ui.entryManagement`.

```csharp
context.UiRegistry.RegisterEntryManagementPanel(
    new PluginEntryManagementPanelRegistration(
        "example.entryInspector",
        "Entry Inspector",
        entryContext => new EntryInspectorControl(
            entryContext.SelectedStory,
            entryContext.ActiveEntryType,
            entryContext.PromptDetails)));
```

Entry-management panels are hosted in the prompt setup UI and receive the selected story, active entry type, and current prompt details.

Host 42 / SDK 1.5 also supports user-invoked prompt-detail actions beside the **Save** button. These actions require both `ui.entryManagement` and `prompt.write`:

```csharp
context.UiRegistry.RegisterEntryManagementAction(
    new PluginEntryManagementActionRegistration(
        "example.rewritePromptDetail",
        "Rewrite detail",
        async actionContext =>
        {
            string proposed = await BuildProposalAsync(
                actionContext.ActiveEntryType,
                actionContext.Name,
                actionContext.Description);

            // This replaces only the active editor field. It does not save the entry.
            actionContext.ApplyDescription(proposed);
        },
        ToolTip: "Prepare a replacement and insert it after user approval."));
```

`PluginEntryManagementActionContext` is a snapshot of the currently visible, possibly unsaved editor state. It contains the active entry type plus the applicable name, age, category, and description/info fields. `ApplyDescription` is a host-owned callback that replaces only the active Writing guidance, World detail, Character description, Item info, or Custom description field. Arclyra verifies that the same prompt detail is still active before applying the text, keeps normal dirty tracking, and does **not** save automatically. Plugins should obtain explicit user approval before replacing existing text and must not retain the callback after the interaction finishes.

Host 48 also hosts these registrations in Prompt setup **Versions**, below the version content. For an alternative version, `Description` contains its current unsaved full prompt text; `ApplyDescription` inserts approved text into that version editor, leaving **Save changes** to the writer. The callback is single-use and checks the entry/workspace, source/version identity, editor revision, version existence and concurrent persisted edits. Main is read-only in Versions, so actions there are disabled for Main. No SDK signature or additional capability is required.

### Long-running entry interactions (SDK 1.15 / host 53)

The unchanged positional context constructor now has optional init properties:
`CancellationToken` (default None), `CanApplyDescription` (default null on older
hosts) and `IsAlternativeVersion` (default false). Observe the token during every
await/review and check the preflight on the UI dispatcher before preparing or
applying a proposal. A true preflight is not a lease: `ApplyDescription` always
revalidates independently. Features requiring this complete lifetime must require
host 53 and fail closed if the preflight callback is absent.

Both Details and alternative Versions are single-use, bounded to 262,144 UTF-16
characters, and valid only while that action is still running. Empty/oversized,
duplicate and stale submissions cannot write. Changes to workspace/story/entry,
editor fields, tab/selection/revision, source properties, entry/version collections,
version properties, plugin registrations or editor unload invalidate the invocation.
Changed-and-reverted input does not revive it. Collection invalidation is deliberately
conservative: another entry/version addition may require restarting the interaction.
The host detaches source subscriptions on invalidation and when the action returns or throws. Keep
the action alive only for the explicit interaction, never retain its writer afterwards.

The approved write updates only the active unsaved field; its own change
notifications do not cancel a successful apply while it is being reported. Normal
Save / Save changes remains a separate author action. Main is still read-only in
Versions; no new capability or unrestricted story-data writer is introduced.

### Source-bound alternative creation (SDK 1.17 / host 55)

The unchanged entry-action context optionally supplies
`Func<string, string, PluginPromptEntryVersionDto> CreateAlternativeVersion`.
Only the name and approved full text are accepted; the host captures the actual
source object, workspace, story, registration and editor lifetime. Do not construct
opaque source ids or substitute a general prompt writer. Null means no supported
persisted source (new editor/older host); retain review/manual copy instead.
The existing `ui.entryManagement` + `prompt.write` gate covers this callback.

Call only after explicit destination/name/full-text approval. Names are trimmed,
1–120 UTF-16 characters (`MaximumAlternativeVersionNameLength`), not Main, and
unique against the captured source's latest alternatives using ordinal case-insensitive
trimmed comparison. Text is nonblank and at most 262,144 UTF-16 characters. The
host checks on its dispatcher and consumes the **same** invocation as
`ApplyDescription`, even for rejected input. Either operation prevents a second
creation or description replacement. Editor/source changes and unload invalidate
both; Main in Versions remains ineligible.

Creation appends exactly one new ID/text/name using existing host model tracking
and normal persistence/autosave. It never selects or assigns the new version,
changes primary Main fields, saves an existing unsaved editor, or overwrites/renames
old versions. The version picker refreshes its options while preserving current
selection, read-only state, editor revision and complete unsaved text. Existing
plugins/constructors and general version APIs remain unchanged; SDK assembly identity
stays 1.2.0.0. Require host 55 for this creation feature.

## Custom editors

Capability: `ui.editors`.

`IPluginUiRegistry` also implements `IPluginEditorRegistry`. Register editors when your plugin wants to replace or augment editing for specific Arclyra document targets.

```csharp
context.UiRegistry.RegisterEditor(
    new PluginEditorRegistration(
        "example.markdownDraftEditor",
        "Markdown Draft Editor",
        PluginEditorTarget.GeneratedChapterDraft,
        editorContext => new MarkdownDraftEditorControl(editorContext)));
```

Supported `PluginEditorTarget` values are `ChapterOutline`, `GeneratedChapterDraft`, `PromptDetail`, and `StoryOverview`. `PluginEditorContext` supplies story/chapter identity, a document id, title, editable content, metadata, and host callbacks:

- call `SaveAsync(newContent)` to save content through Arclyra;
- call `CancelAsync()` to close or cancel the edit;
- call `ValidateAsync(newContent)` when supplied to let the host validate before saving.

For an existing persisted `GeneratedChapterDraft`, Arclyra preserves the historical index-based `DocumentId` format for compatibility and additionally exposes stable identity through `Metadata`:
- `PluginEditorMetadataKeys.GeneratedDraftId` contains the canonical `ChapterDraft.ID` when available;
- `PluginEditorMetadataKeys.GeneratedDraftLegacyIndex` contains the legacy zero-based generated-content index.

New plugins should prefer the stable draft id for long-lived identity while retaining compatibility with contexts where it is not available, such as a not-yet-persisted generated draft.

## AI generation paste actions

Capability: `ui.aiGenerationPasteActions`.

```csharp
context.UiRegistry.RegisterAiGenerationPasteAction(
    new PluginAiGenerationPasteActionRegistration(
        "example.copyPromptSummary",
        "Copy summary",
        "Copy a compact prompt summary.",
        SortOrder: 10,
        async pasteContext => await CopySummaryAsync(
            pasteContext.SelectedProvider,
            pasteContext.CurrentRenderedPrompt,
            pasteContext.CancellationToken)));
```

Paste actions receive provider, story-guide, chapter-command, rendered-prompt, and cancellation snapshots. They do not receive direct browser access; use `IPluginAiGenerationWindowService` with `aiGeneration.browserAccess` for audited browser automation.

Custom editor lifecycle notifications are documented in the [Events API](events.md).

## Ordinary provider toolbar replacement (SDK 1.13 / host 51)

Register `PluginAiGenerationToolbarRegistration` with a stable toolbar id, owned
`PluginConfigurationId`, display name and at most eight ordered
`PluginAiGenerationToolbarAction` items. Each action has a unique id, label,
optional tooltip and `PluginAiGenerationPasteActionHandler`. Its invocation receives
the selected provider and current prepared guide/command plus a lifetime token.
Support cancellation and obtain separate approval for any insertion/import/write.

Arclyra snapshots the action list, creates themed controls on its dispatcher and
replaces the **whole** standard provider/paste toolbar only for that owner's
configuration at its configured HTTP(S) origin/path. User providers with the same
name or URL are ineligible. Direct generators and first-party placement are excluded.
The host always includes **Other providers**; navigation away, provider changes,
registration removal, window close or callback failure restore the standard bar,
cancel active tokens and remove handlers. A dismissed toolbar stays dismissed until
the provider changes. Late callbacks do not block a new toolbar's actions.

This API grants neither private host control access, Trusted Component placement,
browser automation nor data-write privileges. The additive default method on older
registries throws a clear `NotSupportedException`. Use host 51 as minimum compatibility.

## Dedicated first-party AI placement (SDK 1.12 / host 50)

`IPluginContext.FirstPartyUiService` is reserved for host-verified first-party
Trusted Components and is null for ordinary plugins.
It provides `RegisterArclyraAiPanel(Func<FrameworkElement>)` and
`OpenArclyraAiPanel()` for the single host-owned AI window. The factory runs on
the UI dispatcher and includes setup/settings in its content. No capability
string, display label, copied panel ID or ordinary signature obtains this
placement. Components using this placement may not register ordinary
settings/workspace panels.
Ordinary `UiRegistry` registrations and owner scoping remain unchanged.
The service expires on component unload; retained calls fail after disposal.

### Full-text review (SDK 1.14 / host 52)

`IPluginDialogService.ShowTextReview(PluginTextReviewRequest, token)` requires
`ui.dialogs`. The host owns a multiline editable/read-only surface with full text,
character count, confirm/cancel and scrollable field descriptions. Maximum length
is enforced before and after review (at most 262,144 characters). Cancellation
closes the review; older host implementations fail closed with an unconfirmed
result. It does not save data or transmit text by itself.

SDK 1.16 / host 54 adds optional `PluginTextReviewRequest.OriginalText` as an init
property; the positional constructor/deconstruction and SDK assembly identity are
unchanged. Null preserves ordinary review. A non-null original opts into a read-only
inline diff beside the editable proposal. Both original and proposal must fit the
request's MaximumLength independently; an oversized original fails before opening.
The host requires explicit **Review changes** before confirmation and clears that
review state after every edit, including a changed-and-reverted edit. Source text
remains immutable, diff markers never become returned proposal text, and the dialog
does not write or save data. Cancellation closes it using the existing lifetime.

Comparison is transient and lossless at Unicode grapheme boundaries. Matching and
visual-segment budgets bound work; if detailed matching is too expensive, complete
changed blocks appear with a coarse-matching notice rather than truncation. Added /
removed text has independent underline/bold / strike-through styles plus contrasting
backgrounds and explicit legend; changed whitespace uses display-only symbols.
No new capability, plugin-owned window, timer, storage or general data writer is
introduced. Features requiring comparison must require host 54; older hosts may
ignore the optional property, and default-interface implementations still fail closed.

## User-armed browser preview (SDK 1.19 / host 57)

`PluginAiGenerationPasteActionContext.StartLivePreview` is optional and supplied
only for a valid chapter target to loaded owners with `aiGeneration.browserAccess`.
Call it from the current user toolbar invocation after successful explicit prompt
insertion, passing a `PluginBrowserFrameScriptRequest` with a read-only script.
It arms one host-owned subscription in the existing visible browser. The script
returns `{ "ok": true, "text": "complete cumulative prose", "generating": true }`;
empty text leaves the current draft intact. The 262,144-character bound applies.
The legacy preview uses a 1.5-second timer (the host-63 chapter observer uses
250 ms), allows one read at a time, pauses during
user reviews, and checks ownership, visible frame/document, workspace, prompt and
chapter identity before and after each read. It rebases only its own transient
preview updates; intervening author edits invalidate the chapter snapshot.
Stop preview, navigation, toolbar restoration, owner unload and window close cancel
the subscription and release its captured target. It never saves a revision or
initiates generation; the existing reviewed import remains the save path.
Retained callbacks cannot arm a subscription after their user invocation ends.

## Compact prompt-detail actions and toolbar return (host 58)

The existing entry-management registrations appear as items in one **AI tools**
menu in Details and alternative Versions. Labels, tooltips and registration sort
order are retained; callbacks and source-bound guards are unchanged. Disabled or
busy editors close the menu and reject retained commands. Rebuilding/unloading
clears item handlers and captured delegates. Save remains a separate host action.

After **Other providers**, the standard toolbar offers **<provider name> tools**
for the same dismissed owned provider on its configured page. Return revalidates
a unique active toolbar registration, owner/configuration, provider id and page;
it clears dismissal and restores that toolbar without navigation or provider
mutation. Foreign providers/pages, removed registrations and closed hosts cannot
reactivate it. The SDK registration and capability contracts are unchanged.


## Chapter-sized browser preview and split review (SDK 1.20 / host 59)

`PluginBrowserFrameScriptRequest.ChapterPreviewTargetWords` is optional. Zero
preserves the complete single-chapter preview; values 200–10000 request automatic
length suggestions. The host keeps cumulative source text, publishes only the
currently growing segment to Smart Builder, prefers paragraph then sentence breaks,
and preserves every character across segments. It does not infer narrative boundaries
or completion. Choose a target appropriate for your integration.

`PluginAiGenerationPasteActionContext.ReviewAndImportAsChaptersAsync` is optional
and available only for a captured host chapter transaction. It opens a host-owned
split review with adjustable target and a complete per-segment reader. Saving is
single-use, pauses streaming, revalidates the exact website source and target after
review, saves the first segment as a new revision and inserts additional chapters
immediately after the selected chapter. Old revisions, final selections and subsequent
chapters are preserved. The first segment already saved in the target rejects the
batch to prevent duplicate chapters. A successful split selects its final chapter and
clears the consumed unsaved preview. Cancel changes nothing.

The `{ ok, text, generating }` read-only preview response also accepts optional
`waiting: true` with empty text. Waiting does not stop the subscription or replace the
existing preview. Invalid/ambiguous fields, navigation or stale context still stop it.


## Stored-chapter browser synchronization (SDK 1.21 / host 60)

An optional `PluginBrowserChapterSynchronization` on a frame request pins the overview
and provides a compare-and-replace script factory. Its `SynchronizeEdits` option requires
an initial full host diff review. The host captures chapter identity/topology separately
from the ordinary import lease, so author text edits can be tracked within that scope.
Matched chapters use current working prose plus bounded recent known revision candidates.
Only the selected mapped section is published to Smart Builder; unknown text is not
arbitrarily divided by length. New text after prior matches belongs to the selected new
chapter. Ambiguous matches and cross-chapter source changes reject.

Web and host baselines are independent. Host preview echoes are ignored; author changes
in the active draft and working stored prose become outgoing span replacements. Idle
state, exact full source/overview, current host snapshot, owner/capability, frame identity
and source acknowledgment are required. Generation defers outgoing changes. Both-side
chapter changes pause for a new explicit diff review. Site summaries within changed spans
are protected, and unmatched chapters are never implicitly shared. No polling operation
saves a revision, selects final prose or generates text.

`ReviewAndImportMappedChapterAsync` resolves the selected chapter section before the
ordinary single-use revision review. Unavailable or ambiguous mapping fails without a
full-story fallback. Existing text-task and null-option workflows remain compatible.

## Prompt-detail browser invocation (host 61)

A registered, user-invoked entry-management action may call the existing
`RunApprovedFrameScriptAsync` service while its captured Details/alternative
Versions editor is current and an already-visible manual browser and active toolbar
belong to that same plugin. `aiGeneration.browserAccess` is still required.
The host pins the window, provider, document revision and workspace at invocation;
the editor's cancellation/preflight rejects navigation, source changes and removal.
It never opens a browser, selects a foreign provider, falls back to another window
or grants a chapter import writer. Each operation links the action/service lifetimes
and rechecks the context before and after the native frame script. Concurrent
toolbar work blocks the detail handoff. Completion disposes the invocation and
releases editor/window delegates, including retained asynchronous callbacks.
No SDK signature or capability change is required; older hosts still require an
explicit toolbar action for frame scripts. Plugins using direct detail insertion
must require host 61 and provide explicit editable request approval themselves.

## Detail task preparation (SDK 1.22 / host 62)

`PluginEntryManagementActionContext.MarkPreparationComplete` is optional. Invoke it
only after a successfully approved request has been inserted. The host releases the
AI tools busy state while retaining the captured guarded writer during the action's
remaining asynchronous task. Starting another action cancels the previous source
lifetime; Cancel pending AI task is also available in the menu. An old action's late
completion cannot clear or dispose a newer action. Source changes, unload, removal,
window close and workspace changes retain the previous cancellation rules. Old
plugins that do not call this callback retain their original busy-until-completion
behavior. Existing context constructor and callbacks remain compatible.

`SelectBrowserProviderAsync(configurationId, token)` requires a loaded owner with
aiGeneration.browserAccess and is a captured-source callback,
not a general browser opener. Use it after explicit request approval. The host checks
the current owned user invocation and editor, selects only a registered manual
provider of that owner in the same already-open visible window, and waits at most
15 seconds for the unique visible direct native frame. Unrelated advertisement
loading is not a readiness condition. Provider/window/editor/workspace changes
reject the operation. Only this host-controlled selection can rebind the invocation
to the new provider/document; subsequent frame operations retain pre/post checks.
No Generate/Continue action or foreign-provider selection is granted.

`IPluginDialogService.ShowTextTaskSetup(request, token)` requires ui.dialogs and
displays task choices, output language, additional instructions and the full request
in one themed form. Choices and inputs are bounded; invalid composition disables
confirmation. The prompt callback is used only while this transient dialog is open.
Cancel/close/cancellation transmits nothing. Older hosts return an unconfirmed
result by the default interface implementation. Plugins requiring these surfaces
must declare host 62.

## Automatic browser observation and story loading (SDK 1.23 / host 63)

All additions are optional init properties; positional constructors and SDK assembly
identity remain unchanged. No new privilege/capability or general writer is introduced.

`PluginAiGenerationToolbarRegistration.PrepareObservationAsync` requests automatic
observation preparation only for a valid saved chapter on the owner's active manual
provider/page with browser access. The host supplies the existing scoped toolbar
context, serializes invocation, and arms only a transient preview. Preparation may
inspect output but must never initiate generation or save. Context/document changes
invalidate it. It pauses while captured editor interactions own the browser. A new
explicit insertion can establish a new baseline after a conflict/saved revision.

`UseMoreMenu` groups secondary actions and provider selection without navigation;
`PluginAiGenerationToolbarAction.IsSecondary` and optional `IsVisible` support
contextual actions. The host evaluates visibility on its dispatcher and detaches
all button/menu handlers on restoration. Default registrations preserve legacy placement.

`PluginAiGenerationPasteActionContext.LoadChaptersAsync` is supplied only for a
captured chapter and browser-access capability. `PluginBrowserStoryLoadRequest`
provides an inspect script, exact compare/replacement factory and preview factory.
The host owns revision policy, bounded complete-story preview, explicit confirmation,
source/topology/document guards, acknowledgment and recorded boundaries. It saves
no Arclyra data. Inspect returns `{ok,text,overview,generating}`; replacement acknowledges
`story` plus `overview`. Loading does not use the legacy chapter-matching heuristic. The host-63 loader
targets the owned provider's uniquely matched direct iframe `outputIframeEl`; its
page and frame must satisfy the existing approved-frame policy.
`ChapterGenerationUnavailableReason` restricts a loaded story to its last chapter.

A preview request can supply `BuildObservationScript(lastRevision)` for incremental
polling, `InitialBrowserStory` for exact loaded-boundary validation, and the host-reviewed
`PreviewPrefix` for continuing saved final-chapter prose. Preview envelopes optionally
include bounded integer `revision`, `words`, `target`, validated `phase` and boolean
`safeguard`, plus `cleared` for a verified deliberate empty-tail correction. Unchanged envelopes return `waiting:true,text:""`; this never clears staging.
The host runs one native read at a time on a 250 ms chapter timer, preserves separate
incoming proposals on author conflicts, and saves no revisions. Browser observers must
have finite leases and must not perform unattended generation. Observer timers may sample
values; only a current host poll may request the standard website Stop action.

`PluginChapterEditorActionContext.SelectBrowserProviderAsync` extends the existing
source-bound entry-management selection to chapter actions. The host establishes the
same browser invocation scope and revalidates document/revision/selection/workspace,
owner, provider and window across awaits. It opens no new/global browser and grants
neither generation nor persistence. Automatic task acquisition still passes through
existing explicit proposal/diff/application callbacks.
