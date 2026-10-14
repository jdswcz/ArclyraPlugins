# Chapter editor actions

SDK 1.6 adds an optional, additive action surface for Arclyra's built-in chapter editor. It does not replace the editor and does not expose a WPF text control, mutable Story object, or unrestricted save callback to the plugin.

## Capability and registration

Declare `ui.editorActions`, then register one or more menus through `IPluginContext.ChapterEditorActionService`. Each action declares whether it inserts at the captured caret or replaces the captured selection, whether it requires a non-empty selection, and an asynchronous execution delegate.

The host owns menu placement, enabled state, keyboard behavior and invocation. The built-in chapter editor shows active registrations in its themed toolbar, disables selection-only actions until text is selected, and permits only one running action per editor. Registration ids must be stable and unique within the plugin.

## Invocation snapshot

`PluginChapterEditorSnapshot` is immutable and contains bounded chapter text plus story, chapter, document and revision identity. Selection start/length, selected text and caret offset all refer to the same captured revision. Content and replacement text are each limited to 262,144 characters.

Treat the snapshot as stale immediately after capture. Do not assume the live document still matches it after asynchronous generation or review.

## Guarded apply

### Read-only snapshot preflight (SDK 1.18 / host 56)

The unchanged context constructor now has optional init property
`Func<bool>? IsSnapshotCurrent`. It checks the current host workspace reference,
story reference, registration, editor-close token and document/revision/content/
selection/caret without preparing a change or consuming `ApplyAsync`. The host
marshals capture to its dispatcher; completion/disposal clears the delegate so a
retained preflight returns false without touching the editor. Text edited and
reverted still has a different host revision. The callback is a preflight, not a
lease; check after every await/review and before any current-source delivery.

Null is the compatible legacy default. Read-only tools that depend on current
source validation must require host 56 and reject missing preflight or missing
cancellable invocation lifetime. Existing writer behavior is unchanged and still
revalidates independently; there is no new capability, broad data service or editor
control access. SDK assembly identity remains 1.2.0.0.

### Single-use writer

`PluginChapterEditorActionContext.ApplyAsync` accepts replacement text only. The target caret or selection is fixed by the registration and captured snapshot; the plugin cannot choose an arbitrary range. The callback is single-use even when the host rejects the proposal.

The host returns `PluginChapterEditorApplyResult` with one of these states:

- `Applied`;
- `StaleDocument`;
- `EditorClosed`;
- `PluginUnavailable`;
- `InvalidReplacement`;
- `AlreadyUsed`.

The host must revalidate document identity, revision, editor lifetime, plugin lifetime and the captured range immediately before changing text. A rejected proposal must leave the document unchanged. The host remains responsible for dirty tracking, undo/redo, word count and save behavior.

## Plugin behavior

Generate and review proposed text without editing the document. Let the writer inspect and optionally edit the proposal before calling `ApplyAsync`. Support cancellation and discard, handle every non-applied status, and release owned windows and work during plugin shutdown.
