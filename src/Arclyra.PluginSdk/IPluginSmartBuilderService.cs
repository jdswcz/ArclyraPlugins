namespace Arclyra.PluginSdk;

/// <summary>
/// Registers plugin contributions that extend Smart Builder UI and prompt-processing workflows.
/// </summary>
public interface IPluginSmartBuilderService
{
    /// <summary>
    /// Registers a toolbar action shown with the Smart Builder chapter prompt tools.
    /// Requires the <see cref="PluginCapabilities.UiSmartBuilder" /> capability.
    /// </summary>
    void RegisterToolbarAction(PluginSmartBuilderToolbarRegistration registration);

    /// <summary>
    /// Registers a sidebar panel hosted beside Smart Builder prompt review surfaces.
    /// Requires the <see cref="PluginCapabilities.UiSmartBuilder" /> capability.
    /// </summary>
    void RegisterSidebarPanel(PluginSmartBuilderSidebarRegistration registration);

    /// <summary>
    /// Registers content hosted below the Smart Builder prompt rows.
    /// Requires the <see cref="PluginCapabilities.UiSmartBuilder" /> capability.
    /// </summary>
    void RegisterBelowPromptContent(PluginSmartBuilderBelowPromptContentRegistration registration);

    /// <summary>
    /// Registers an action shown in the host Smart Builder chapter actions menu.
    /// Requires the <see cref="PluginCapabilities.UiSmartBuilder" /> capability.
    /// </summary>
    void RegisterChapterAction(PluginSmartBuilderChapterActionRegistration registration);

    /// <summary>
    /// Registers an action shown for each Smart Builder prompt row.
    /// Requires the <see cref="PluginCapabilities.UiSmartBuilder" /> capability.
    /// </summary>
    void RegisterPromptRowAction(PluginPromptRowActionRegistration registration);

    /// <summary>
    /// Registers a prompt validator invoked while Smart Builder builds prompt text.
    /// Requires the <see cref="PluginCapabilities.PromptRead" /> capability.
    /// </summary>
    void RegisterPromptValidator(PluginPromptValidatorRegistration registration);

    /// <summary>
    /// Registers a prompt transform invoked while Smart Builder builds prompt text.
    /// Transforms require <see cref="PluginCapabilities.PromptRead" /> and <see cref="PluginCapabilities.PromptWrite" />.
    /// </summary>
    void RegisterPromptTransform(PluginPromptTransformRegistration registration);
}
