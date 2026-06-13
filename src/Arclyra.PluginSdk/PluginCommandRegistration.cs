namespace Arclyra.PluginSdk;

/// <summary>
/// Describes a command that a plugin exposes to Arclyra.
/// </summary>
/// <param name="CommandId">The stable command identifier.</param>
/// <param name="DisplayName">The user-facing command name.</param>
/// <param name="ExecuteAsync">The callback invoked when the command is executed.</param>
/// <param name="Description">An optional description of what the command does.</param>
public sealed record PluginCommandRegistration(
    string CommandId,
    string DisplayName,
    Func<CancellationToken, Task> ExecuteAsync,
    string? Description = null);
