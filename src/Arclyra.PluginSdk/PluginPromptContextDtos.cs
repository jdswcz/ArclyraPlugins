namespace Arclyra.PluginSdk;

/// <summary>
/// A read-only snapshot of guided chapter setup rendered prompt details.
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
/// A stable snapshot of a guided chapter setup prompt detail selection.
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

/// <summary>
/// A read-only snapshot of a persisted prompt detail's scope and chapter targeting.
/// </summary>
public sealed record PluginPromptDetailScopeDto(
    string Id,
    string StoryId,
    PluginSelectionScope Scope,
    IReadOnlyList<string> ChapterIds,
    bool IsChapterScoped,
    PluginChapterScopeMode ChapterScopeMode,
    int? ChapterStartNumber,
    int? ChapterEndNumber);

/// <summary>
/// Updates only the persisted prompt detail scope and chapter targeting.
/// </summary>
public sealed record PluginPromptDetailScopeUpdateRequest(
    PluginSelectionScope? Scope = null,
    IReadOnlyList<string>? ChapterIds = null,
    bool? IsChapterScoped = null,
    PluginChapterScopeMode? ChapterScopeMode = null,
    int? ChapterStartNumber = null,
    int? ChapterEndNumber = null);

/// <summary>
/// Updates chapter-specific prompt text for a persisted prompt detail.
/// Set <see cref="PromptText" /> to null to remove the customization.
/// </summary>
public sealed record PluginPromptDetailCustomizationRequest(
    string ChapterId,
    string? PromptText);

public sealed record PluginPromptDetailMutationResult(
    bool Success,
    PluginPromptDetailDto? PromptDetail,
    string? Message = null);

/// <summary>
/// A reusable prompt-entry version. The virtual Main version uses the stable id <c>main</c>.
/// </summary>
public sealed record PluginPromptEntryVersionDto(
    string Id,
    string Name,
    string PromptText,
    bool IsMain);

/// <summary>
/// Creates a named reusable alternative version for a prompt source entry.
/// </summary>
public sealed record PluginPromptEntryVersionCreateRequest(
    string Name,
    string PromptText);

/// <summary>
/// Updates a reusable alternative version. Null values leave the corresponding property unchanged.
/// </summary>
public sealed record PluginPromptEntryVersionUpdateRequest(
    string? Name = null,
    string? PromptText = null);

/// <summary>
/// Describes where a reusable version is selected by a persisted prompt detail.
/// </summary>
public sealed record PluginPromptEntryVersionUsageDto(
    string StoryId,
    string PromptDetailId,
    string? ChapterId,
    bool IsDefault);

public sealed record PluginPromptSourceEntryDto(
    string Id,
    PluginGenerationEntryType EntryType,
    PluginSelectionScope Scope,
    string? StoryId,
    string Content,
    string? Detail,
    string? Name,
    string? Category,
    IReadOnlyList<string> RequiredItemIds);

public sealed record PluginPromptSourceEntryCreateRequest(
    PluginGenerationEntryType EntryType,
    PluginSelectionScope Scope,
    string Content,
    string? StoryId = null,
    string? Detail = null,
    string? Name = null,
    string? Category = null,
    IReadOnlyList<string>? RequiredItemIds = null);

public sealed record PluginPromptSourceEntryUpdateRequest(
    string? Content = null,
    string? Detail = null,
    string? Name = null,
    string? Category = null,
    IReadOnlyList<string>? RequiredItemIds = null);

public sealed record PluginPromptSourceEntryMutationResult(
    bool Success,
    PluginPromptSourceEntryDto? SourceEntry,
    string? Message = null);

public enum PluginPromptRowSource
{
    TemplateText,
    PlaceholderTitle,
    ChapterPlotDescription,
    Entry
}
