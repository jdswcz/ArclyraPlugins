namespace Arclyra.PluginSdk;

/// <summary>
/// Registers plugin contributions that extend guided chapter setup UI and prompt-processing workflows.
/// </summary>
public interface IPluginSmartBuilderService
{
    /// <summary>
    /// Registers a toolbar action shown with the guided chapter setup chapter prompt tools.
    /// Requires the <see cref="PluginCapabilities.UiSmartBuilder" /> capability.
    /// </summary>
    void RegisterToolbarAction(PluginSmartBuilderToolbarRegistration registration);

    /// <summary>
    /// Registers a sidebar panel hosted beside guided chapter setup prompt review surfaces.
    /// Requires the <see cref="PluginCapabilities.UiSmartBuilder" /> capability.
    /// </summary>
    void RegisterSidebarPanel(PluginSmartBuilderSidebarRegistration registration);

    /// <summary>
    /// Registers content hosted below the guided chapter setup prompt rows.
    /// Requires the <see cref="PluginCapabilities.UiSmartBuilder" /> capability.
    /// </summary>
    void RegisterBelowPromptContent(PluginSmartBuilderBelowPromptContentRegistration registration);

    /// <summary>
    /// Registers an action shown in the host guided chapter setup actions menu.
    /// Requires the <see cref="PluginCapabilities.UiSmartBuilder" /> capability.
    /// </summary>
    void RegisterChapterAction(PluginSmartBuilderChapterActionRegistration registration);

    /// <summary>
    /// Registers an action shown for each guided chapter setup prompt row.
    /// Requires the <see cref="PluginCapabilities.UiSmartBuilder" /> capability.
    /// </summary>
    void RegisterPromptRowAction(PluginPromptRowActionRegistration registration);

    /// <summary>
    /// Registers a prompt validator invoked while guided chapter setup builds prompt text.
    /// Requires the <see cref="PluginCapabilities.PromptRead" /> capability.
    /// </summary>
    void RegisterPromptValidator(PluginPromptValidatorRegistration registration);

    /// <summary>
    /// Registers a prompt transform invoked while guided chapter setup builds prompt text.
    /// Transforms require <see cref="PluginCapabilities.PromptRead" /> and <see cref="PluginCapabilities.PromptWrite" />.
    /// </summary>
    void RegisterPromptTransform(PluginPromptTransformRegistration registration);
}
