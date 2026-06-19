# Capabilities

Capabilities are declared in `plugin.json` and gate SDK services at runtime. They help users and the host understand intended access, but they are not an operating-system sandbox. Request the smallest set that supports the plugin feature.

## Stable capabilities

| Area | Capability | Enables |
| --- | --- | --- |
| Story data | `story.read` | Read story, chapter, character, item, world detail, custom entry, and generated draft snapshots. |
| Story data | `story.write` | Mutate story data through host-owned APIs. |
| Prompt context | `prompt.read` | Read chapter prompt templates, rendered prompts, and prompt details. |
| Prompt context | `prompt.write` | Create, update, delete, reorder, scope, or customize prompt details. |
| Settings | `settings.read` | Read host user preference snapshots. |
| AI providers | `aiProviders.read` | Read AI provider configurations. |
| AI providers | `aiProviders.write` | Add, update, or remove plugin-owned AI provider configurations. |
| AI generation | `ai.generation` | Register a direct AI generation handler. |
| AI generation | `aiGeneration.browserAccess` | Use audited host-mediated AI generation window browser automation. |
| Events | `events.subscribe` | Subscribe to the event bus. Pair with specific event-domain capabilities. See [Events API](events.md). |
| Events | `events.story`, `events.chapter`, `events.generatedDraft`, `events.promptDetail`, `events.aiProvider`, `events.transfer` | Subscribe to specific event domains. |
| UI | `ui.settings`, `ui.workspacePanel`, `ui.smartBuilder`, `ui.editors`, `ui.storyOverview`, `ui.storyListActions`, `ui.dialogs`, `ui.newStoryWizard`, `ui.entryManagement`, `ui.aiGenerationPasteActions` | Register the corresponding host UI contribution. |
| File/native | `fileSystem.pluginDirectory` | Read `IPluginContext.PluginDirectory`. |
| File/native | `nativeDependencies` | Load native/unmanaged dependencies from the plugin directory. |

## Reserved capabilities

`network` is reserved for future host-mediated network access. Production manifests cannot request it until the SDK exposes a corresponding service.

## Example

```json
{
  "capabilities": [
    "story.read",
    "prompt.read",
    "ui.smartBuilder",
    "events.subscribe",
    "events.promptDetail"
  ]
}
```
