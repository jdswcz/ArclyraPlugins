# Arclyra.PluginSdk

`Arclyra.PluginSdk` is the public contract package for Arclyra Writing Studio plugins. Plugins implement `IArclyraPlugin`, declare capabilities in `plugin.json`, and use host-provided services from `IPluginContext`.

## Documentation

The detailed plugin-authoring docs live in [`GitHub repository`](https://github.com/jdswcz/ArclyraPlugins):

- [Overview and quick start](../docs/plugins/README.md)
- [Terminology](../docs/plugins/terminology.md)
- [Capabilities](../docs/plugins/capabilities.md)
- [Story data API](../docs/plugins/story-data-api.md)
- [Prompt context API](../docs/plugins/prompt-context-api.md)
- [Smart Builder API](../docs/plugins/smart-builder-api.md)
- [AI generation API](../docs/plugins/ai-generation-api.md)
- [UI extension API](../docs/plugins/ui-extension-api.md)
- [Lifecycle events](../docs/plugins/lifecycle-events.md)
- [Packaging](../docs/plugins/packaging.md)
- [Plugin signing tool](../docs/plugins/signtool.md)

## Minimal plugin

```csharp
using Arclyra.PluginSdk;

namespace ExamplePlugin;

public sealed class Plugin : IArclyraPlugin
{
    public string Id => "com.example.arclyra.hello";
    public string Name => "Hello Arclyra";
    public Task InitializeAsync(IPluginContext context, CancellationToken cancellationToken = default)
    {
        context.Logger.LogInformation("Hello from {PluginName}.", Name);
        return Task.CompletedTask;
    }

    public Task ShutdownAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
```

## Validate SDK examples

After changing SDK contracts or plugin docs, build the SDK project:

```bash
dotnet build Arclyra.PluginSdk/Arclyra.PluginSdk.csproj
```
