namespace Arclyra.PluginSdk;

/// <summary>SDK 1.23 / host 63. Factories operate only on the captured owned visible frame.
/// Inspect must return { ok, text, overview, generating }; replacement must acknowledge exact story/overview.</summary>
public sealed record PluginBrowserStoryLoadRequest(string InspectScript,
    Func<PluginBrowserStoryReplacement, string> BuildReplacementScript,
    Func<string, string, PluginBrowserFrameScriptRequest> CreatePreview);
