namespace Arclyra.PluginSdk;

/// <summary>
/// Replaces the complete provider/paste toolbar while this plugin's browser
/// configuration is selected. Arclyra creates and owns all controls and always
/// supplies an Other providers button. SDK 1.13 / host 51.
/// </summary>
public sealed record PluginAiGenerationToolbarRegistration(
    string ToolbarId,
    string PluginConfigurationId,
    string DisplayName,
    IReadOnlyList<PluginAiGenerationToolbarAction> Actions)
{
    /// <summary>Optional current tool label displayed separately from the plugin name. SDK 1.14.</summary>
    public string? ModeLabel { get; init; }
    /// <summary>SDK 1.23 / host 63. Optional automatic, capability-gated chapter observation setup.
    /// Receives the same scoped context as toolbar actions; never initiates website generation.</summary>
    public PluginAiGenerationPasteActionHandler? PrepareObservationAsync { get; init; }
    /// <summary>SDK 1.23. Groups secondary actions and provider selection in a More menu.</summary>
    public bool UseMoreMenu { get; init; }
}

/// <summary>A user-invoked action rendered by the host in the replacement toolbar.</summary>
public sealed record PluginAiGenerationToolbarAction(
    string ActionId,
    string DisplayName,
    PluginAiGenerationPasteActionHandler ExecuteAsync,
    string? ToolTip = null)
{
    /// <summary>Disable this action when the host has no valid saved chapter import target.</summary>
    public bool RequiresChapterTarget { get; init; }
    /// <summary>SDK 1.31. Allows this explicit start/stop control during an active automation lease.</summary>
    public bool IsAutomationControl { get; init; }
    /// <summary>Optional dynamic caption, evaluated by the host dispatcher.</summary>
    public Func<string>? CurrentDisplayName { get; init; }
    public bool IsSecondary { get; init; }
    /// <summary>SDK 1.26 / host 67. Opt into captured manuscript revision/source validation
    /// for this explicit action. Other toolbar and passive observation actions avoid that capture.</summary>
    public bool RequiresManuscriptContext { get; init; }
    /// <summary>Optional transient visibility, evaluated on the host dispatcher.</summary>
    public Func<bool>? IsVisible { get; init; }
}
