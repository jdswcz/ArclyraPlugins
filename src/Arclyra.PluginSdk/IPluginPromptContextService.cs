namespace Arclyra.PluginSdk;

/// <summary>
/// Provides capability-gated access to chapter prompt templates and rendered Smart Builder prompt context.
/// </summary>
public interface IPluginPromptContextService
{
    /// <summary>
    /// Gets chapter prompt template snapshots for all stories.
    /// Requires the <see cref="PluginCapabilities.PromptRead" /> capability.
    /// </summary>
    IReadOnlyList<PluginChapterPromptTemplateDto> GetChapterPromptTemplates();

    /// <summary>
    /// Gets a chapter prompt template snapshot for one story.
    /// Requires the <see cref="PluginCapabilities.PromptRead" /> capability.
    /// </summary>
    PluginChapterPromptTemplateDto? GetChapterPromptTemplate(string storyId);

    /// <summary>
    /// Renders Smart Builder prompt details for a story/chapter context.
    /// Requires the <see cref="PluginCapabilities.PromptRead" /> capability.
    /// </summary>
    PluginRenderedPromptDetailsDto RenderChapterPrompt(string storyId, string? chapterId = null, string? chapterContentOverride = null);

    /// <summary>
    /// Lists Smart Builder prompt details for a story, optionally filtered by chapter and scope.
    /// Requires the <see cref="PluginCapabilities.PromptRead" /> capability.
    /// </summary>
    IReadOnlyList<PluginPromptDetailDto> GetPromptDetails(string storyId, string? chapterId = null, PluginSelectionScope? scope = null);

    /// <summary>
    /// Adds or updates a runtime prompt detail contributed by the current plugin. Runtime prompt details are included in rendered prompts but are not saved to the story data file.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailDto AddOrUpdateRuntimePromptDetail(string storyId, PluginPromptDetailCreateRequest request, string? runtimePromptDetailId = null);

    /// <summary>
    /// Removes a runtime prompt detail contributed by the current plugin.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    bool RemoveRuntimePromptDetail(string runtimePromptDetailId);

    /// <summary>
    /// Creates a Smart Builder prompt detail for a story.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailDto CreatePromptDetail(string storyId, PluginPromptDetailCreateRequest request);

    /// <summary>
    /// Updates Smart Builder prompt detail content, detail, scope, and chapter usage.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailDto UpdatePromptDetail(string storyId, string promptDetailId, PluginPromptDetailUpdateRequest request);

    /// <summary>
    /// Enables or disables a prompt detail for a chapter.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailDto SetPromptDetailEnabledForChapter(string storyId, string promptDetailId, string chapterId, bool enabled);

    /// <summary>
    /// Deletes a Smart Builder prompt detail from a story.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    bool DeletePromptDetail(string storyId, string promptDetailId);

    /// <summary>
    /// Reorders prompt details within a single entry type.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    IReadOnlyList<PluginPromptDetailDto> ReorderPromptDetails(string storyId, PluginGenerationEntryType entryType, IReadOnlyList<string> orderedPromptDetailIds);
}
