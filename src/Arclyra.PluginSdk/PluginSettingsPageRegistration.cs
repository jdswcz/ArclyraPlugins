using System.Windows;

namespace Arclyra.PluginSdk;

/// <summary>
/// Describes a settings page that a plugin contributes to Arclyra.
/// </summary>
/// <remarks>
/// Plugin settings pages should prefer standard WPF controls and use Arclyra <see cref="FrameworkElement.SetResourceReference" />
/// with shared <c>DynamicResource</c> keys where possible. Arclyra will always place returned controls
/// inside an Arclyra-themed host shell that provides the page title, plugin owner metadata, and ambient theme resources.
/// </remarks>
/// <param name="PageId">The stable settings page identifier.</param>
/// <param name="DisplayName">The user-facing settings page name.</param>
/// <param name="CreateContent">A factory that creates the settings page content. Prefer standard WPF controls and theme-aware <c>DynamicResource</c> references where possible.</param>
/// <param name="SortOrder">The optional ordering value used among settings pages.</param>
public sealed record PluginSettingsPageRegistration(
    string PageId,
    string DisplayName,
    Func<FrameworkElement> CreateContent,
    int SortOrder = 0);
