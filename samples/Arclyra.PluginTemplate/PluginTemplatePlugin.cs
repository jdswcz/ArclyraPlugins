using Arclyra.PluginSdk;

namespace Arclyra.PluginTemplate;

/// <summary>
/// Minimal Arclyra plugin entry point for new plugin projects.
/// </summary>
public sealed class PluginTemplatePlugin : IArclyraPlugin
{
    /// <inheritdoc />
    public string Id => "com.example.arclyra.plugins.template";

    /// <inheritdoc />
    public string Name => "Arclyra Plugin Template";

    /// <inheritdoc />
    public Task InitializeAsync(IPluginContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        context.Logger.LogInformation("Arclyra Plugin Template initialized.");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ShutdownAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
