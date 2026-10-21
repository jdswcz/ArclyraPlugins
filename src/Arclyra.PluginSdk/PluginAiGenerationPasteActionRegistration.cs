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
    CancellationToken CancellationToken)
{
    /// <summary>SDK 1.31 / host 77. Available only during an explicit toolbar action with browser
    /// access and a current chapter. Captures a separate cancellable active session; null on older hosts.</summary>
    public Func<PluginBrowserAutomationSessionRequest, PluginBrowserAutomationResult<IPluginBrowserAutomationSession>>? BeginAutomation { get; init; }

    /// <summary>Optional host status progress for the current cancellable toolbar action.</summary>
    public IProgress<string>? Progress { get; init; }

    /// <summary>SDK 1.26 / host 67. Stable captured story/chapter identities for derived browser context.</summary>
    public string? CurrentStoryId { get; init; }
    public string? CurrentChapterId { get; init; }
    /// <summary>Content-derived stored manuscript revision, including chapter identities and revision choices.</summary>
    public string? CurrentManuscriptRevision { get; init; }
    /// <summary>Invocation-scoped preflight: validates the current workspace, story, chapter and stored manuscript
    /// and matches full browser prose to current/latest stored chapters. Does not write or consume import.</summary>
    public Func<string, bool>? ValidateRollingContextSource { get; init; }
    /// <summary>SDK 1.27 / host 68. Bounded Arclyra manuscript including current unsaved staging.</summary>
    public PluginRollingStorySource? ArclyraRollingContextSource { get; init; }
    /// <summary>Checks identity/revision/staging without requiring equality with browser prose.</summary>
    public Func<bool>? IsRollingContextCurrent { get; init; }
    /// <summary>Reads only known current-chapter boundaries; unknown browser text is history context.</summary>
    public Func<string, PluginRollingStorySource?>? ReadRollingBrowserSource { get; init; }
    /// <summary>SDK 1.24 / host 65. Raw author outline only, excluding templates and context.</summary>
    public string? CurrentChapterOutline { get; init; }
    /// <summary>SDK 1.25 / host 66. Outline section rendered through the chapter template,
    /// including its placeholder title and configured row prefixes.</summary>
    public string? CurrentRenderedChapterOutline { get; init; }
    /// <summary>SDK 1.24 / host 65. Complete rendered guide and chapter prompt without the outline
    /// section (host 66 removes its placeholder title too). All other template, language, instruction and context content is preserved.</summary>
    public string? CurrentRenderedPromptWithoutOutline { get; init; }
    /// <summary>SDK 1.24 / host 65. One-based chapter number; null when no chapter is selected.</summary>
    public int? CurrentChapterNumber { get; init; }
    /// <summary>SDK 1.24 / host 65. User-invoked discard of current unsaved staging and its browser
    /// chapter. The host validates the boundary and acknowledged replacement; saved revisions stay intact.</summary>
    public Func<PluginBrowserChapterDiscardRequest, Task<PluginBrowserAutomationResult>>? DiscardChapterDraftAsync { get; init; }
    /// <summary>Names/scopes of the actual selected prompt fields; no additional field values.</summary>
    public IReadOnlyList<string> SharedFieldLabels { get; init; } = [];
    /// <summary>SDK 1.23 / host 63. Full-story revision selection, preview, confirmation and verified replacement.
    /// No Arclyra persistence; available only for a captured current chapter and browser capability.</summary>
    public Func<PluginBrowserStoryLoadRequest, Task<PluginBrowserAutomationResult>>? LoadChaptersAsync { get; init; }
    /// <summary>SDK 1.23. A loaded full story may continue only in its explicit last chapter.</summary>
    public string? ChapterGenerationUnavailableReason { get; init; }
    /// <summary>Captured chapter target, or a message explaining why no target is available.</summary>
    public string? ImportTargetDescription { get; init; }
    /// <summary>Single-use host workflow: full review, explicit save, stale checks, then a new revision.</summary>
    public Func<string, Func<Task<bool>>, Task<PluginBrowserAutomationResult>>? ReviewAndImportAsNewDraftAsync { get; init; }
    /// <summary>SDK 1.19 / host 57. Arms a host-owned, read-only live chapter preview after an explicit
    /// user action. The script returns JSON { ok, text, generating }; it must never generate.
    /// Navigation, workspace/context changes, provider unload and Stop preview end the subscription.
    /// Preview updates do not save revisions. Null on older hosts or without browser access.</summary>
    public Action<PluginBrowserFrameScriptRequest>? StartLivePreview { get; init; }
    /// <summary>SDK 1.20 / host 59. Single-use host review of automatic length-based chapter splitting.
    /// Saves a new revision in the captured chapter and inserts remaining chapters after it, preserving
    /// existing chapters/final revisions. Source and workspace are revalidated after review.</summary>
    public Func<string, Func<Task<bool>>, Task<PluginBrowserAutomationResult>>? ReviewAndImportAsChaptersAsync { get; init; }
    /// <summary>SDK 1.21 / host 60. Resolves the current chapter from a complete raw browser story before
    /// the ordinary single-use revision review. Unknown/ambiguous boundaries reject instead of guessing.</summary>
    public Func<string, Func<Task<bool>>, Task<PluginBrowserAutomationResult>>? ReviewAndImportMappedChapterAsync { get; init; }
}

/// <summary>
/// Handles a plugin AI generation paste action invocation.
/// </summary>
public delegate Task PluginAiGenerationPasteActionHandler(PluginAiGenerationPasteActionContext context);
