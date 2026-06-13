namespace Arclyra.PluginSdk;

/// <summary>
/// Defines the entry point that Arclyra uses to initialize and shut down a plugin.
/// </summary>
public interface IArclyraPlugin
{
    /// <summary>
    /// Gets the stable plugin identifier used by Arclyra to track the plugin.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the human-readable plugin display name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Initializes the plugin and gives it access to Arclyra services.
    /// </summary>
    /// <param name="context">The host context provided by Arclyra.</param>
    /// <param name="cancellationToken">A token that is signaled when initialization should stop.</param>
    /// <returns>A task that completes when initialization has finished.</returns>
    Task InitializeAsync(IPluginContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Shuts down the plugin and releases any resources that it owns.
    /// </summary>
    /// <param name="cancellationToken">A token that is signaled when shutdown should stop.</param>
    /// <returns>A task that completes when shutdown has finished.</returns>
    Task ShutdownAsync(CancellationToken cancellationToken = default);
}
