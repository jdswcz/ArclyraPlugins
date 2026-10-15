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
    /// Disables host automatic readback and generation automation for an
    /// author-controlled browser workflow. Default false preserves existing providers.
    /// </summary>
    public bool ManualBrowserWorkflow { get; init; }

    /// <summary>Optional request-local choices displayed beside this direct generator. SDK 1.8 / host 45.</summary>
    public PluginAiGenerationMenu? GenerationMenu { get; init; }

    /// <summary>
    /// Gets a value indicating whether the configuration is owned by a plugin.
    /// </summary>
    public bool IsPluginOwned => !string.IsNullOrWhiteSpace(PluginOwnerId);
}

/// <summary>A menu of one-time choices. These are runtime registrations, never provider settings.</summary>
public sealed record PluginAiGenerationMenu(string DisplayName, IReadOnlyList<PluginAiGenerationOption> Options);

/// <summary>An opaque choice understood only by its owning generator.</summary>
public sealed record PluginAiGenerationOption(string Id, string DisplayName);
