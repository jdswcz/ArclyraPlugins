namespace Arclyra.PluginSdk;

/// <summary>One host-owned task/language/notes/request review, SDK 1.22 / host 62.</summary>
public sealed record PluginTextTaskSetupRequest(string Title, string Description,
    IReadOnlyList<PluginTextTaskChoice> Choices, string DefaultTaskId, string DefaultLanguage,
    Func<string, string, string, string> ComposePrompt);

public sealed record PluginTextTaskChoice(string Id, string Name);

public sealed record PluginTextTaskSetupResult(bool IsConfirmed, string TaskId, string Language, string Prompt);
