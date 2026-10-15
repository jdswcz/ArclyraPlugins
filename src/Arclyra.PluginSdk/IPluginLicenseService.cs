namespace Arclyra.PluginSdk;

/// <summary>
/// Provides the legacy Arclyra license-state contract for plugins.
/// </summary>
/// <remarks>
/// Arclyra no longer has a Premium tier. This interface is retained temporarily for binary compatibility with existing plugins.
/// Hosts must report <see langword="true"/> from <see cref="IsPremiumActive"/> and must not contact a licensing service.
/// </remarks>
[Obsolete("Arclyra no longer has a Premium tier. This interface remains only for plugin binary compatibility.")]
public interface IPluginLicenseService
{
    /// <summary>
    /// Gets a compatibility value that is always <see langword="true"/> in Arclyra.
    /// </summary>
    bool IsPremiumActive { get; }
}
