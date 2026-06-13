namespace Arclyra.PluginSdk;

/// <summary>
/// Provides plugin-safe, read-only access to Arclyra user preference snapshots.
/// </summary>
public interface IPluginPreferencesService
{
    /// <summary>
    /// Gets a read-only snapshot of the current Arclyra user preferences.
    /// Requires the <see cref="PluginCapabilities.SettingsRead" /> capability.
    /// </summary>
    PluginUserPreferencesDto GetPreferences();
}

/// <summary>
/// A read-only snapshot of host preferences that are safe for plugins to inspect.
/// </summary>
/// <param name="AppearanceTheme">The selected host appearance theme.</param>
/// <param name="UseLargeText">Whether large text scaling is enabled.</param>
/// <param name="PreventScreenshots">Whether screenshot prevention is enabled.</param>
/// <param name="UseFullWidthGlobalSettings">Whether global settings open in full-width mode.</param>
/// <param name="WorkflowWindowPreference">The user's workflow window placement preference.</param>
public sealed record PluginUserPreferencesDto(
    string AppearanceTheme,
    bool UseLargeText,
    bool PreventScreenshots,
    bool UseFullWidthGlobalSettings,
    PluginWorkflowWindowPreference WorkflowWindowPreference);

/// <summary>
/// Plugin-safe values for Arclyra workflow window placement preferences.
/// </summary>
public enum PluginWorkflowWindowPreference
{
    Automatic,
    PreferExternalWindows,
    PreferInlineControls
}
