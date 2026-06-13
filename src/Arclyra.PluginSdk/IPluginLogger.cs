namespace Arclyra.PluginSdk;

/// <summary>
/// Writes diagnostic information from plugins to the Arclyra host.
/// </summary>
public interface IPluginLogger
{
    /// <summary>
    /// Writes an informational diagnostic message.
    /// </summary>
    /// <param name="message">The message to write.</param>
    void LogInformation(string message);

    /// <summary>
    /// Writes a warning diagnostic message.
    /// </summary>
    /// <param name="message">The message to write.</param>
    void LogWarning(string message);

    /// <summary>
    /// Writes an error diagnostic message.
    /// </summary>
    /// <param name="message">The message to write.</param>
    /// <param name="exception">The optional exception associated with the error.</param>
    void LogError(string message, Exception? exception = null);
}
