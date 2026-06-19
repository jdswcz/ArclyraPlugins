namespace Arclyra.PluginSdk;

/// <summary>
/// Registers a plugin-provided action next to the AI generation paste controls.
/// </summary>
/// <param name="ActionId">Stable action id unique within the plugin.</param>
/// <param name="DisplayName">User-facing button label.</param>
/// <param name="ToolTip">Optional tooltip shown for the action.</param>
/// <param name="SortOrder">Sort order among plugin paste actions.</param>
/// <param name="ExecuteAsync">Async callback invoked when the user clicks the action.</param>
public sealed record PluginAiGenerationPasteActionRegistration(
    string ActionId,
    string DisplayName,
    string? ToolTip,
    int SortOrder,
    PluginAiGenerationPasteActionHandler ExecuteAsync);

/// <summary>
/// Context supplied to plugin AI generation paste actions.
/// </summary>
/// <param name="SelectedProvider">Snapshot of the selected AI provider, if one is selected.</param>
/// <param name="BaseStoryInfo">Current base story info/story guide text.</param>
/// <param name="CurrentChapterCommand">Current chapter command text.</param>
/// <param name="CurrentRenderedPrompt">Current rendered prompt, when the host has one available.</param>
/// <param name="CancellationToken">Cancellation token for the action invocation.</param>
public sealed record PluginAiGenerationPasteActionContext(
    PluginAiConfiguration? SelectedProvider,
    string BaseStoryInfo,
    string CurrentChapterCommand,
    string? CurrentRenderedPrompt,
    CancellationToken CancellationToken);

/// <summary>
/// Handles a plugin AI generation paste action invocation.
/// </summary>
public delegate Task PluginAiGenerationPasteActionHandler(PluginAiGenerationPasteActionContext context);
