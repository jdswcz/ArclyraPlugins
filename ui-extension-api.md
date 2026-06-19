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
| `RegisterAiGenerationPasteAction` | `ui.aiGenerationPasteActions` | Add AI generation paste actions. |
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
