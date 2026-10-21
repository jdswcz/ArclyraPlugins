namespace Arclyra.PluginSdk;

/// <summary>SDK 1.24 / host 65. Discards only the source-bound current browser chapter.
/// Inspect returns { ok, text, overview, generating }; replacement acknowledges exact story/overview.
/// The host clears unsaved staging only after an idle, verified compare-and-replace.</summary>
public sealed record PluginBrowserChapterDiscardRequest(string InspectScript,
    Func<PluginBrowserStoryReplacement, string> BuildReplacementScript,
    Func<string, string, PluginBrowserFrameScriptRequest> CreatePreview);
