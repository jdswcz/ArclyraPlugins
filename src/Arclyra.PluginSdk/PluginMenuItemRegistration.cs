namespace Arclyra.PluginSdk;

/// <summary>
/// Describes a menu item that a plugin contributes to Arclyra.
/// </summary>
/// <param name="MenuPath">The slash-delimited host menu path where the item should appear.</param>
/// <param name="Header">The user-facing menu item header.</param>
/// <param name="CommandId">The command identifier invoked by the menu item.</param>
/// <param name="SortOrder">The optional ordering value used among sibling menu items.</param>
public sealed record PluginMenuItemRegistration(
    string MenuPath,
    string Header,
    string CommandId,
    int SortOrder = 0);
