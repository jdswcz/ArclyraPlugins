namespace Arclyra.PluginSdk;

/// <summary>Identity of the selected local model, language and summary algorithm.</summary>
public sealed record PluginStorySummaryProfile(string Identity, string ModelName, string LanguageCode);

/// <summary>Complete historical prose, or only newly historical prose when PreviousSummary is supplied.
/// ExpectedIdentity prevents reuse across model/language changes. No manuscript is applied by this service.</summary>
public sealed record PluginStorySummaryRequest(string HistoricalText, string? PreviousSummary,
    int MaximumWords, string ExpectedIdentity);

/// <summary>A completed factual summary bound to the generation profile.</summary>
public sealed record PluginStorySummaryResult(string Text, string Identity);

/// <summary>Host-mediated local Arclyra AI summaries. Requires ai.summarize. Missing components fail
/// without downloading or opening a chapter-generation workflow. Callers review results before applying them.</summary>
public interface IPluginStorySummaryService
{
    Task<PluginStorySummaryProfile> GetProfileAsync(CancellationToken cancellationToken = default);
    Task<PluginStorySummaryResult> SummarizeAsync(PluginStorySummaryRequest request,
        IProgress<string>? progress = null, CancellationToken cancellationToken = default);
}
