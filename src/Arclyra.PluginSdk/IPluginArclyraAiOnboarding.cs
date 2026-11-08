namespace Arclyra.PluginSdk;

/// <summary>Optional first-launch setup contract consumed only from the verified Arclyra AI component.</summary>
public interface IPluginArclyraAiOnboarding
{
    /// <summary>Whether the local model, runtime and refusal repair are ready to use.</summary>
    bool IsReadyForOnboarding { get; }

    /// <summary>Prepares Compact and refusal repair without benchmarking, reusing ready local assets.</summary>
    /// <param name="confirm">Authorizes the resolved download and disk requirements before downloading.</param>
    /// <param name="progress">Reports installation stages on the caller's progress dispatcher.</param>
    /// <param name="cancellationToken">Stops setup while retaining resumable downloads.</param>
    Task InstallCompactForOnboardingAsync(
        Func<PluginArclyraAiDownloadPlan, CancellationToken, Task<bool>> confirm,
        IProgress<PluginArclyraAiSetupProgress> progress, CancellationToken cancellationToken);
}

public sealed record PluginArclyraAiDownloadPlan(string Model, long RemainingBytes, long DiskBytes);

public enum PluginArclyraAiSetupStage { Component, Model, Runtime, RefusalRepair }

public sealed record PluginArclyraAiSetupProgress(string Message, double? Fraction = null,
    PluginArclyraAiSetupStage Stage = PluginArclyraAiSetupStage.Component, bool IsReady = false,
    long? CompletedBytes = null, long? TotalBytes = null);
