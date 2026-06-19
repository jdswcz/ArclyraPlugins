namespace Arclyra.PluginSdk;

/// <summary>
/// A read-only snapshot of an Arclyra story.
/// </summary>
public sealed record PluginStoryDto(
    string Id,
    string Name,
    string? BaseStoryInfo,
    IReadOnlyList<string> Instructions,
    IReadOnlyList<PluginChapterDto> Chapters,
    IReadOnlyList<PluginCharacterDto> Characters,
    IReadOnlyList<PluginItemDto> Items,
    IReadOnlyList<PluginCustomEntryDto> CustomEntries,
    IReadOnlyList<string> StoryInformation,
    IReadOnlyList<PluginGenerationSelectionDto> GenerationSelections,
    PluginChapterPromptTemplateDto ChapterPromptTemplate);

/// <summary>
/// A read-only snapshot of a chapter.
/// </summary>
public sealed record PluginChapterDto(
    string Id,
    int Number,
    string Content,
    string ContentPreview,
    IReadOnlyList<string> GeneratedContent);

/// <summary>
/// A read-only snapshot of a character entry.
/// </summary>
public sealed record PluginCharacterDto(
    string Id,
    string Name,
    string? Age,
    string? Description,
    string? CharacterTemplate,
    PluginSelectionScope Scope,
    string? StoryId);

/// <summary>
/// A read-only snapshot of an item entry.
/// </summary>
public sealed record PluginItemDto(
    string Id,
    string Name,
    string? ItemInfo,
    IReadOnlyList<string> RequiredItemIds,
    PluginSelectionScope Scope,
    string? StoryId);

/// <summary>
/// A read-only snapshot of a custom entry definition.
/// </summary>
public sealed record PluginCustomEntryDto(
    string Id,
    string Category,
    string? Description,
    PluginSelectionScope Scope,
    string? StoryId);

/// <summary>
/// A read-only snapshot of a generation selection entry.
/// </summary>
public sealed record PluginGenerationSelectionDto(
    string Id,
    PluginGenerationEntryType EntryType,
    string Content,
    string? Detail,
    PluginSelectionScope Scope,
    IReadOnlyList<string> ChapterIds,
    bool IsChapterScoped,
    PluginChapterScopeMode ChapterScopeMode,
    int? ChapterStartNumber,
    int? ChapterEndNumber,
    IReadOnlyList<PluginChapterEntryCustomizationDto> ChapterCustomizations);

/// <summary>
/// A read-only snapshot of chapter-specific selection prompt text.
/// </summary>
public sealed record PluginChapterEntryCustomizationDto(string ChapterId, string PromptText);

/// <summary>
/// A read-only snapshot of the chapter prompt template settings used by guided chapter setup.
/// </summary>
public sealed record PluginChapterPromptTemplateDto(
    string StoryId,
    string StoryName,
    string Template,
    string ChapterPlotTemplate,
    string ChapterPlotRowPrefix);

/// <summary>
/// Identifies the SDK/API source category for a prompt detail. Use Arclyra's writer-facing terminology, such as "World details" and "Writing guidance", in plugin UI text.
/// </summary>
public enum PluginGenerationEntryType
{
    /// <summary>Item prompt details.</summary>
    Item,

    /// <summary>Character prompt details.</summary>
    Character,

    /// <summary>Writing guidance prompt details. Kept as <c>Instruction</c> for SDK compatibility.</summary>
    Instruction,

    /// <summary>World details prompt details. Kept as <c>StoryInformation</c> for SDK compatibility.</summary>
    StoryInformation,

    /// <summary>Custom prompt details.</summary>
    Custom
}

public enum PluginSelectionScope
{
    Unspecified,
    Global,
    Story
}

public enum PluginChapterScopeMode
{
    SelectedChapters,
    SinceChapter,
    UntilChapter,
    ChapterRange
}

/// <summary>
/// A read-only snapshot of a story world-detail entry.
/// </summary>
public sealed record PluginWorldDetailDto(string Id, string StoryId, string Content);

/// <summary>
/// A read-only snapshot of a generated chapter draft.
/// </summary>
public sealed record PluginGeneratedDraftDto(string StoryId, string ChapterId, int DraftIndex, string Content);

public sealed record PluginCreateStoryRequest(string Name, string? BaseStoryInfo = null);

public sealed record PluginUpdateStoryRequest(string StoryId, string Name, string? BaseStoryInfo = null);

public sealed record PluginCreateChapterRequest(string StoryId, string Content = "", IReadOnlyList<string>? GeneratedContent = null);

public sealed record PluginUpdateChapterRequest(string StoryId, string ChapterId, string? Content = null, IReadOnlyList<string>? GeneratedContent = null);

public sealed record PluginCreateCharacterRequest(
    string Name,
    string? Age = null,
    string? Description = null,
    string? CharacterTemplate = null,
    string? StoryId = null);

public sealed record PluginUpdateCharacterRequest(
    string CharacterId,
    string? Name = null,
    string? Age = null,
    string? Description = null,
    string? CharacterTemplate = null,
    string? StoryId = null);

public sealed record PluginCreateItemRequest(
    string Name,
    string? ItemInfo = null,
    IReadOnlyList<string>? RequiredItemIds = null,
    string? StoryId = null);

public sealed record PluginUpdateItemRequest(
    string ItemId,
    string? Name = null,
    string? ItemInfo = null,
    IReadOnlyList<string>? RequiredItemIds = null,
    string? StoryId = null);

public sealed record PluginCreateWorldDetailRequest(string StoryId, string Content);

public sealed record PluginUpdateWorldDetailRequest(string StoryId, string WorldDetailId, string Content);

public sealed record PluginCreateCustomEntryRequest(string Category, string? Description = null, string? StoryId = null);

public sealed record PluginUpdateCustomEntryRequest(string CustomEntryId, string? Category = null, string? Description = null, string? StoryId = null);

public sealed record PluginScopedEntryRequest(string EntryId, string? StoryId = null);

public sealed record PluginCreateGeneratedDraftRequest(string StoryId, string ChapterId, string Content);

public sealed record PluginUpdateGeneratedDraftRequest(string StoryId, string ChapterId, int DraftIndex, string Content);
