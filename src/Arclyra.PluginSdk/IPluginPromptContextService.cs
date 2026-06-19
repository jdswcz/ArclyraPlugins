namespace Arclyra.PluginSdk;

/// <summary>
/// Provides capability-gated access to chapter prompt templates and rendered guided chapter setup prompt context.
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
    /// Renders guided chapter setup prompt details for a story/chapter context.
    /// Requires the <see cref="PluginCapabilities.PromptRead" /> capability.
    /// </summary>
    PluginRenderedPromptDetailsDto RenderChapterPrompt(string storyId, string? chapterId = null, string? chapterContentOverride = null);

    /// <summary>
    /// Lists guided chapter setup prompt details for a story, optionally filtered by chapter and scope. Includes persisted and runtime plugin details for backward compatibility.
    /// Requires the <see cref="PluginCapabilities.PromptRead" /> capability.
    /// </summary>
    IReadOnlyList<PluginPromptDetailDto> GetPromptDetails(string storyId, string? chapterId = null, PluginSelectionScope? scope = null);

    /// <summary>
    /// Lists persisted guided chapter setup prompt details stored in Story.GenerationSelections.
    /// Requires the <see cref="PluginCapabilities.PromptRead" /> capability.
    /// </summary>
    IReadOnlyList<PluginPromptDetailDto> GetPersistedPromptDetails(string storyId, string? chapterId = null, PluginSelectionScope? scope = null);

    /// <summary>
    /// Lists runtime prompt details stored through PluginRuntimeContributions.
    /// Requires the <see cref="PluginCapabilities.PromptRead" /> capability.
    /// </summary>
    IReadOnlyList<PluginPromptDetailDto> GetRuntimePromptDetails(string storyId, string? chapterId = null, PluginSelectionScope? scope = null);

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
    /// Creates a guided chapter setup prompt detail for a story.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailDto CreatePromptDetail(string storyId, PluginPromptDetailCreateRequest request);

    /// <summary>
    /// Updates guided chapter setup prompt detail content, detail, scope, and chapter usage.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailDto UpdatePromptDetail(string storyId, string promptDetailId, PluginPromptDetailUpdateRequest request);

    /// <summary>
    /// Gets only persisted prompt detail scope/chapter targeting.
    /// Requires the <see cref="PluginCapabilities.PromptRead" /> capability.
    /// </summary>
    PluginPromptDetailScopeDto GetPromptDetailScope(string storyId, string promptDetailId);

    /// <summary>
    /// Updates only persisted prompt detail scope/chapter targeting.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailMutationResult UpdatePromptDetailScope(string storyId, string promptDetailId, PluginPromptDetailScopeUpdateRequest request);

    /// <summary>
    /// Updates only persisted prompt detail content/detail text.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailMutationResult UpdatePersistedPromptDetailText(string storyId, string promptDetailId, PluginPromptDetailUpdateRequest request);

    /// <summary>
    /// Updates only persisted prompt detail scope/chapter targeting.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailMutationResult UpdatePersistedPromptDetailScope(string storyId, string promptDetailId, PluginPromptDetailScopeUpdateRequest request);

    /// <summary>
    /// Adds, updates, or removes chapter-specific prompt text for a persisted prompt detail.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailMutationResult SetPersistedPromptDetailCustomization(string storyId, string promptDetailId, PluginPromptDetailCustomizationRequest request);

    /// <summary>
    /// Adds or updates chapter-specific prompt text for a persisted prompt detail.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailMutationResult SetPromptDetailChapterCustomization(string storyId, string promptDetailId, string chapterId, string promptText);

    /// <summary>
    /// Removes chapter-specific prompt text from a persisted prompt detail.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailMutationResult RemovePromptDetailChapterCustomization(string storyId, string promptDetailId, string chapterId);

    /// <summary>
    /// Replaces all chapter-specific prompt text for a persisted prompt detail.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailMutationResult ReplacePromptDetailChapterCustomizations(string storyId, string promptDetailId, IReadOnlyDictionary<string, string> chapterPromptTexts);

    /// <summary>
    /// Enables or disables a prompt detail for a chapter.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    PluginPromptDetailDto SetPromptDetailEnabledForChapter(string storyId, string promptDetailId, string chapterId, bool enabled);

    /// <summary>
    /// Deletes a guided chapter setup prompt detail from a story.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    bool DeletePromptDetail(string storyId, string promptDetailId);

    /// <summary>
    /// Reorders prompt details within a single entry type.
    /// Requires the <see cref="PluginCapabilities.PromptWrite" /> capability.
    /// </summary>
    IReadOnlyList<PluginPromptDetailDto> ReorderPromptDetails(string storyId, PluginGenerationEntryType entryType, IReadOnlyList<string> orderedPromptDetailIds);

    /// <summary>
    /// Lists source entries for writing guidance, world details (`StoryInformation` API identifier), items, characters, and custom entries.
    /// Requires the <see cref="PluginCapabilities.StoryRead" /> capability.
    /// </summary>
    IReadOnlyList<PluginPromptSourceEntryDto> GetPromptSourceEntries(string? storyId = null, PluginGenerationEntryType? entryType = null, PluginSelectionScope? scope = null);

    /// <summary>
    /// Creates a source entry. Persisted story/global entry limits are preserved.
    /// Requires the <see cref="PluginCapabilities.StoryWrite" /> capability.
    /// </summary>
    PluginPromptSourceEntryMutationResult CreatePromptSourceEntry(PluginPromptSourceEntryCreateRequest request);

    /// <summary>
    /// Updates a source entry using the app's observable collections and properties.
    /// Requires the <see cref="PluginCapabilities.StoryWrite" /> capability.
    /// </summary>
    PluginPromptSourceEntryMutationResult UpdatePromptSourceEntry(string sourceEntryId, PluginPromptSourceEntryUpdateRequest request);

    /// <summary>
    /// Deletes a source entry.
    /// Requires the <see cref="PluginCapabilities.StoryWrite" /> capability.
    /// </summary>
    bool DeletePromptSourceEntry(string sourceEntryId);
}
