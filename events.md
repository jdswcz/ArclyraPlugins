# Events API

`IPluginContext.EventBus` lets plugins subscribe to stable host events through `IPluginEventBus`. Events are notifications only: DTOs are immutable snapshots and should not be treated as live host objects.

## Required capabilities

Every subscription requires `events.subscribe`. Event-domain capabilities communicate and gate the event families your plugin needs; request only the domains you use. `settings.changed` also requires `settings.read`.

| Event domain | Additional capability | Event names |
| --- | --- | --- |
| Stories | `events.story` | `story.opened`, `story.closed`, `story.selected`, `story.created` |
| Chapters | `events.chapter` | `chapter.created`, `chapter.duplicated`, `chapter.opened`, `chapter.closed`, `chapter.deleted`, `chapter.content.saved`, `chapter.outline.changed`, `chapter.content.changed` |
| Generated drafts | `events.generatedDraft` | `generatedDraft.added`, `generatedDraft.edited`, `generatedDraft.deleted`, `generatedDraft.exported` |
| Prompt details | `events.promptDetail` | `promptDetail.added`, `promptDetail.edited`, `promptDetail.deleted`, `promptDetail.enabled`, `promptDetail.disabled`, `promptDetail.reordered`, `promptDetail.scoped` |
| AI providers | `events.aiProvider` | `aiProvider.added`, `aiProvider.changed`, `aiProvider.removed` |
| Settings | `settings.read` | `settings.changed` |
| Transfers/import/export | `events.transfer` | `export.completed`, `import.completed` |
| Editors | `ui.editors` when contributing editors | `editor.opened`, `editor.saved`, `editor.canceled`, `editor.validationError` |
| AI generation window | `aiGeneration.browserAccess` when using browser/window automation | `aiGenerationWindow.opened`, `aiGenerationWindow.closed`, `aiGeneration.selectedProviderChanged` |

Use the constants in `PluginEventNames` instead of hard-coded strings.

## Subscribe and unsubscribe

`Subscribe<TEvent>` has async and synchronous overloads. Keep the returned `PluginEventRegistration` and dispose it during shutdown or when the plugin UI no longer needs updates.

```csharp
private PluginEventRegistration? _promptDetailSubscription;

public Task InitializeAsync(IPluginContext context, CancellationToken cancellationToken = default)
{
    _promptDetailSubscription = context.EventBus.Subscribe<PluginPromptDetailEvent>(
        PluginEventNames.PromptDetailAdded,
        async (eventContext, evt, handlerCancellationToken) =>
        {
            context.Logger.LogInformation(
                $"Prompt detail added to {evt.StoryName}: {evt.PromptDetailId}");
            await RefreshPromptCacheAsync(evt.StoryId, handlerCancellationToken);
        });

    return Task.CompletedTask;
}

public Task ShutdownAsync(CancellationToken cancellationToken = default)
{
    _promptDetailSubscription?.Dispose();
    _promptDetailSubscription = null;
    return Task.CompletedTask;
}
```

Synchronous handlers are useful for lightweight logging or invalidation:

```csharp
PluginEventRegistration registration = context.EventBus.Subscribe<PluginStoryEvent>(
    PluginEventNames.StorySelected,
    (eventContext, evt) => context.Logger.LogInformation($"Selected story: {evt.StoryName}"));
```

## Event context

Every handler receives `PluginEventContext` with:

| Property | Meaning |
| --- | --- |
| `EventName` | Stable event name published by Arclyra. |
| `EventId` | Unique id for this event publication. |
| `OccurredAt` | UTC timestamp for when the host published the event. |

Use the DTO `EventName` or context `EventName` when one handler is intentionally shared by multiple event names.

## Event DTOs

| DTO | Used for | Key fields |
| --- | --- | --- |
| `PluginStoryEvent` | Story open/close/select/create events. | `StoryId`, `StoryName` |
| `PluginChapterEvent` | Chapter lifecycle and outline events. | `StoryId`, `StoryName`, `ChapterId`, `ChapterNumber`, optional `SourceChapterId` for duplicates |
| `PluginChapterContentEvent` | Chapter content save/change events. | `StoryId`, `StoryName`, `ChapterId`, `ChapterNumber`, `ContentLength` |
| `PluginGeneratedDraftEvent` | Generated-draft add/edit/delete/export events. | `StoryId`, `StoryName`, `ChapterId`, `ChapterNumber`, `DraftIndex`, `ContentLength`, optional `ExportPath` |
| `PluginPromptDetailEvent` | Prompt detail add/edit/delete/enable/disable/reorder/scope events. | `StoryId`, `StoryName`, `PromptDetailId`, `EntryType`, `Content`, optional `Detail`, `Scope` |
| `PluginAiProviderEvent` | AI provider add/change/remove events. | `ConfigurationId`, `Name`, optional plugin ownership ids |
| `PluginUserPreferencesChangedEvent` | Host preference saves. | `Preferences` snapshot |
| `PluginTransferEvent` | Import/export completion events. | `StoryId`, `StoryName`, `Format`, optional `Path` |
| `PluginEditorEvent` | Editor open/save/cancel/validation-error events. | `StoryId`, `StoryName`, `Target`, `DocumentId`, optional chapter data, optional `ErrorMessage` |
| `PluginAiGenerationWindowEvent` | AI generation window open/close/provider-change events. | `IsWindowOpen`, optional `SelectedProvider` |

## Event-driven refresh pattern

Event DTOs intentionally carry enough metadata for filtering and logging, but not complete mutable models. Re-read the relevant snapshot through the appropriate service when your plugin needs current data.

```csharp
context.EventBus.Subscribe<PluginStoryEvent>(
    PluginEventNames.StorySelected,
    (eventContext, evt) =>
    {
        PluginStoryDto? currentStory = context.StoryDataService.GetStory(evt.StoryId);
        if (currentStory != null)
        {
            UpdatePluginPanel(currentStory);
        }
    });
```

The example above requires `events.subscribe`, `events.story`, and `story.read`.
