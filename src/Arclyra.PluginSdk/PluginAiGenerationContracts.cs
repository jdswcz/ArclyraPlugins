namespace Arclyra.PluginSdk;

/// <summary>
/// Context passed to a plugin AI generator for one Smart Builder chapter-generation request.
/// </summary>
public sealed record PluginAiGenerationRequest(
    string StoryGuide,
    string ChapterPrompt,
    string? StoryId = null,
    string? ChapterId = null,
    int? ChapterNumber = null);

/// <summary>
/// Reports progress text from a plugin AI generator back to Arclyra's generation UI.
/// </summary>
public sealed record PluginAiGenerationProgress(string StatusMessage, bool IsGenerating = true);

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
