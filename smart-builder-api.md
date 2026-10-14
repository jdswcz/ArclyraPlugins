# Smart Builder API

## Approved draft revisions (SDK 1.9 / host 46+)

Draft-action contexts now optionally provide `CancellationToken`,
`CanApplyRevision` and `ApplyRevision`. Older positional constructors and service
implementations remain compatible. The token is cancelled when the workspace
unloads or the action completes. A running action uses the same generation busy
state as Make manual edits/browser/plugin generation while retaining its original
completed-draft provenance. Nested actions and new generation are blocked.

Before committing plugin learning, check `CanApplyRevision()` on the UI thread.
After the writer explicitly reviews and accepts the complete answer, call
`ApplyRevision(approvedOutput)` on that same dispatcher. The callback is single-use,
bounded, rejects missing/foreign provenance, edits/navigation/unload/cancellation
and unavailable registrations, and records the revised draft's original provider
and source prompt. It updates the current Smart Builder draft only; it does not
write character/item/story-guide data or saved chapter manuscript versions.
`false` means preserve newer author content and keep the accepted proposal
available through the plugin's review/audit UI. Null callbacks mean the host does
not support approved revisions; do not fabricate direct host-data writes.

## Draft action menus (SDK 1.4 / host 41+)

`IPluginContext.SmartBuilderService` also implements the optional
`IPluginSmartBuilderDraftActionService`. Register a
`PluginSmartBuilderDraftActionMenuRegistration` with a stable menu id, display
name and ordered `PluginSmartBuilderDraftActionRegistration` items. The existing
`ui.smartBuilder` capability gates this contribution; no data-write capability
is granted. Existing service implementations and registrations remain unchanged.

The host places these menus immediately before **Make manual edits**. Buttons
and open menu items follow its `AllowSave` generation lock, and commands recheck
generation state and registration ownership at item click. Plugin unload removes
the contribution and stale menu commands cannot execute it.

`PluginSmartBuilderDraftActionContext` contains the current story/chapter identity
and an optional `CompletedDraft`. Its transient snapshot identifies the plugin,
configuration, exact sent chapter prompt, completed answer and completion time.
It exists only after successful direct plugin generation and matching application
to the current workspace. New generation, failure/cancellation, browser readback,
manual changes and navigation invalidate it. Prompt changes after completion do
not replace the actual sent request. Plugins must check owner/configuration before
using a draft; null provenance should produce their own helpful message. Menus
never imply permission to overwrite prose or persist a correction.

`IPluginContext.SmartBuilderService` registers guided chapter setup UI contributions, row actions, validators, and transforms through `IPluginSmartBuilderService`.

## Capabilities

- `ui.smartBuilder` for toolbar actions, sidebar panels, below-prompt content, chapter actions, and prompt row actions.
- `prompt.read` for prompt validators.
- `prompt.read` and `prompt.write` for prompt transforms.

## Registration points

```csharp
context.SmartBuilderService.RegisterToolbarAction(
    new PluginSmartBuilderToolbarRegistration(
        "example.refresh",
        "Refresh plugin hints",
        async actionContext => await RefreshHintsAsync(actionContext)));

context.SmartBuilderService.RegisterPromptRowAction(
    new PluginPromptRowActionRegistration(
        "example.inspectRow",
        "Inspect row",
        async rowContext => await InspectRowAsync(rowContext.Row)));
```

Available registration methods are `RegisterToolbarAction`, `RegisterSidebarPanel`, `RegisterBelowPromptContent`, `RegisterChapterAction`, `RegisterPromptRowAction`, `RegisterPromptValidator`, and `RegisterPromptTransform`.

## Validators and transforms

Validators inspect the composed prompt and return `PluginPromptValidationMessage` values.

```csharp
context.SmartBuilderService.RegisterPromptValidator(
    new PluginPromptValidatorRegistration(
        "example.minimumLength",
        "Minimum prompt length",
        transformContext => transformContext.Prompt.RenderedText.Length < 200
            ? [new PluginPromptValidationMessage(
                PluginPromptValidationSeverity.Warning,
                "Prompt is short; consider adding more story context.")]
            : []));
```

Transforms receive `PluginPromptTransformContext` and return `PluginPromptTransformResult`. Use transforms sparingly because they can alter generated-command text.
