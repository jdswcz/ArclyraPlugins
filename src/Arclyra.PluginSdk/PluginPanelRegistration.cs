using System.Windows;

namespace Arclyra.PluginSdk;

/// <summary>
/// Describes a panel that a plugin contributes to Arclyra.
/// </summary>
/// <remarks>
/// Plugin panels should prefer standard WPF controls and use Arclyra <see cref="FrameworkElement.SetResourceReference" />
/// with shared <c>DynamicResource</c> keys where possible. Arclyra will always place returned controls
/// inside an Arclyra-themed host shell that provides the panel title, plugin owner metadata, and ambient theme resources.
/// </remarks>
/// <param name="PanelId">The stable panel identifier.</param>
/// <param name="DisplayName">The user-facing panel name.</param>
/// <param name="CreateContent">A factory that creates the panel content. Prefer standard WPF controls and theme-aware <c>DynamicResource</c> references where possible.</param>
/// <param name="DefaultDockLocation">The optional preferred dock location for the panel.</param>
public sealed record PluginPanelRegistration(
    string PanelId,
    string DisplayName,
    Func<FrameworkElement> CreateContent,
    string? DefaultDockLocation = null);
