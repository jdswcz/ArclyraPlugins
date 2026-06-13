namespace Arclyra.PluginSdk;

/// <summary>
/// Describes an AI provider configuration exposed through the Arclyra plugin SDK.
/// </summary>
/// <param name="Id">The host configuration identifier.</param>
/// <param name="Name">The user-facing provider name.</param>
/// <param name="Url">The provider URL opened by Arclyra.</param>
/// <param name="PromptPasteScript">The browser script Arclyra runs to paste prompts.</param>
/// <param name="ReadGeneratedChapterScript">The browser script Arclyra runs to read generated content.</param>
/// <param name="PluginOwnerId">The plugin id that owns the configuration, or <see langword="null" /> for host/user configurations.</param>
/// <param name="PluginConfigurationId">The plugin's stable provider identifier, or <see langword="null" /> for host/user configurations.</param>
public sealed record PluginAiConfiguration(
    string Id,
    string Name,
    string Url,
    string PromptPasteScript,
    string ReadGeneratedChapterScript,
    string? PluginOwnerId = null,
    string? PluginConfigurationId = null)
{
    /// <summary>
    /// Gets a value indicating whether the configuration is owned by a plugin.
    /// </summary>
    public bool IsPluginOwned => !string.IsNullOrWhiteSpace(PluginOwnerId);
}
