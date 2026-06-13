namespace Arclyra.PluginSdk;

/// <summary>
/// A read-only snapshot of Smart Builder rendered prompt details.
/// </summary>
public sealed record PluginRenderedPromptDetailsDto(
    string StoryId,
    string StoryName,
    string? ChapterId,
    int? ChapterNumber,
    string RenderedText,
    int InstructionCount,
    int ItemCount,
    int StoryInformationCount,
    int CharacterCount,
    int CustomCount,
    IReadOnlyList<PluginRenderedPromptRowDto> Rows);

/// <summary>
/// A read-only snapshot of a rendered prompt row.
/// </summary>
public sealed record PluginRenderedPromptRowDto(
    string RenderedText,
    PluginPromptRowSource Source,
    string? PlaceholderName,
    PluginGenerationEntryType? EntryType,
    string? RenderedTitle,
    string? TemplatePlaceholderToken,
    string? PlaceholderTitle,
    IReadOnlyList<string> ChapterPlotRows);

/// <summary>
/// A stable snapshot of a Smart Builder prompt detail selection.
/// </summary>
public sealed record PluginPromptDetailDto(
    string Id,
    string StoryId,
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

public sealed record PluginPromptDetailCreateRequest(
    PluginGenerationEntryType EntryType,
    string Content,
    string? Detail = null,
    PluginSelectionScope Scope = PluginSelectionScope.Story,
    IReadOnlyList<string>? ChapterIds = null,
    bool IsChapterScoped = false,
    PluginChapterScopeMode ChapterScopeMode = PluginChapterScopeMode.SelectedChapters,
    int? ChapterStartNumber = null,
    int? ChapterEndNumber = null);

public sealed record PluginPromptDetailUpdateRequest(
    string? Content = null,
    string? Detail = null,
    PluginSelectionScope? Scope = null,
    IReadOnlyList<string>? ChapterIds = null,
    bool? IsChapterScoped = null,
    PluginChapterScopeMode? ChapterScopeMode = null,
    int? ChapterStartNumber = null,
    int? ChapterEndNumber = null);

public enum PluginPromptRowSource
{
    TemplateText,
    PlaceholderTitle,
    ChapterPlotDescription,
    Entry
}
