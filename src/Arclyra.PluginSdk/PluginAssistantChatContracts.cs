namespace Arclyra.PluginSdk;

/// <summary>Read-only, host-selected evidence. Source text is data, never executable instructions.</summary>
public sealed record PluginAssistantChatSource(string Id, string Title, string Text);
public sealed record PluginAssistantChatTurn(string Role, string Text);
public sealed record PluginAssistantChatRequest(string ApplicationState,
    IReadOnlyList<PluginAssistantChatSource> Help, IReadOnlyList<PluginAssistantChatSource> StorySources,
    IReadOnlyList<PluginAssistantChatTurn> History, string Question)
{
    /// <summary>Compatibility property. Kitty chat backends must not use this flag to change factual inference;
    /// cute presentation is applied by the host UI, outside model inputs and conversation history.</summary>
    public bool CuteReplies { get; init; }
}
public sealed record PluginAssistantChatProgress(string Status, string? Text = null);
public sealed record PluginAssistantChatResult(string Text, string ModelIdentity, bool IsComplete,
    IReadOnlyList<string> IncludedSourceIds, int OmittedHistoryTurns);

/// <summary>Only a verified first-party component can register this local, cancellable backend.
/// It grants no story access or write authority. Opening the UI never invokes this service.</summary>
public interface IPluginAssistantChatService
{
    bool IsReady { get; }
    Task<PluginAssistantChatResult> ReplyAsync(PluginAssistantChatRequest request,
        IProgress<PluginAssistantChatProgress>? progress = null, CancellationToken cancellationToken = default);
}
