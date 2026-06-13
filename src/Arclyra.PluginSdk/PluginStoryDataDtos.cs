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
/// A read-only snapshot of the chapter prompt template settings used by Smart Builder.
/// </summary>
public sealed record PluginChapterPromptTemplateDto(
    string StoryId,
    string StoryName,
    string Template,
    string ChapterPlotTemplate,
    string ChapterPlotRowPrefix);

public enum PluginGenerationEntryType
{
    Item,
    Character,
    Instruction,
    StoryInformation,
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
