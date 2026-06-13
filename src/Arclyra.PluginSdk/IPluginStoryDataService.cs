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
    /// Gets global instruction snapshots.
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
    /// Gets global custom entry snapshots.
    /// Requires the <see cref="PluginCapabilities.StoryRead" /> capability.
    /// </summary>
    IReadOnlyList<PluginCustomEntryDto> GetGlobalCustomEntries();

    /// <summary>
    /// Adds an instruction through the host-owned observable collection.
    /// Pass a story id to add a story instruction; omit it to add a global instruction.
    /// Requires the <see cref="PluginCapabilities.StoryWrite" /> capability.
    /// </summary>
    void AddInstruction(string instruction, string? storyId = null);

    /// <summary>
    /// Adds a custom entry through the host-owned observable collection.
    /// Pass a story id to add a story custom entry; omit it to add a global custom entry.
    /// Requires the <see cref="PluginCapabilities.StoryWrite" /> capability.
    /// </summary>
    PluginCustomEntryDto AddCustomEntry(string category, string? description = null, string? storyId = null);

    /// <summary>
    /// Gets metadata owned by the current plugin. Metadata does not grant access to host story content.
    /// </summary>
    IReadOnlyDictionary<string, string> GetPluginMetadata();

    /// <summary>
    /// Adds, updates, or removes metadata owned by the current plugin. Pass <see langword="null" /> to remove a key.
    /// </summary>
    void SetPluginMetadata(string key, string? value);
}
