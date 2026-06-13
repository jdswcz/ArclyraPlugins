namespace Arclyra.PluginSdk;

/// <summary>
/// Allows plugins to subscribe to stable host events.
/// </summary>
public interface IPluginEventBus
{
    /// <summary>
    /// Subscribes to a stable host event name.
    /// Requires the <see cref="PluginCapabilities.EventsSubscribe" /> capability.
    /// </summary>
    PluginEventRegistration Subscribe<TEvent>(
        string eventName,
        Func<PluginEventContext, TEvent, CancellationToken, Task> handler)
        where TEvent : PluginEventDto;

    /// <summary>
    /// Subscribes to a stable host event name with a synchronous handler.
    /// Requires the <see cref="PluginCapabilities.EventsSubscribe" /> capability.
    /// </summary>
    PluginEventRegistration Subscribe<TEvent>(
        string eventName,
        Action<PluginEventContext, TEvent> handler)
        where TEvent : PluginEventDto;
}
