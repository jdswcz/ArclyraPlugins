namespace Arclyra.PluginSdk;

/// <summary>
/// Provides host services and metadata to a running Arclyra plugin.
/// </summary>
public interface IPluginContext
{
    /// <summary>
    /// Gets the full path to the plugin's installation directory.
    /// Requires the <see cref="PluginCapabilities.FileSystemPluginDirectory" /> capability.
    /// </summary>
    string PluginDirectory { get; }

    /// <summary>
    /// Gets the current Arclyra application version.
    /// </summary>
    Version HostVersion { get; }

    /// <summary>
    /// Gets the logger that writes messages through the Arclyra plugin host.
    /// </summary>
    IPluginLogger Logger { get; }

    /// <summary>
    /// Gets the registry used to add plugin-provided UI surfaces.
    /// </summary>
    IPluginUiRegistry UiRegistry { get; }

    /// <summary>
    /// Gets the capability-gated service used to show host-owned dialogs.
    /// Requires the <see cref="PluginCapabilities.UiDialogs" /> capability.
    /// </summary>
    IPluginDialogService DialogService { get; }

    /// <summary>
    /// Gets the registry used to add plugin-provided commands.
    /// </summary>
    IPluginCommandRegistry CommandRegistry { get; }

    /// <summary>
    /// Gets the bus used to subscribe to stable host events.
    /// </summary>
    IPluginEventBus EventBus { get; }

    /// <summary>
    /// Gets the service used to inspect and manage plugin-owned AI provider configurations.
    /// </summary>
    IPluginAiProviderService AiProviderService { get; }

    /// <summary>
    /// Gets the capability-gated service used to access story data snapshots and explicit host-owned mutations.
    /// </summary>
    IPluginStoryDataService StoryDataService { get; }

    /// <summary>
    /// Gets the capability-gated service used to access chapter prompt templates and rendered prompt details.
    /// </summary>
    IPluginPromptContextService PromptContextService { get; }

    /// <summary>
    /// Gets the capability-gated service used to register Smart Builder UI contributions and prompt transforms.
    /// </summary>
    IPluginSmartBuilderService SmartBuilderService { get; }

    /// <summary>
    /// Gets the capability-gated service used to read host user preference snapshots.
    /// </summary>
    IPluginPreferencesService PreferencesService { get; }
}
