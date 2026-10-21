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
    /// Gets the full path to the plugin's host-owned writable data directory.
    /// Use this location for persistent plugin settings, downloaded components,
    /// learned data and other mutable plugin-owned files.
    /// </summary>
    string PluginDataDirectory { get; }

    /// <summary>
    /// Gets host-owned, workspace-scoped encrypted storage for plugin data that may
    /// contain story text, prompts, generated output, learned feedback or other
    /// user content. Older hosts return null through the default interface member.
    /// </summary>
    IPluginProtectedStorageService? ProtectedStorageService => null;

    /// <summary>Capability-gated local Arclyra AI historical summaries; null on older hosts.</summary>
    IPluginStorySummaryService? StorySummaryService => null;

    /// <summary>Optional host-owned continuity memory; null on older hosts.</summary>
    IPluginStoryMemoryService? StoryMemoryService => null;

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

    /// <summary>Gets narrowly scoped first-party AI placement only after dedicated
    /// host signature/identity verification. Ordinary plugins and older hosts return null.</summary>
    IPluginFirstPartyUiService? FirstPartyUiService => null;

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
    /// Gets the capability-gated service used to register guided chapter setup UI contributions and prompt transforms.
    /// </summary>
    IPluginSmartBuilderService SmartBuilderService { get; }

    /// <summary>
    /// Gets the capability-gated service used to register actions in Arclyra's built-in chapter editor.
    /// Requires the <see cref="PluginCapabilities.UiEditorActions" /> capability.
    /// </summary>
    IPluginChapterEditorActionService ChapterEditorActionService { get; }

    /// <summary>
    /// Gets the capability-gated service used to read host user preference snapshots.
    /// </summary>
    IPluginPreferencesService PreferencesService { get; }

    /// <summary>
    /// Gets the legacy license compatibility service. Arclyra no longer has a Premium tier,
    /// so <see cref="IPluginLicenseService.IsPremiumActive" /> always returns <see langword="true"/>.
    /// New plugins must not use this property for feature gating.
    /// </summary>
#pragma warning disable CS0618 // The obsolete type is intentionally exposed for binary compatibility.
    [Obsolete("Arclyra no longer has a Premium tier. LicenseService remains only for plugin binary compatibility.")]
    IPluginLicenseService LicenseService { get; }
#pragma warning restore CS0618

    /// <summary>
    /// Gets the capability-gated service used to inspect the AI generation window and access its browser control.
    /// </summary>
    IPluginAiGenerationWindowService AiGenerationWindowService { get; }
}
