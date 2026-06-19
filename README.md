# Arclyra plugin documentation

Arclyra plugins extend Arclyra Writing Studio with optional commands, menu items, settings pages, workspace panels, story UI integrations, guided chapter setup (Smart Builder API) tools, AI-provider helpers, and host-mediated data workflows.

This directory is the detailed plugin-authoring reference. The SDK package README is intentionally short and links here for the maintained reference content.

## Start here

1. Read [Terminology](terminology.md) so UI labels and SDK identifiers are used consistently.
2. Review [Capabilities](capabilities.md) before writing a manifest.
3. Use the API page that matches your integration point:
   - [Story data API](story-data-api.md)
   - [Prompt context API](prompt-context-api.md)
   - [Smart Builder API](smart-builder-api.md)
   - [AI generation API](ai-generation-api.md)
   - [UI extension API](ui-extension-api.md)
   - [Host services API](host-services-api.md)
   - [Events API](events.md)
   - [Lifecycle events](lifecycle-events.md)
4. Package and distribute with [Packaging](packaging.md). See also [Plugin signing tool](signtool.md).

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
        context.Logger.LogInformation($"Hello from {Name}.");
        context.CommandRegistry.RegisterCommand(
            new PluginCommandRegistration(
                "hello.showMessage",
                "Show hello message",
                commandCancellationToken =>
                {
                    context.DialogService.ShowInfo(
                        "Hello Arclyra",
                        "The plugin command ran successfully.");
                    return Task.CompletedTask;
                }));

        context.UiRegistry.RegisterMenuItem(
            new PluginMenuItemRegistration("tools.hello", "Hello Arclyra", "hello.showMessage"));

        return Task.CompletedTask;
    }

    public Task ShutdownAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
```

Manifest capabilities for the sample above:

```json
{
  "id": "com.example.arclyra.hello",
  "name": "Hello Arclyra",
  "version": "1.0.0",
  "entryAssembly": "ExamplePlugin.dll",
  "entryType": "ExamplePlugin.Plugin",
  "minHostVersion": "1.0.0",
  "capabilities": [ "ui.dialogs" ]
}
```

## Manifest fields

| Field | Required | Notes |
| --- | --- | --- |
| `id` | Yes | Stable reverse-DNS-style plugin id. Must match `IArclyraPlugin.Id`. |
| `name` | Yes | Display name shown by the host. |
| `version` | Yes | Semantic plugin version from the manifest. |
| `entryAssembly` | Yes | Assembly file inside the package `plugin/` folder. |
| `entryType` | Yes | Public type implementing `IArclyraPlugin`. |
| `minHostVersion` | Recommended | Lowest Arclyra host version the plugin supports. |
| `capabilities` | Recommended | Least-privilege list of SDK capabilities. |
| `description`, `author`, `website`, `license` | Optional | Display/review metadata. |

## Validation command

For documentation-only changes, build the public SDK to catch signature drift that would make examples stale:

```bash
dotnet build Arclyra.PluginSdk/Arclyra.PluginSdk.csproj
```
