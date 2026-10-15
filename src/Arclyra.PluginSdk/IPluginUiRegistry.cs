namespace Arclyra.PluginSdk;

/// <summary>
/// Registers plugin-provided UI contributions with the Arclyra host.
/// </summary>
public interface IPluginUiRegistry : IPluginEditorRegistry
{
    /// <summary>
    /// Replaces the provider/paste toolbar for an owned browser configuration only.
    /// Requires ui.aiGenerationToolbar and prompt.read. Older hosts reject this
    /// optional extension; existing implementations remain binary-compatible.
    /// </summary>
    void RegisterAiGenerationToolbar(PluginAiGenerationToolbarRegistration registration) =>
        throw new NotSupportedException("AI generation toolbar replacement requires Arclyra host 51 or newer.");

    /// <summary>
    /// Registers a menu item contributed by a plugin.
    /// </summary>
    /// <param name="registration">The menu item registration details.</param>
    void RegisterMenuItem(PluginMenuItemRegistration registration);

    /// <summary>
    /// Registers a settings page contributed by a plugin.
    /// Requires the <see cref="PluginCapabilities.UiSettings" /> capability.
    /// </summary>
    /// <param name="registration">The settings page registration details.</param>
    void RegisterSettingsPage(PluginSettingsPageRegistration registration);

    /// <summary>
    /// Registers a dockable or hosted panel contributed by a plugin.
    /// Requires the <see cref="PluginCapabilities.UiWorkspacePanel" /> capability.
    /// </summary>
    /// <param name="registration">The panel registration details.</param>
    void RegisterPanel(PluginPanelRegistration registration);

    /// <summary>
    /// Requests that Arclyra open one of this plugin's registered workspace panels.
    /// Requires the <see cref="PluginCapabilities.UiWorkspacePanel" /> capability.
    /// </summary>
    /// <param name="panelId">The stable panel id previously registered by this plugin.</param>
    void OpenPanel(string panelId);

    /// <summary>
    /// Registers a panel contributed to the story overview region.
    /// Requires the <see cref="PluginCapabilities.UiStoryOverview" /> capability.
    /// </summary>
    void RegisterStoryOverviewPanel(PluginStoryOverviewPanelRegistration registration);

    /// <summary>
    /// Registers a button contributed to every item in the main story list.
    /// Requires the <see cref="PluginCapabilities.UiStoryListActions" /> capability.
    /// </summary>
    void RegisterStoryListItemButton(PluginStoryListItemButtonRegistration registration);

    /// <summary>
    /// Registers an optional step contributed to the new-story wizard.
    /// Requires the <see cref="PluginCapabilities.UiNewStoryWizard" /> capability.
    /// </summary>
    void RegisterNewStoryWizardStep(PluginNewStoryWizardStepRegistration registration);

    /// <summary>
    /// Registers a panel contributed to the prompt setup UI.
    /// Requires the <see cref="PluginCapabilities.UiEntryManagement" /> capability.
    /// </summary>
    void RegisterEntryManagementPanel(PluginEntryManagementPanelRegistration registration);

    /// <summary>
    /// Registers a user-invoked action beside the Prompt setup Save button.
    /// Requires both <see cref="PluginCapabilities.UiEntryManagement" /> and
    /// <see cref="PluginCapabilities.PromptWrite" /> because the action can replace
    /// the active prompt-detail description through a host-owned callback.
    /// </summary>
    void RegisterEntryManagementAction(PluginEntryManagementActionRegistration registration);

    /// <summary>
    /// Registers an action contributed next to the AI generation paste controls.
    /// Requires the <see cref="PluginCapabilities.UiAiGenerationPasteActions" /> capability.
    /// Browser automation remains restricted to <see cref="PluginCapabilities.AiGenerationBrowserAccess" /> through IPluginAiGenerationWindowService.
    /// </summary>
    void RegisterAiGenerationPasteAction(PluginAiGenerationPasteActionRegistration registration);
}
