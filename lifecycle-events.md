# Lifecycle and events

Plugins implement `IArclyraPlugin` and are initialized by the host with an `IPluginContext`.

## Lifecycle

```csharp
public sealed class Plugin : IArclyraPlugin
{
    public string Id => "com.example.lifecycle";
    public string Name => "Lifecycle Example";
    public Task InitializeAsync(IPluginContext context, CancellationToken cancellationToken = default)
    {
        context.Logger.LogInformation($"Initialized {Id} on Arclyra {context.HostVersion}.");
        return Task.CompletedTask;
    }

    public Task ShutdownAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
```

Shutdown is best-effort. Release event subscriptions, timers, background tasks, cancellation sources, WPF controls, delegates, static references, and unmanaged resources.

## Context services

`IPluginContext` exposes `HostVersion`, `PluginDirectory`, `Logger`, `CommandRegistry`, `UiRegistry`, `DialogService`, `EventBus`, `AiProviderService`, `StoryDataService`, `PromptContextService`, `SmartBuilderService`, `PreferencesService`, `LicenseService`, and `AiGenerationWindowService`.

## Events

See the full [Events API](events.md) for event names, required capabilities, DTOs, and subscription patterns.

Capability: `events.subscribe`, plus one or more domain capabilities such as `events.story` or `events.promptDetail`.

```csharp
PluginEventRegistration subscription = context.EventBus.Subscribe<PluginPromptDetailEvent>(
    PluginEventNames.PromptDetailAdded,
    (eventContext, evt) => context.Logger.LogInformation($"Prompt detail added: {evt.PromptDetailId}"));
```

Stable event DTOs include `PluginStoryEvent`, `PluginChapterEvent`, `PluginChapterContentEvent`, `PluginGeneratedDraftEvent`, `PluginPromptDetailEvent`, `PluginAiProviderEvent`, `PluginUserPreferencesChangedEvent`, `PluginTransferEvent`, `PluginEditorEvent`, and `PluginAiGenerationWindowEvent`.
