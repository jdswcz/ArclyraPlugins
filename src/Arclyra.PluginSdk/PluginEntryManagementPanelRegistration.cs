using System.Windows;

namespace Arclyra.PluginSdk;

/// <summary>
/// Describes a plugin panel hosted in the prompt detail entry-management UI.
/// </summary>
public sealed record PluginEntryManagementPanelRegistration(
    string PanelId,
    string DisplayName,
    Func<PluginEntryManagementPanelContext, FrameworkElement> CreateContent,
    string? Description = null,
    int SortOrder = 0);

/// <summary>
/// Context supplied to plugin panels in the prompt detail entry-management UI.
/// </summary>
public sealed record PluginEntryManagementPanelContext(
    PluginStoryDto? SelectedStory,
    PluginGenerationEntryType ActiveEntryType,
    IReadOnlyList<PluginPromptDetailDto> PromptDetails);
