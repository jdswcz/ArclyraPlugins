using System.Windows;

namespace Arclyra.PluginSdk;

/// <summary>
/// Describes a Smart Builder toolbar action contributed by a plugin.
/// </summary>
public sealed record PluginSmartBuilderToolbarRegistration(
    string ActionId,
    string DisplayName,
    Func<PluginSmartBuilderToolbarActionContext, Task> ExecuteAsync,
    int SortOrder = 0,
    string? ToolTip = null);

/// <summary>
/// Describes a Smart Builder sidebar panel contributed by a plugin.
/// </summary>
public sealed record PluginSmartBuilderSidebarRegistration(
    string PanelId,
    string DisplayName,
    Func<FrameworkElement> CreateContent,
    int SortOrder = 0,
    string? Description = null);

/// <summary>
/// Describes Smart Builder content hosted below the prompt rows.
/// </summary>
public sealed record PluginSmartBuilderBelowPromptContentRegistration(
    string ContentId,
    string DisplayName,
    Func<FrameworkElement> CreateContent,
    int SortOrder = 0,
    string? Description = null);

/// <summary>
/// Describes an action shown in the host Smart Builder chapter actions menu.
/// </summary>
public sealed record PluginSmartBuilderChapterActionRegistration(
    string ActionId,
    string DisplayName,
    Func<PluginSmartBuilderChapterActionContext, Task> ExecuteAsync,
    int SortOrder = 0,
    string? ToolTip = null);

/// <summary>
/// Describes an action shown for each Smart Builder prompt row.
/// </summary>
public sealed record PluginPromptRowActionRegistration(
    string ActionId,
    string DisplayName,
    Func<PluginPromptRowActionContext, Task> ExecuteAsync,
    int SortOrder = 0,
    string? ToolTip = null);

/// <summary>
/// Describes a validator invoked during Smart Builder prompt composition.
/// </summary>
public sealed record PluginPromptValidatorRegistration(
    string ValidatorId,
    string DisplayName,
    Func<PluginPromptTransformContext, IReadOnlyList<PluginPromptValidationMessage>> Validate,
    PluginPromptTransformStage Stage = PluginPromptTransformStage.AfterHostComposition,
    int SortOrder = 0);

/// <summary>
/// Describes a transform invoked during Smart Builder prompt composition.
/// </summary>
public sealed record PluginPromptTransformRegistration(
    string TransformId,
    string DisplayName,
    Func<PluginPromptTransformContext, PluginPromptTransformResult> Transform,
    PluginPromptTransformStage Stage = PluginPromptTransformStage.AfterHostComposition,
    int SortOrder = 0);

public enum PluginPromptTransformStage
{
    AfterHostComposition
}

public sealed record PluginSmartBuilderToolbarActionContext(
    string StoryId,
    string StoryName,
    string? ChapterId,
    int? ChapterNumber,
    PluginRenderedPromptDetailsDto Prompt);

public sealed record PluginSmartBuilderChapterActionContext(
    string StoryId,
    string StoryName,
    string? ChapterId,
    int? ChapterNumber,
    string ChapterOutlineText,
    PluginRenderedPromptDetailsDto Prompt);

public sealed record PluginPromptRowActionContext(
    string StoryId,
    string StoryName,
    string? ChapterId,
    int? ChapterNumber,
    PluginRenderedPromptRowDto Row,
    PluginRenderedPromptDetailsDto Prompt);

public sealed record PluginPromptTransformContext(
    string StoryId,
    string StoryName,
    string? ChapterId,
    int? ChapterNumber,
    PluginRenderedPromptDetailsDto Prompt);

public sealed record PluginPromptTransformResult(
    PluginRenderedPromptDetailsDto Prompt,
    IReadOnlyList<PluginPromptValidationMessage>? ValidationMessages = null);

public sealed record PluginPromptValidationMessage(
    PluginPromptValidationSeverity Severity,
    string Message,
    string? Code = null);

public enum PluginPromptValidationSeverity
{
    Information,
    Warning,
    Error
}
