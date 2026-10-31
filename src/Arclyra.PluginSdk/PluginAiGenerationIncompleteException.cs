namespace Arclyra.PluginSdk;

/// <summary>
/// An interrupted direct generation with an explicitly unapproved preview.
/// This is never a completed result, a conversation turn or permission to save.
/// Hosts may display the exact preview for manual inspection; they must not apply it.
/// </summary>
public sealed class PluginAiGenerationIncompleteException : InvalidOperationException
{
    public PluginAiGenerationIncompleteException(string message, string unapprovedDraft,
        bool wasCanceled = false, Exception? innerException = null) : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(unapprovedDraft);
        if (unapprovedDraft.Length > PluginAiConversation.MaxContentCharacters)
            throw new ArgumentOutOfRangeException(nameof(unapprovedDraft));
        UnapprovedDraft = unapprovedDraft;
        WasCanceled = wasCanceled;
    }

    public string UnapprovedDraft { get; }
    public bool WasCanceled { get; }
}
