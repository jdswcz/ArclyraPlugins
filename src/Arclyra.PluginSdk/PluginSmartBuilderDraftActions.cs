namespace Arclyra.PluginSdk;

/// <summary>Optional SDK 1.4 service, supplied through IPluginContext.SmartBuilderService by host 41+.</summary>
public interface IPluginSmartBuilderDraftActionService
{
    /// <summary>Registers a menu beside Make manual edits. Requires ui.smartBuilder.</summary>
    void RegisterDraftActionMenu(PluginSmartBuilderDraftActionMenuRegistration registration);
}

public sealed record PluginSmartBuilderDraftActionMenuRegistration(
    string MenuId,
    string DisplayName,
    IReadOnlyList<PluginSmartBuilderDraftActionRegistration> Actions,
    int SortOrder = 0,
    string? ToolTip = null);

public sealed record PluginSmartBuilderDraftActionRegistration(
    string ActionId,
    string DisplayName,
    Func<PluginSmartBuilderDraftActionContext, Task> ExecuteAsync);

/// <summary>Current workspace identity and the provenance of its completed, unedited plugin draft.</summary>
public sealed record PluginSmartBuilderDraftActionContext(
    string StoryId,
    string? ChapterId,
    int? ChapterNumber,
    PluginCompletedAiDraft? CompletedDraft)
{
    /// <summary>SDK 1.9 / host 46: lifetime of this action, cancelled when its workspace closes.</summary>
    public CancellationToken CancellationToken { get; init; }

    /// <summary>Checks that the captured completed draft and plugin are still current.</summary>
    public Func<bool>? CanApplyRevision { get; init; }

    /// <summary>Single-use, UI-thread host callback for an explicitly approved complete revision.
    /// Returns false for stale/unloaded/edited drafts. It never writes character/item/story-guide data.</summary>
    public Func<string, bool>? ApplyRevision { get; init; }
}

/// <summary>Transient snapshot; stream previews, failed generations and browser/manual content have no provenance.</summary>
public sealed record PluginCompletedAiDraft(
    string OwnerPluginId,
    string? PluginConfigurationId,
    string SourcePrompt,
    string Output,
    DateTimeOffset CompletedAt);
