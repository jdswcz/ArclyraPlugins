# Capabilities

Capabilities are declared in `plugin.json` and gate SDK services at runtime. They help users and the host understand intended access, but they are not an operating-system sandbox. Request the smallest set that supports the plugin feature.

## Stable capabilities

| Area | Capability | Description | Enables |
| --- | --- | --- | --- |
| Story data | `story.read` | Reads story data. | Read story data snapshots. |
| Story data | `story.write` | Updates story data. | Add or update story data through explicit host-owned mutation APIs. |
| Prompt context | `prompt.read` | Reads prompt context. | Read prompt templates and rendered prompt context. |
| Prompt context | `prompt.write` | Updates prompt templates. | Update prompt templates or prompt context through explicit host APIs. |
| Settings | `settings.read` | Reads settings. | Read host user preference snapshots. |
| AI providers | `aiProviders.read` | Reads AI providers. | Read AI provider configurations. |
| AI providers | `aiProviders.write` | Adds AI providers. | Add, update, or remove plugin-owned AI provider configurations. |
| AI generation | `ai.generation` | Generates AI drafts. | Act as an AI generation bridge and return generated chapter drafts directly to Arclyra. |
| AI generation | `aiGeneration.browserAccess` | Uses audited AI generation browser automation. | Access the live Chromium browser in the AI generation window. |
| Events | `events.subscribe` | Subscribes to events. | Subscribe to the host event bus. Pair with specific event-domain capabilities. See [Events API](events.md). |
| Events | `events.story` | Receives story events. | Subscribe to story lifecycle events. |
| Events | `events.chapter` | Receives chapter events. | Subscribe to chapter lifecycle and content events. |
| Events | `events.generatedDraft` | Receives draft events. | Subscribe to generated draft events. |
| Events | `events.promptDetail` | Receives prompt events. | Subscribe to guided chapter setup prompt detail events. |
| Events | `events.aiProvider` | Receives AI provider events. | Subscribe to AI provider configuration events. |
| Events | `events.transfer` | Receives transfer events. | Subscribe to import/export completion events. |
| UI | `ui.settings` | Contributes settings UI. | Contribute UI to the Arclyra settings experience. |
| UI | `ui.workspacePanel` | Contributes workspace UI. | Contribute dockable or hosted workspace panels. |
| UI | `ui.smartBuilder` | Contributes Smart Builder UI. | Contribute UI directly to guided chapter setup workflows. |
| UI | `ui.editors` | Contributes editor UI. | Replace supported host editing surfaces with plugin-provided editor controls. |
| UI | `ui.storyOverview` | Contributes story overview UI. | Contribute UI to story overview regions. |
| UI | `ui.storyListActions` | Contributes story list actions. | Contribute action buttons to rows in the main story list. |
| UI | `ui.dialogs` | Shows dialogs. | Show simple host-owned dialogs for user-initiated UI prompts. |
| UI | `ui.newStoryWizard` | Contributes new-story wizard steps. | Contribute optional steps to the new-story wizard. |
| UI | `ui.entryManagement` | Contributes entry-management UI. | Contribute UI to prompt setup regions. |
| UI | `ui.aiGenerationPasteActions` | Contributes AI generation paste actions. | Contribute user-invoked actions next to AI generation paste controls. |
| File/native | `fileSystem.pluginDirectory` | Reads plugin files. | Access the path to the plugin installation directory. |
| File/native | `nativeDependencies` | Loads native dependencies. | Load native/unmanaged dependencies from the plugin installation directory. |

## Reserved capabilities

| Area | Capability | Description | Enables |
| --- | --- | --- | --- |
| Network | `network` | Uses network access. | Reserved for future host-mediated network access. Production manifests cannot request this capability until the SDK exposes the corresponding service. |

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
