namespace Arclyra.PluginSdk;

/// <summary>
/// Stable host event names published by Arclyra.
/// </summary>
public static class PluginEventNames
{
    public const string StoryOpened = "story.opened";
    public const string StoryClosed = "story.closed";
    public const string StorySelected = "story.selected";
    public const string StoryCreated = "story.created";

    public const string ChapterCreated = "chapter.created";
    public const string ChapterDuplicated = "chapter.duplicated";
    public const string ChapterOpened = "chapter.opened";
    public const string ChapterClosed = "chapter.closed";
    public const string ChapterDeleted = "chapter.deleted";
    public const string ChapterContentSaved = "chapter.content.saved";
    public const string ChapterOutlineChanged = "chapter.outline.changed";
    public const string ChapterContentChanged = "chapter.content.changed";

    public const string GeneratedDraftAdded = "generatedDraft.added";
    public const string GeneratedDraftEdited = "generatedDraft.edited";
    public const string GeneratedDraftDeleted = "generatedDraft.deleted";
    public const string GeneratedDraftExported = "generatedDraft.exported";

    public const string PromptDetailAdded = "promptDetail.added";
    public const string PromptDetailEdited = "promptDetail.edited";
    public const string PromptDetailDeleted = "promptDetail.deleted";
    public const string PromptDetailEnabled = "promptDetail.enabled";
    public const string PromptDetailDisabled = "promptDetail.disabled";
    public const string PromptDetailReordered = "promptDetail.reordered";
    public const string PromptDetailScoped = "promptDetail.scoped";

    public const string AiProviderAdded = "aiProvider.added";
    public const string AiProviderChanged = "aiProvider.changed";
    public const string AiProviderRemoved = "aiProvider.removed";

    /// <summary>
    /// Published when host user preferences are saved. Subscriptions require both events.subscribe and settings.read.
    /// </summary>
    public const string SettingsChanged = "settings.changed";

    public const string ExportCompleted = "export.completed";
    public const string ImportCompleted = "import.completed";

    public const string EditorOpened = "editor.opened";
    public const string EditorSaved = "editor.saved";
    public const string EditorCanceled = "editor.canceled";
    public const string EditorValidationError = "editor.validationError";
}

/// <summary>
/// Host-provided metadata passed with every plugin event.
/// </summary>
/// <param name="EventName">The stable event name.</param>
/// <param name="EventId">A unique id for this publication.</param>
/// <param name="OccurredAt">The UTC time at which the host published the event.</param>
public sealed record PluginEventContext(string EventName, string EventId, DateTimeOffset OccurredAt);

/// <summary>
/// Base type for stable Arclyra event DTOs.
/// </summary>
public abstract record PluginEventDto(string EventName);

public sealed record PluginStoryEvent(
    string EventName,
    string StoryId,
    string StoryName) : PluginEventDto(EventName);

public sealed record PluginChapterEvent(
    string EventName,
    string StoryId,
    string StoryName,
    string ChapterId,
    int ChapterNumber,
    string? SourceChapterId = null) : PluginEventDto(EventName);

public sealed record PluginChapterContentEvent(
    string EventName,
    string StoryId,
    string StoryName,
    string ChapterId,
    int ChapterNumber,
    int ContentLength) : PluginEventDto(EventName);

public sealed record PluginGeneratedDraftEvent(
    string EventName,
    string StoryId,
    string StoryName,
    string ChapterId,
    int ChapterNumber,
    int DraftIndex,
    int ContentLength,
    string? ExportPath = null) : PluginEventDto(EventName);

public sealed record PluginPromptDetailEvent(
    string EventName,
    string StoryId,
    string StoryName,
    string PromptDetailId,
    string EntryType,
    string Content,
    string? Detail,
    string Scope) : PluginEventDto(EventName);

public sealed record PluginAiProviderEvent(
    string EventName,
    string ConfigurationId,
    string Name,
    string? PluginOwnerId,
    string? PluginConfigurationId) : PluginEventDto(EventName);

/// <summary>
/// Published when the host user preference file is saved.
/// </summary>
/// <param name="EventName">The stable event name.</param>
/// <param name="Preferences">A read-only snapshot of plugin-safe host preferences.</param>
public sealed record PluginUserPreferencesChangedEvent(
    string EventName,
    PluginUserPreferencesDto Preferences) : PluginEventDto(EventName);

public sealed record PluginTransferEvent(
    string EventName,
    string StoryId,
    string StoryName,
    string Format,
    string? Path) : PluginEventDto(EventName);

public sealed record PluginEditorEvent(
    string EventName,
    string StoryId,
    string StoryName,
    PluginEditorTarget Target,
    string DocumentId,
    string? ChapterId,
    int? ChapterNumber,
    string? ErrorMessage = null) : PluginEventDto(EventName);
