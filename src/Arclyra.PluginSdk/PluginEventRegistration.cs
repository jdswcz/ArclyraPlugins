namespace Arclyra.PluginSdk;

/// <summary>
/// Represents an active plugin event subscription. Dispose it to unsubscribe.
/// </summary>
public sealed class PluginEventRegistration : IDisposable
{
    private readonly Action _dispose;
    private bool _disposed;

    public PluginEventRegistration(string id, string pluginId, string eventName, Type eventType, Action dispose)
    {
        Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id;
        PluginId = pluginId ?? string.Empty;
        EventName = eventName ?? string.Empty;
        EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
        _dispose = dispose ?? throw new ArgumentNullException(nameof(dispose));
    }

    public string Id { get; }

    public string PluginId { get; }

    public string EventName { get; }

    public Type EventType { get; }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _dispose();
    }
}
