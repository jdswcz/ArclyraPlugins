namespace Arclyra.PluginSdk;

/// <summary>Read-only local review. The host alone presents proposals and applies author approvals.</summary>
public interface IPluginPromptReviewService
{
    /// <summary>Whether local components and a model are ready; false offers host-owned setup.</summary>
    bool IsReady => true;

    /// <summary>
    /// Reports status strings. If progress also implements IProgress&lt;PluginPromptReviewResult&gt;,
    /// the reviewer may report immutable cumulative snapshots of fully validated findings.
    /// Snapshots never authorize applying text; the host must recheck source/cancellation.
    /// The final return remains authoritative and supports hosts without this optional channel.
    /// </summary>
    Task<PluginPromptReviewResult> ReviewAsync(PluginPromptReviewRequest request,
        IProgress<string>? progress = null, CancellationToken cancellationToken = default);
}

public enum PluginPromptReviewScope
{
    EntirePrompt, WritingGuidance, Characters, WorldDetails, Items, ChapterOutline, SelectedDetail, Custom
}

public enum PluginPromptReviewKind { Consistency, Question, CreativeIdea }

/// <summary>Opaque target id; Text is the complete effective detail, without the template's row prefix.</summary>
public sealed record PluginPromptReviewTarget(string Id, PluginPromptReviewScope Section, string Name, string Text);

/// <summary>The complete prompt/guide always accompany the strictly bounded set of editable targets.</summary>
public sealed record PluginPromptReviewRequest(
    string StoryId, string ChapterId, string ChapterPrompt, string StoryGuide,
    PluginPromptReviewScope Scope, IReadOnlyList<PluginPromptReviewTarget> Targets, bool IncludeCreativeSuggestions)
{
    /// <summary>Author's reply or refinement of one proposal. No other target may be returned.</summary>
    public string? AuthorReply { get; init; }
    public PluginPromptReviewSuggestion? PreviousSuggestion { get; init; }
}

public sealed record PluginPromptReviewSuggestion(
    string TargetId, PluginPromptReviewKind Kind, string Title, string Explanation, string? ProposedText);

/// <summary>An empty suggestion list is a successful no-change review, never a transport/parser fallback.</summary>
public sealed record PluginPromptReviewResult(string Summary, IReadOnlyList<PluginPromptReviewSuggestion> Suggestions);
