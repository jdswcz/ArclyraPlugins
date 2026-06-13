using System.Windows;

namespace Arclyra.PluginSdk;

/// <summary>
/// Describes a plugin panel hosted in the story overview region.
/// </summary>
public sealed record PluginStoryOverviewPanelRegistration(
    string PanelId,
    string DisplayName,
    Func<PluginStoryOverviewPanelContext, FrameworkElement> CreateContent,
    string? Description = null,
    int SortOrder = 0);

/// <summary>
/// Describes an optional plugin-provided step in the new-story wizard.
/// </summary>
public sealed record PluginNewStoryWizardStepRegistration(
    string StepId,
    string DisplayName,
    Func<PluginNewStoryWizardStepContext, FrameworkElement> CreateContent,
    Func<PluginNewStoryWizardValidationContext, Task<PluginWizardStepValidationResult>>? ValidateAsync = null,
    Func<PluginNewStoryWizardCompletionContext, Task>? ApplyAsync = null,
    string? Description = null,
    int SortOrder = 0,
    bool ShowAsStep = true);

/// <summary>
/// Context supplied to story overview plugin panels.
/// </summary>
public sealed record PluginStoryOverviewPanelContext(PluginStoryDto? CurrentStory, IPluginStoryOverviewWriter? StoryOverview = null);

/// <summary>
/// Describes a plugin button shown beside the built-in actions on each main story-list row.
/// </summary>
public sealed record PluginStoryListItemButtonRegistration(
    string ButtonId,
    string DisplayName,
    Func<PluginStoryListItemButtonContext, Task> ExecuteAsync,
    string? ToolTip = null,
    int SortOrder = 0);

/// <summary>
/// Context supplied when a plugin story-list button is clicked.
/// </summary>
public sealed record PluginStoryListItemButtonContext(PluginStoryDto Story, CancellationToken CancellationToken = default);

/// <summary>
/// Host-mediated editor for the built-in story overview fields.
/// </summary>
public interface IPluginStoryOverviewWriter
{
    string Title { get; set; }

    string Synopsis { get; set; }
}


/// <summary>
/// Snapshot of built-in new-story wizard draft data.
/// </summary>
public sealed record PluginNewStoryWizardDraftDto(
    string StoryId,
    string Title,
    string Premise,
    IReadOnlyList<string> WorldDetails,
    IReadOnlyList<string> WritingGoals,
    string CharacterName,
    string? CharacterAge,
    string? CharacterDescription);

/// <summary>
/// Context supplied when creating plugin wizard-step content.
/// </summary>
public sealed record PluginNewStoryWizardStepContext(PluginNewStoryWizardDraftDto Draft, IPluginNewStoryWizardDraftEditor? DraftEditor = null, string? BuiltInStepId = null);

/// <summary>
/// Context supplied when validating plugin wizard-step input.
/// </summary>
public sealed record PluginNewStoryWizardValidationContext(PluginNewStoryWizardDraftDto Draft, IPluginNewStoryWizardDraftEditor? DraftEditor = null);

/// <summary>
/// Context supplied after Arclyra has created the story and is accepting host-mediated plugin contributions.
/// </summary>
public sealed record PluginNewStoryWizardCompletionContext(
    PluginStoryDto CreatedStory,
    PluginNewStoryWizardDraftDto Draft,
    IPluginNewStoryWizardContributionWriter Contributions,
    IPluginNewStoryWizardDraftEditor? DraftEditor = null);

/// <summary>
/// Host-mediated editor for the built-in new-story wizard fields.
/// </summary>
public interface IPluginNewStoryWizardDraftEditor
{
    string Title { get; set; }

    string Premise { get; set; }

    string WorldDetails { get; set; }

    string WritingGoals { get; set; }

    string CharacterName { get; set; }

    string CharacterAge { get; set; }

    string CharacterDescription { get; set; }

    PluginNewStoryWizardDraftDto GetDraft();
}


/// <summary>
/// Host-mediated writer for plugin data contributed during new-story completion.
/// </summary>
public interface IPluginNewStoryWizardContributionWriter
{
    void AddStoryInformation(string content);

    void AddWritingInstruction(string content);

    void AddCharacter(string name, string? age = null, string? description = null);
}

/// <summary>
/// Validation result returned by plugin wizard steps.
/// </summary>
public sealed record PluginWizardStepValidationResult(bool IsValid, string? Message = null)
{
    public static PluginWizardStepValidationResult Success { get; } = new(true);

    public static PluginWizardStepValidationResult Failure(string message) => new(false, message);
}
