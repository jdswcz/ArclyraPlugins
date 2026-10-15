namespace Arclyra.PluginSdk;

/// <summary>
/// Context passed to a plugin AI generator for one guided chapter setup chapter-generation request.
/// </summary>
public sealed record PluginAiGenerationRequest(
    string StoryGuide,
    string ChapterPrompt,
    string? StoryId = null,
    string? ChapterId = null,
    int? ChapterNumber = null)
{
    /// <summary>Optional host-owned story/plugin-scoped persistence for direct generators.</summary>
    public PluginAiConversation? Conversation { get; init; }

    /// <summary>One-time menu choice; null means the generator's saved default. Never persisted by the host.</summary>
    public string? GenerationOptionId { get; init; }
}

/// <summary>
/// One transient user action exposed by a plugin while a direct generation is running.
/// The host invokes only the action instance supplied by the current progress update.
/// </summary>
public sealed record PluginAiGenerationUserAction(
    string Id,
    string Label,
    Func<CancellationToken, Task> ExecuteAsync,
    string? ToolTip = null)
{
    /// <summary>Whether the host should allow invoking this transient action right now.</summary>
    public bool IsEnabled { get; init; } = true;
}

/// <summary>
/// Reports progress text from a plugin AI generator back to Arclyra's generation UI.
/// </summary>
public sealed class PluginAiGenerationProgress
{
    public PluginAiGenerationProgress(string StatusMessage, bool IsGenerating = true)
        : this(StatusMessage, IsGenerating, null)
    {
    }

    public PluginAiGenerationProgress(string StatusMessage, bool IsGenerating, string? GeneratedChapterDraft)
        : this(StatusMessage, IsGenerating, GeneratedChapterDraft, null)
    {
    }

    public PluginAiGenerationProgress(string StatusMessage, bool IsGenerating, string? GeneratedChapterDraft, string? ThinkingTrace)
    {
        this.StatusMessage = StatusMessage;
        this.IsGenerating = IsGenerating;
        this.GeneratedChapterDraft = GeneratedChapterDraft;
        this.ThinkingTrace = ThinkingTrace;
    }

    public string StatusMessage { get; }

    public bool IsGenerating { get; }

    /// <summary>
    /// Optional latest generated chapter draft text for generators that can stream partial output.
    /// </summary>
    public string? GeneratedChapterDraft { get; }

    /// <summary>Optional cumulative provider-supplied reasoning preview. Never chapter content or conversation history.</summary>
    public string? ThinkingTrace { get; }

    /// <summary>
    /// Optional replacement set of transient actions for the current generation stage.
    /// Null leaves the host's current actions unchanged; an empty collection clears them.
    /// </summary>
    public IReadOnlyList<PluginAiGenerationUserAction>? UserActions { get; init; }
}

/// <summary>
/// Result returned by a plugin AI generator.
/// </summary>
public sealed record PluginAiGenerationResult(string GeneratedChapterDraft, string? StatusMessage = null);

/// <summary>
/// Handles a plugin-owned AI generation request.
/// </summary>
public delegate Task<PluginAiGenerationResult> PluginAiGenerationHandler(
    PluginAiGenerationRequest request,
    IProgress<PluginAiGenerationProgress> progress,
    CancellationToken cancellationToken);
