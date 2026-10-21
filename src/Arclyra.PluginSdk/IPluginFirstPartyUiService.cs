using System.Windows;

namespace Arclyra.PluginSdk;

/// <summary>
/// Narrow first-party placement issued only to a host-verified, allowlisted Arclyra AI component.
/// Manifest capabilities and ordinary developer signatures cannot obtain this service.
/// </summary>
public interface IPluginFirstPartyUiService
{
    /// <summary>Registers the single dedicated AI surface, including its setup and settings.
    /// The factory runs on the host UI dispatcher and must return fresh content.</summary>
    void RegisterArclyraAiPanel(Func<FrameworkElement> createContent);

    /// <summary>Requests opening/focusing the host-owned Arclyra AI window.</summary>
    void OpenArclyraAiPanel();

    /// <summary>Registers the verified local summary utility. Revoked on component unload.</summary>
    void RegisterStorySummarizer(IPluginStorySummaryService service)
        => throw new NotSupportedException("Local summary registration requires Arclyra 70 or newer.");
}
