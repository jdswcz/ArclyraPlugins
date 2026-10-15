namespace Arclyra.PluginSdk;

/// <summary>A user-approved operation in one direct iframe of the plugin's active manual browser provider.</summary>
public sealed record PluginBrowserFrameScriptRequest(string PluginConfigurationId, string FrameElementId, string Script)
{
    /// <summary>SDK 1.20 / host 59. Optional automatic chapter-length preview target (200–10000 words).
    /// Zero preserves the complete single-chapter preview. Splitting saves nothing until reviewed.</summary>
    public int ChapterPreviewTargetWords { get; init; }
    /// <summary>SDK 1.23. Optional bounded incremental poll factory; receives the last acknowledged revision.
    /// The host invokes it only while the same chapter/browser lease is current.</summary>
    public Func<int?, string>? BuildObservationScript { get; init; }
    /// <summary>SDK 1.23. Exact baseline for loaded-story boundary validation; never a saved revision.</summary>
    public string? InitialBrowserStory { get; init; }
    /// <summary>SDK 1.23. Host-reviewed saved last-chapter prose preceding the newly captured tail.</summary>
    public string? PreviewPrefix { get; init; }
    /// <summary>SDK 1.21 / host 60. Optional stored-chapter mapping and explicitly reviewed edit synchronization.
    /// The read script includes bounded raw story/overview fields. No generation or revision saving is automated.</summary>
    public PluginBrowserChapterSynchronization? ChapterSynchronization { get; init; }
}

/// <summary>Options for a user-armed chapter-aware browser session. SynchronizeEdits requires host review
/// before the first outgoing operation. Subsequent nonconflicting author edits are sent only while idle.</summary>
public sealed record PluginBrowserChapterSynchronization(string ExpectedOverview,
    Func<PluginBrowserStoryReplacement, string> BuildReplacementScript, bool SynchronizeEdits = false, string? InitialStory = null);

/// <summary>A compare-and-replace request for the exact visible source document, including its overview.</summary>
public sealed record PluginBrowserStoryReplacement(string ExpectedStory, string ExpectedOverview, string Text);

/// <summary>Host-owned full-text review. The host enforces the limit before and after editing.</summary>
public sealed record PluginTextReviewRequest(string Title, string Description, string Text, string ConfirmText = "Confirm", bool IsReadOnly = false, int MaximumLength = 262144)
{
    /// <summary>Optional full original for a read-only inline diff. SDK 1.16 / host 54.
    /// Null preserves ordinary review; each input is independently bounded by MaximumLength.
    /// After editing, the author must review the current changes again before confirming.</summary>
    public string? OriginalText { get; init; }
}
