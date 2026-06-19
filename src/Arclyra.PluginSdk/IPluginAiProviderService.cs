namespace Arclyra.PluginSdk;

/// <summary>
/// Provides plugin-safe access to Arclyra AI provider configurations.
/// </summary>
public interface IPluginAiProviderService
{
    /// <summary>
    /// Occurs when the host AI provider configuration collection or one of its entries changes.
    /// </summary>
    event EventHandler? ConfigurationsChanged;

    /// <summary>
    /// Gets a snapshot of the current AI provider configurations.
    /// Requires the <see cref="PluginCapabilities.AiProvidersRead" /> capability.
    /// </summary>
    /// <returns>A plugin SDK DTO snapshot of the current configurations.</returns>
    IReadOnlyList<PluginAiConfiguration> GetConfigurations();

    /// <summary>
    /// Adds or updates an AI provider configuration owned by the current plugin.
    /// Requires the <see cref="PluginCapabilities.AiProvidersWrite" /> capability.
    /// </summary>
    /// <param name="configuration">The provider configuration values to persist.</param>
    /// <returns>The persisted configuration with host and plugin ownership metadata populated.</returns>
    PluginAiConfiguration AddOrUpdateConfiguration(PluginAiConfiguration configuration);

    /// <summary>
    /// Removes a provider configuration owned by the current plugin.
    /// Requires the <see cref="PluginCapabilities.AiProvidersWrite" /> capability.
    /// </summary>
    /// <param name="configurationId">The host configuration id or stable plugin configuration id to remove.</param>
    /// <returns><see langword="true" /> when a configuration was removed; otherwise, <see langword="false" />.</returns>
    bool RemovePluginConfiguration(string configurationId);

    /// <summary>
    /// Adds or updates a plugin-owned AI provider that generates chapter drafts directly, without Arclyra's embedded browser.
    /// Requires the <see cref="PluginCapabilities.AiGeneration" /> capability.
    /// </summary>
    /// <param name="configuration">The provider metadata shown in Arclyra's AI generation provider selector.</param>
    /// <param name="handler">The async generator invoked with the rendered guided chapter setup prompt.</param>
    /// <returns>The registered configuration with host and plugin ownership metadata populated.</returns>
    PluginAiConfiguration AddOrUpdateGenerator(PluginAiConfiguration configuration, PluginAiGenerationHandler handler);
}
