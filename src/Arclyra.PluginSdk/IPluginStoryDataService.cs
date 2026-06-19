namespace Arclyra.PluginSdk;

/// <summary>
/// Provides capability-gated, plugin-safe access to Arclyra story data snapshots and explicit host-owned mutations.
/// </summary>
public interface IPluginStoryDataService
{
    /// <summary>
    /// Gets read-only snapshots of all stories.
    /// Requires the <see cref="PluginCapabilities.StoryRead" /> capability.
    /// </summary>
    IReadOnlyList<PluginStoryDto> GetStories();

    /// <summary>
    /// Gets a read-only snapshot of a story by id.
    /// Requires the <see cref="PluginCapabilities.StoryRead" /> capability.
    /// </summary>
    PluginStoryDto? GetStory(string storyId);

    /// <summary>
    /// Gets global writing guidance snapshots. The API name keeps "Instructions" for compatibility.
    /// Requires the <see cref="PluginCapabilities.StoryRead" /> capability.
    /// </summary>
    IReadOnlyList<string> GetGlobalInstructions();

    /// <summary>
    /// Gets global character snapshots.
    /// Requires the <see cref="PluginCapabilities.StoryRead" /> capability.
    /// </summary>
    IReadOnlyList<PluginCharacterDto> GetGlobalCharacters();

    /// <summary>
    /// Gets global item snapshots.
    /// Requires the <see cref="PluginCapabilities.StoryRead" /> capability.
    /// </summary>
    IReadOnlyList<PluginItemDto> GetGlobalItems();

    /// <summary>
    /// Gets global custom prompt detail source entry snapshots.
    /// Requires the <see cref="PluginCapabilities.StoryRead" /> capability.
    /// </summary>
    IReadOnlyList<PluginCustomEntryDto> GetGlobalCustomEntries();

    /// <summary>
    /// Adds writing guidance through the host-owned observable collection.
    /// Pass a story id to add story writing guidance; omit it to add global writing guidance. The API name keeps "Instruction" for compatibility.
    /// Requires the <see cref="PluginCapabilities.StoryWrite" /> capability.
    /// </summary>
    void AddInstruction(string instruction, string? storyId = null);

    /// <summary>
    /// Adds a custom prompt detail source entry through the host-owned observable collection.
    /// Pass a story id to add a story custom entry; omit it to add a global custom entry.
    /// Requires the <see cref="PluginCapabilities.StoryWrite" /> capability.
    /// </summary>
    PluginCustomEntryDto AddCustomEntry(string category, string? description = null, string? storyId = null);

    /// <summary>
    /// Creates a story through the host-owned story collection.
    /// Requires the <see cref="PluginCapabilities.StoryWrite" /> capability.
    /// </summary>
    PluginStoryDto CreateStory(PluginCreateStoryRequest request);

    /// <summary>
    /// Updates story metadata and base story information.
    /// Requires the <see cref="PluginCapabilities.StoryWrite" /> capability.
    /// </summary>
    PluginStoryDto UpdateStory(PluginUpdateStoryRequest request);

    /// <summary>
    /// Deletes a story by id.
    /// Requires the <see cref="PluginCapabilities.StoryWrite" /> capability.
    /// </summary>
    void DeleteStory(string storyId);

    PluginChapterDto CreateChapter(PluginCreateChapterRequest request);
    PluginChapterDto UpdateChapter(PluginUpdateChapterRequest request);
    void DeleteChapter(string storyId, string chapterId);

    PluginCharacterDto CreateCharacter(PluginCreateCharacterRequest request);
    PluginCharacterDto UpdateCharacter(PluginUpdateCharacterRequest request);
    void DeleteCharacter(PluginScopedEntryRequest request);

    PluginItemDto CreateItem(PluginCreateItemRequest request);
    PluginItemDto UpdateItem(PluginUpdateItemRequest request);
    void DeleteItem(PluginScopedEntryRequest request);

    PluginWorldDetailDto CreateWorldDetail(PluginCreateWorldDetailRequest request);
    PluginWorldDetailDto UpdateWorldDetail(PluginUpdateWorldDetailRequest request);
    void DeleteWorldDetail(string storyId, string worldDetailId);

    PluginCustomEntryDto CreateCustomEntry(PluginCreateCustomEntryRequest request);
    PluginCustomEntryDto UpdateCustomEntry(PluginUpdateCustomEntryRequest request);
    void DeleteCustomEntry(PluginScopedEntryRequest request);

    PluginGeneratedDraftDto CreateGeneratedDraft(PluginCreateGeneratedDraftRequest request);
    PluginGeneratedDraftDto UpdateGeneratedDraft(PluginUpdateGeneratedDraftRequest request);
    void DeleteGeneratedDraft(string storyId, string chapterId, int draftIndex);


    /// <summary>
    /// Gets metadata owned by the current plugin. Metadata does not grant access to host story content.
    /// </summary>
    IReadOnlyDictionary<string, string> GetPluginMetadata();

    /// <summary>
    /// Adds, updates, or removes metadata owned by the current plugin. Pass <see langword="null" /> to remove a key.
    /// </summary>
    void SetPluginMetadata(string key, string? value);
}
