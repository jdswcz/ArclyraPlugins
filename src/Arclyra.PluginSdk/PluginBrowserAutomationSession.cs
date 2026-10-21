namespace Arclyra.PluginSdk;

/// <summary>User-started, host-owned active browser lease. SDK 1.31 / host 77.
/// It outlives the initiating toolbar action and never grants access outside its captured direct frame.</summary>
public interface IPluginBrowserAutomationSession : IDisposable
{
    CancellationToken CancellationToken { get; }
    string InitialChapterText { get; }
    Task<PluginBrowserAutomationResult<string?>> RunFrameScriptAsync(string script, CancellationToken cancellationToken = default);
    /// <summary>Publishes complete unsaved chapter staging. Success acknowledges host acceptance;
    /// await it before the next DOM action. Manual edits revoke the lease instead of being overwritten.</summary>
    Task<PluginBrowserAutomationResult> PublishPreviewAsync(string text, CancellationToken cancellationToken = default);
    void ReportProgress(string status, int words, int target, int cycle);
}

/// <summary>Bounds one explicit active session. MaximumDuration must be between one minute and two hours.</summary>
public sealed record PluginBrowserAutomationSessionRequest(string PluginConfigurationId, string FrameElementId, TimeSpan MaximumDuration);
