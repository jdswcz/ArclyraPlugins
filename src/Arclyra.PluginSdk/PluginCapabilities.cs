namespace Arclyra.PluginSdk;

/// <summary>
/// Stable capability names that a plugin can declare in its manifest to request access to host data and integration points.
/// </summary>
public static class PluginCapabilities
{
    /// <summary>
    /// Allows a plugin to read story data snapshots.
    /// </summary>
    public const string StoryRead = "story.read";

    /// <summary>
    /// Allows a plugin to add or update story data through explicit host-owned mutation APIs.
    /// </summary>
    public const string StoryWrite = "story.write";

    /// <summary>
    /// Allows a plugin to read prompt templates and rendered prompt context.
    /// </summary>
    public const string PromptRead = "prompt.read";

    /// <summary>
    /// Reserved for plugins that update prompt templates or prompt context through explicit host APIs.
    /// </summary>
    public const string PromptWrite = "prompt.write";

    /// <summary>
    /// Allows a plugin to read host user preference snapshots.
    /// </summary>
    public const string SettingsRead = "settings.read";

    /// <summary>
    /// Allows a plugin to read AI provider configurations.
    /// </summary>
    public const string AiProvidersRead = "aiProviders.read";

    /// <summary>
    /// Allows a plugin to add, update, or remove AI provider configurations it owns.
    /// </summary>
    public const string AiProvidersWrite = "aiProviders.write";

    /// <summary>
    /// Allows a plugin to act as an AI generation bridge and return generated chapter drafts directly to Arclyra.
    /// </summary>
    public const string AiGeneration = "ai.generation";

    /// <summary>
    /// Allows a plugin to access the live Chromium browser in the AI generation window.
    /// </summary>
    public const string AiGenerationBrowserAccess = "aiGeneration.browserAccess";

    /// <summary>
    /// Allows a plugin to subscribe to host events.
    /// </summary>
    public const string EventsSubscribe = "events.subscribe";

    /// <summary>
    /// Allows a plugin to subscribe to story lifecycle events.
    /// </summary>
    public const string EventsStory = "events.story";

    /// <summary>
    /// Allows a plugin to subscribe to chapter lifecycle and content events.
    /// </summary>
    public const string EventsChapter = "events.chapter";

    /// <summary>
    /// Allows a plugin to subscribe to generated draft events.
    /// </summary>
    public const string EventsGeneratedDraft = "events.generatedDraft";

    /// <summary>
    /// Allows a plugin to subscribe to guided chapter setup prompt detail events.
    /// </summary>
    public const string EventsPromptDetail = "events.promptDetail";

    /// <summary>
    /// Allows a plugin to subscribe to AI provider configuration events.
    /// </summary>
    public const string EventsAiProvider = "events.aiProvider";

    /// <summary>
    /// Allows a plugin to subscribe to import/export completion events.
    /// </summary>
    public const string EventsTransfer = "events.transfer";

    /// <summary>
    /// Allows a plugin to contribute UI to the Arclyra settings experience.
    /// </summary>
    public const string UiSettings = "ui.settings";

    /// <summary>
    /// Allows a plugin to contribute dockable or hosted workspace panels.
    /// </summary>
    public const string UiWorkspacePanel = "ui.workspacePanel";

    /// <summary>
    /// Reserved for plugins that contribute UI directly to guided chapter setup workflows.
    /// </summary>
    public const string UiSmartBuilder = "ui.smartBuilder";

    /// <summary>
    /// Allows a plugin to replace supported host editing surfaces with plugin-provided editor controls.
    /// </summary>
    public const string UiEditors = "ui.editors";

    /// <summary>
    /// Allows a plugin to contribute UI to story overview regions.
    /// </summary>
    public const string UiStoryOverview = "ui.storyOverview";

    /// <summary>
    /// Allows a plugin to contribute action buttons to rows in the main story list.
    /// </summary>
    public const string UiStoryListActions = "ui.storyListActions";

    /// <summary>
    /// Allows a plugin to show simple host-owned dialogs for user-initiated UI prompts.
    /// </summary>
    public const string UiDialogs = "ui.dialogs";

    /// <summary>
    /// Allows a plugin to contribute optional steps to the new-story wizard.
    /// </summary>
    public const string UiNewStoryWizard = "ui.newStoryWizard";

    /// <summary>
    /// Allows a plugin to contribute UI to prompt setup regions.
    /// </summary>
    public const string UiEntryManagement = "ui.entryManagement";

    /// <summary>
    /// Allows a plugin to contribute user-invoked actions next to AI generation paste controls.
    /// </summary>
    public const string UiAiGenerationPasteActions = "ui.aiGenerationPasteActions";

    /// <summary>
    /// Reserved for future host-mediated network access. Production manifests cannot request this capability until the SDK exposes the corresponding service.
    /// </summary>
    public const string Network = "network";

    /// <summary>
    /// Allows a plugin to access the path to its installation directory.
    /// </summary>
    public const string FileSystemPluginDirectory = "fileSystem.pluginDirectory";

    /// <summary>
    /// Allows a plugin to load native/unmanaged dependencies from its installation directory.
    /// </summary>
    public const string NativeDependencies = "nativeDependencies";

    /// <summary>
    /// Gets all known stable capability names.
    /// </summary>
    public static IReadOnlySet<string> All { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        StoryRead,
        StoryWrite,
        PromptRead,
        PromptWrite,
        SettingsRead,
        AiProvidersRead,
        AiProvidersWrite,
        AiGeneration,
        AiGenerationBrowserAccess,
        EventsSubscribe,
        EventsStory,
        EventsChapter,
        EventsGeneratedDraft,
        EventsPromptDetail,
        EventsAiProvider,
        EventsTransfer,
        UiSettings,
        UiWorkspacePanel,
        UiSmartBuilder,
        UiEditors,
        UiStoryOverview,
        UiStoryListActions,
        UiDialogs,
        UiNewStoryWizard,
        UiEntryManagement,
        UiAiGenerationPasteActions,
        FileSystemPluginDirectory,
        NativeDependencies
    };

    /// <summary>
    /// Gets known capability names reserved for preview or future host APIs. Production manifests cannot request these capabilities yet.
    /// </summary>
    public static IReadOnlySet<string> Reserved { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        Network
    };
}
