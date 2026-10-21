namespace Arclyra.PluginSdk;

public enum PluginStoryMemoryStatus { Missing = 0, Outdated = 1, Current = 2 }

/// <summary>Whole story, story before BeforeChapterId (exclusive), or one explicit
/// ChapterId/DraftId. Only factual continuity summaries are supported in this minimal API.</summary>
public sealed record PluginStoryMemoryRequest(string StoryId, string? ChapterId = null,
    string? DraftId = null, string? BeforeChapterId = null, int MaximumWords = 500);

/// <summary>Detached immutable memory snapshot. Unknown provenance stays null.</summary>
public sealed record PluginStoryMemoryDto(PluginStoryMemoryStatus Status, string? Id, string? Text,
    string? SourceFingerprint, string? GeneratorProfileFingerprint, string? ModelIdentity,
    string? Language, bool IsManuallyEdited, DateTimeOffset? LastModifiedAtUtc)
{
    public int? FingerprintVersion { get; init; }
    public string? AlgorithmIdentity { get; init; }
    public string? Provenance { get; init; }
    public DateTimeOffset? CreatedAtUtc { get; init; }
}

/// <summary>Optional host-owned memory. Get requires story.read and never runs inference.
/// RequestFresh additionally requires ai.summarize; it reuses current memory or generates
/// locally on demand. Author-edited records cannot be overwritten through this API.</summary>
public interface IPluginStoryMemoryService
{
    Task<PluginStoryMemoryDto> GetAsync(PluginStoryMemoryRequest request,
        CancellationToken cancellationToken = default);
    Task<PluginStoryMemoryDto> RequestFreshAsync(PluginStoryMemoryRequest request,
        IProgress<string>? progress = null, CancellationToken cancellationToken = default);
}
