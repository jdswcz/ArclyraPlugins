namespace Arclyra.PluginSdk;

/// <summary>
/// Provides host-owned dialog surfaces that plugins can use for simple, user-initiated UI prompts.
/// Requires the <see cref="PluginCapabilities.UiDialogs" /> capability.
/// </summary>
public interface IPluginDialogService
{
    /// <summary>
    /// Shows an informational dialog.
    /// </summary>
    void ShowInfo(string title, string message, string primaryText = "OK");

    /// <summary>
    /// Shows a two-button confirmation dialog and returns <see langword="true" /> when the user chooses the confirm action.
    /// </summary>
    bool ShowConfirmation(string title, string message, string confirmText = "Yes", string cancelText = "No", bool isDestructive = false);

    /// <summary>
    /// Shows a text input dialog and returns whether the user confirmed along with the resulting text.
    /// </summary>
    PluginDialogInputResult ShowInput(string title, string message, string defaultValue = "", string confirmText = "OK", string cancelText = "Cancel");

    /// <summary>
    /// Shows a three-button yes/no/cancel dialog.
    /// </summary>
    PluginThreeButtonDialogResult ShowYesNoCancel(string title, string message, string yesText = "Yes", string noText = "No", string cancelText = "Cancel");
}

/// <summary>
/// Result returned by a plugin input dialog.
/// </summary>
public sealed record PluginDialogInputResult(bool IsConfirmed, string InputText);

/// <summary>
/// Result values returned by a plugin yes/no/cancel dialog.
/// </summary>
public enum PluginThreeButtonDialogResult
{
    Yes,
    No,
    Cancel
}
