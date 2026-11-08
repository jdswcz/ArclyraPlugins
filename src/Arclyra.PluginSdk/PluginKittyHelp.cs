namespace Arclyra.PluginSdk;

/// <summary>A plain-text usage topic about the registering plugin.</summary>
/// <param name="Id">A stable, plugin-local slug of up to 64 lowercase letters, digits or hyphens.</param>
/// <param name="Title">The user-facing topic title, up to 120 characters.</param>
/// <param name="Keywords">Search terms and aliases, up to 500 characters.</param>
/// <param name="Text">Public usage instructions, up to 2,000 characters.</param>
public sealed record PluginKittyHelpTopic(string Id, string Title, string Keywords, string Text);

/// <summary>Supplies static plugin documentation to Arclyra Kitty.</summary>
public interface IPluginKittyHelpRegistry
{
    /// <summary>Registers a topic during plugin initialization. A plugin can supply up to 24 unique topics.</summary>
    void Register(PluginKittyHelpTopic topic);
}
