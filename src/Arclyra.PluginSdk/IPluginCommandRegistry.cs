namespace Arclyra.PluginSdk;

/// <summary>
/// Registers executable plugin commands with the Arclyra host.
/// </summary>
public interface IPluginCommandRegistry
{
    /// <summary>
    /// Registers a command contributed by a plugin.
    /// </summary>
    /// <param name="registration">The command registration details.</param>
    void RegisterCommand(PluginCommandRegistration registration);
}
