namespace Arclyra.PluginSdk;

/// <summary>
/// Registers plugin-provided UI contributions with the Arclyra host.
/// </summary>
public interface IPluginUiRegistry : IPluginEditorRegistry
{
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
    /// Registers a panel contributed to the prompt detail entry-management UI.
    /// Requires the <see cref="PluginCapabilities.UiEntryManagement" /> capability.
    /// </summary>
    void RegisterEntryManagementPanel(PluginEntryManagementPanelRegistration registration);
}
