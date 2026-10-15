namespace Arclyra.PluginSdk;

/// <summary>
/// Describes a plugin action hosted in Prompt setup Details and alternative Versions.
/// </summary>
public sealed record PluginEntryManagementActionRegistration(
    string ActionId,
    string DisplayName,
    Func<PluginEntryManagementActionContext, Task> ExecuteAsync,
    string? ToolTip = null,
    int SortOrder = 0);

/// <summary>
/// Snapshot of the prompt-detail editor supplied to a plugin action.
/// The host-owned <see cref="ApplyDescription" /> callback replaces only the
/// editable description/info field that was active when the action started.
/// In host 48+ alternative-version editors, Description is the current unsaved
/// version prompt text and ApplyDescription replaces that text without saving.
/// </summary>
public sealed record PluginEntryManagementActionContext(
    PluginStoryDto? SelectedStory,
    PluginGenerationEntryType ActiveEntryType,
    string? EntryId,
    string Name,
    string Description,
    string? Age,
    string? Category,
    Action<string> ApplyDescription)
{
    /// <summary>SDK 1.22 / host 62: release the preparation busy state after insertion while
    /// keeping the guarded task alive for import. Starting another action cancels the old task.</summary>
    public Action? MarkPreparationComplete { get; init; }

    /// <summary>SDK 1.22 / host 62: select an owned manual provider in the already-open
    /// captured browser after request approval. Never opens a browser or starts generation.</summary>
    public Func<string, CancellationToken, Task<PluginBrowserAutomationResult>>? SelectBrowserProviderAsync { get; init; }

    /// <summary>Maximum trimmed name length for the host 55 creation callback.</summary>
    public const int MaximumAlternativeVersionNameLength = 120;

    /// <summary>SDK 1.17 / host 55: optionally creates one unused alternative on the captured
    /// persisted source after explicit user approval. Shares ApplyDescription's single-use
    /// lifetime, rejects Main/duplicate names, and never changes editor text or assignments.
    /// Null for new/unsupported sources and older hosts. No source id may be supplied.</summary>
    public Func<string, string, PluginPromptEntryVersionDto>? CreateAlternativeVersion { get; init; }

    /// <summary>Host 53+ / SDK 1.15: canceled on editor/target changes, unload or invocation completion.</summary>
    public CancellationToken CancellationToken { get; init; }

    /// <summary>Host 53+ / SDK 1.15: UI-thread preflight for this single-use writer. Null on older hosts.
    /// A true result does not replace the authoritative validation inside ApplyDescription.</summary>
    public Func<bool>? CanApplyDescription { get; init; }

    /// <summary>True for the editable alternative-version surface, never the read-only Main view.</summary>
    public bool IsAlternativeVersion { get; init; }
}
