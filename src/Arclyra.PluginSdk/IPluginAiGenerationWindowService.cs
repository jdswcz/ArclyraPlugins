namespace Arclyra.PluginSdk;

/// <summary>
/// Provides host-mediated access to the AI generation window.
/// </summary>
public interface IPluginAiGenerationWindowService
{
    /// <summary>
    /// Gets whether the AI generation window is currently open.
    /// </summary>
    bool IsWindowOpen { get; }

    /// <summary>
    /// Gets a snapshot of the AI provider currently selected in the AI generation control, if available.
    /// </summary>
    PluginAiConfiguration? SelectedProvider { get; }

    /// <summary>
    /// Reads the AI provider currently selected in the AI generation control.
    /// Requires the aiGeneration.browserAccess capability and is audited by the host.
    /// </summary>
    Task<PluginBrowserAutomationResult<PluginAiConfiguration?>> ReadSelectedProviderAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Pastes the host's current chapter prompt into the selected AI provider page using the provider's approved paste script.
    /// Requires the aiGeneration.browserAccess capability and is audited by the host.
    /// </summary>
    Task<PluginBrowserAutomationResult> PasteCurrentPromptAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Triggers generation for the selected provider using host-approved automation.
    /// Requires the aiGeneration.browserAccess capability and is audited by the host.
    /// </summary>
    Task<PluginBrowserAutomationResult> TriggerGenerationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads generated content from the selected AI provider page using the provider's approved read script.
    /// Requires the aiGeneration.browserAccess capability and is audited by the host.
    /// </summary>
    Task<PluginBrowserAutomationResult<string?>> ReadGeneratedContentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs a user-approved script against the selected AI provider page and returns the raw script result as text.
    /// Requires the aiGeneration.browserAccess capability and is audited by the host.
    /// </summary>
    Task<PluginBrowserAutomationResult<string?>> RunApprovedScriptAsync(string script, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the outcome of a host-mediated AI browser automation operation.
/// </summary>
public record PluginBrowserAutomationResult(bool Success, string? StatusMessage)
{
    public static PluginBrowserAutomationResult Succeeded(string? statusMessage = null) => new(true, statusMessage);

    public static PluginBrowserAutomationResult Failed(string statusMessage) => new(false, statusMessage);
}

/// <summary>
/// Represents the outcome of a host-mediated AI browser automation operation that returns data.
/// </summary>
public sealed record PluginBrowserAutomationResult<T>(bool Success, string? StatusMessage, T? Value)
    : PluginBrowserAutomationResult(Success, StatusMessage)
{
    public static PluginBrowserAutomationResult<T> Succeeded(T? value, string? statusMessage = null) => new(true, statusMessage, value);

    public new static PluginBrowserAutomationResult<T> Failed(string statusMessage) => new(false, statusMessage, default);
}
