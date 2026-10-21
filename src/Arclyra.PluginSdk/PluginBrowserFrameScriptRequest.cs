namespace Arclyra.PluginSdk;

/// <summary>A user-approved operation in one direct iframe of the plugin's active manual browser provider.</summary>
public sealed record PluginBrowserFrameScriptRequest(string PluginConfigurationId, string FrameElementId, string Script)
{
    /// <summary>SDK 1.24 / host 65. Explicit insertion/loading establishes a discard boundary.
    /// Passive observation must leave this false unless the initial browser story is empty.</summary>
    public bool EstablishChapterDraftBoundary { get; init; }
    /// <summary>SDK 1.24. Exact overview expected by this chapter observer and its discard boundary.</summary>
    public string? ChapterDraftOverview { get; init; }
    /// <summary>SDK 1.20 / host 59. Optional automatic chapter-length preview target (200–10000 words).
    /// Zero preserves the complete single-chapter preview. Splitting saves nothing until reviewed.</summary>
    public int ChapterPreviewTargetWords { get; init; }
    /// <summary>SDK 1.23. Optional bounded incremental poll factory; receives the last acknowledged revision.
    /// The host invokes it only while the same chapter/browser lease is current.</summary>
    public Func<int?, string>? BuildObservationScript { get; init; }
    /// <summary>SDK 1.23. Exact baseline for loaded-story boundary validation; never a saved revision.</summary>
    public string? InitialBrowserStory { get; init; }
    /// <summary>SDK 1.26 / host 67. Optional read-only observer factory for the host-established
    /// earlier-chapter prefix. Called after insertion/load boundary validation, allowing repeated
    /// preparation to capture the complete current draft and count all of its words. The returned
    /// request must retain InitialBrowserStory; it replaces any saved-preview prefix.</summary>
    public Func<string, PluginBrowserFrameScriptRequest>? RebaseChapterObservation { get; init; }
    /// <summary>SDK 1.27 / host 68. Explicitly approved compact working context. The host validates
    /// the invocation revision, preserves/updates unsaved staging and replaces obsolete full-story mapping.</summary>
    public PluginBrowserCompactChapterContext? CompactChapterContext { get; init; }
    /// <summary>SDK 1.23. Host-reviewed saved last-chapter prose preceding the newly captured tail.</summary>
    public string? PreviewPrefix { get; init; }
    /// <summary>SDK 1.21 / host 60. Optional stored-chapter mapping and explicitly reviewed edit synchronization.
    /// The read script includes bounded raw story/overview fields. No generation or revision saving is automated.</summary>
    public PluginBrowserChapterSynchronization? ChapterSynchronization { get; init; }
}

/// <summary>Captured source for author-selected rolling context; null start means unknown chapter mapping.</summary>
public sealed record PluginRollingStorySource(string Text, string CurrentChapterText, int? CurrentChapterStart);

/// <summary>Compact editor lease: current chapter is seeded separately from new browser additions.
/// DiscardReplacement restores only the approved preceding context if the summary covered current prose.</summary>
public sealed record PluginBrowserCompactChapterContext(string ManuscriptRevision, string CurrentChapterText, string DiscardReplacement);

/// <summary>Options for a user-armed chapter-aware browser session. SynchronizeEdits requires host review
/// before the first outgoing operation. Subsequent nonconflicting author edits are sent only while idle.</summary>
public sealed record PluginBrowserChapterSynchronization(string ExpectedOverview,
    Func<PluginBrowserStoryReplacement, string> BuildReplacementScript, bool SynchronizeEdits = false, string? InitialStory = null);

/// <summary>A compare-and-replace request for the exact visible source document, including its overview.</summary>
public sealed record PluginBrowserStoryReplacement(string ExpectedStory, string ExpectedOverview, string Text);

/// <summary>Host-owned full-text review. The host enforces the limit before and after editing.</summary>
public sealed record PluginTextReviewRequest(string Title, string Description, string Text, string ConfirmText = "Confirm", bool IsReadOnly = false, int MaximumLength = 262144)
{
    /// <summary>SDK 1.25 / host 66. Optional checkbox with an alternative editor buffer.
    /// OriginalText comparison and this option are mutually exclusive.</summary>
    public PluginTextReviewOption? Option { get; init; }
    /// <summary>Optional full original for a read-only inline diff. SDK 1.16 / host 54.
    /// Null preserves ordinary review; each input is independently bounded by MaximumLength.
    /// After editing, the author must review the current changes again before confirming.</summary>
    public string? OriginalText { get; init; }
}

/// <summary>Optional review checkbox. Text is used when unchecked; AlternativeText when checked.
/// The host retains independent edits to both buffers until the review closes.</summary>
public sealed record PluginTextReviewOption(string Label, string AlternativeText, bool IsSelectedByDefault = false)
{
    /// <summary>Allow empty selected text when the approved operation also inserts a separate nonempty field.</summary>
    public bool AllowEmptyTextWhenSelected { get; init; }
}
