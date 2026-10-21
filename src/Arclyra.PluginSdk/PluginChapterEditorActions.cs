namespace Arclyra.PluginSdk;

/// <summary>Describes how a chapter editor action may change the captured document.</summary>
public enum PluginChapterEditorEditKind
{
    InsertAtCaret,
    ReplaceSelection,
    /// <summary>Host 71: replace the exact captured selection, or the complete captured
    /// document when selection length is zero. Whitespace-only targets are rejected.</summary>
    ReplaceSelectionOrDocument
}

/// <summary>Describes the outcome of a host-mediated editor change.</summary>
public enum PluginChapterEditorApplyStatus
{
    Applied,
    StaleDocument,
    EditorClosed,
    PluginUnavailable,
    InvalidReplacement,
    AlreadyUsed
}

public sealed record PluginChapterEditorApplyResult(
    PluginChapterEditorApplyStatus Status,
    string? Message = null)
{
    public bool IsApplied => Status == PluginChapterEditorApplyStatus.Applied;
}

/// <summary>Immutable text and selection state captured when the writer invokes an action.</summary>
public sealed class PluginChapterEditorSnapshot
{
    public const int MaximumContentLength = 262_144;
    public const int MaximumReplacementLength = 262_144;

    public PluginChapterEditorSnapshot(
        string storyId,
        string storyName,
        string chapterId,
        int chapterNumber,
        string documentId,
        string documentRevision,
        string content,
        int selectionStart,
        int selectionLength,
        int caretOffset)
    {
        StoryId = RequireValue(storyId, nameof(storyId));
        StoryName = RequireValue(storyName, nameof(storyName));
        ChapterId = RequireValue(chapterId, nameof(chapterId));
        if (chapterNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(chapterNumber));
        ChapterNumber = chapterNumber;
        DocumentId = RequireValue(documentId, nameof(documentId));
        DocumentRevision = RequireValue(documentRevision, nameof(documentRevision));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        if (Content.Length > MaximumContentLength)
            throw new ArgumentOutOfRangeException(nameof(content), $"Editor content cannot exceed {MaximumContentLength} characters.");
        if (selectionStart < 0 || selectionStart > Content.Length)
            throw new ArgumentOutOfRangeException(nameof(selectionStart));
        if (selectionLength < 0 || selectionLength > Content.Length - selectionStart)
            throw new ArgumentOutOfRangeException(nameof(selectionLength));
        if (caretOffset < 0 || caretOffset > Content.Length)
            throw new ArgumentOutOfRangeException(nameof(caretOffset));

        SelectionStart = selectionStart;
        SelectionLength = selectionLength;
        CaretOffset = caretOffset;
        SelectedText = Content.Substring(selectionStart, selectionLength);
    }

    public string StoryId { get; }
    public string StoryName { get; }
    public string ChapterId { get; }
    public int ChapterNumber { get; }
    public string DocumentId { get; }
    public string DocumentRevision { get; }
    public string Content { get; }
    public int SelectionStart { get; }
    public int SelectionLength { get; }
    public int CaretOffset { get; }
    public string SelectedText { get; }

    private static string RequireValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A non-empty value is required.", parameterName);
        return value;
    }
}

/// <summary>Invocation state for one chapter editor action. The host-bound apply callback is single-use.</summary>
public sealed class PluginChapterEditorActionContext
{
    private Func<string, CancellationToken, Task<PluginChapterEditorApplyResult>>? _applyAsync;

    public PluginChapterEditorActionContext(
        PluginChapterEditorSnapshot snapshot,
        Func<string, CancellationToken, Task<PluginChapterEditorApplyResult>> applyAsync)
    {
        Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        _applyAsync = applyAsync ?? throw new ArgumentNullException(nameof(applyAsync));
    }

    public PluginChapterEditorSnapshot Snapshot { get; }

    /// <summary>SDK 1.18 / host 56: optional read-only preflight of this invocation's
    /// workspace, registration, document/revision/content and selection/caret.
    /// False after completion/close; null on older hosts. Never consumes ApplyAsync.</summary>
    public Func<bool>? IsSnapshotCurrent { get; init; }
    /// <summary>SDK 1.23 / host 63. Selects an owned manual provider in the captured already-open browser,
    /// after user request approval; selection/document/workspace guards remain authoritative.</summary>
    public Func<string, CancellationToken, Task<PluginBrowserAutomationResult>>? SelectBrowserProviderAsync { get; init; }

    public Task<PluginChapterEditorApplyResult> ApplyAsync(
        string replacementText,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(replacementText);
        if (replacementText.Length > PluginChapterEditorSnapshot.MaximumReplacementLength)
            throw new ArgumentOutOfRangeException(nameof(replacementText),
                $"Replacement text cannot exceed {PluginChapterEditorSnapshot.MaximumReplacementLength} characters.");

        Func<string, CancellationToken, Task<PluginChapterEditorApplyResult>>? callback =
            Interlocked.Exchange(ref _applyAsync, null);
        if (callback == null)
        {
            return Task.FromResult(new PluginChapterEditorApplyResult(
                PluginChapterEditorApplyStatus.AlreadyUsed,
                "This editor change was already submitted."));
        }

        return callback(replacementText, cancellationToken);
    }
}

public sealed record PluginChapterEditorActionRegistration(
    string ActionId,
    string DisplayName,
    PluginChapterEditorEditKind EditKind,
    bool RequiresSelection,
    Func<PluginChapterEditorActionContext, CancellationToken, Task> ExecuteAsync);

public sealed record PluginChapterEditorActionMenuRegistration(
    string MenuId,
    string DisplayName,
    IReadOnlyList<PluginChapterEditorActionRegistration> Actions);

/// <summary>Optional capability-gated service for actions in Arclyra's built-in chapter editor.</summary>
public interface IPluginChapterEditorActionService
{
    void RegisterActionMenu(PluginChapterEditorActionMenuRegistration registration);
}
