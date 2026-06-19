namespace Arclyra.PluginSdk;

/// <summary>
/// Provides read-only access to coarse host license state for plugins.
/// </summary>
/// <remarks>
/// This service intentionally exposes only whether premium/unlimited access is active. It does not expose serial numbers,
/// license owners, expiration dates, or activation/deactivation operations.
/// </remarks>
public interface IPluginLicenseService
{
    /// <summary>
    /// Gets a value indicating whether premium/unlimited access is active for the current host session.
    /// Trial access is treated as active.
    /// </summary>
    bool IsPremiumActive { get; }
}
