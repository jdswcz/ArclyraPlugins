# Reusable prompt-entry versions

The SDK 1.22 / host 62 reference baseline supports reusable named alternatives for Writing guidance, World details (`StoryInformation` in API identifiers), Characters, Items and Custom source entries. A source entry owns versions; a persisted story prompt detail selects them as its default or for individual chapters. Versions are shared references, not copied per-chapter text.

## Capabilities and IDs

| Operation | Capability | Identifier |
| --- | --- | --- |
| Discover source entries through `GetPromptSourceEntries` | `story.read` | Source entry `Id` |
| Read versions and usage | `prompt.read` | Source entry ID and version ID |
| Create/update/rename/delete alternatives | `prompt.write` | Source entry ID and version ID |
| Assign/reset a default or chapter version | `prompt.write` | Persisted prompt-detail ID and optional chapter ID |
| Modify source entry fields themselves | `story.write` | Source entry ID |

Never interchange a source ID and a prompt-detail ID. Main has stable ID `main` and is virtual: its text comes from the source entry. Version APIs cannot update, rename or delete Main. Change source fields through the source-entry APIs, or use the host's Details editor.

## Version methods

All methods below are on `IPluginPromptContextService`:

```csharp
IReadOnlyList<PluginPromptEntryVersionDto> GetPromptEntryVersions(string sourceEntryId);
PluginPromptEntryVersionDto CreatePromptEntryVersion(string sourceEntryId, PluginPromptEntryVersionCreateRequest request);
PluginPromptEntryVersionDto UpdatePromptEntryVersion(string sourceEntryId, string versionId, PluginPromptEntryVersionUpdateRequest request);
PluginPromptEntryVersionDto RenamePromptEntryVersion(string sourceEntryId, string versionId, string name);
bool DeletePromptEntryVersion(string sourceEntryId, string versionId);
IReadOnlyList<PluginPromptEntryVersionUsageDto> GetPromptEntryVersionUsage(string sourceEntryId, string versionId);
void SetPromptEntryDefaultVersion(string promptDetailId, string versionId);
void ResetPromptEntryDefaultVersion(string promptDetailId);
void SetPromptEntryChapterVersion(string promptDetailId, string chapterId, string versionId);
void ResetPromptEntryChapterVersion(string promptDetailId, string chapterId);
```

`PluginPromptEntryVersionDto` contains `Id`, `Name`, `PromptText` and `IsMain`. Create requests contain `Name` and `PromptText`; update requests have optional `Name` and `PromptText`, where null leaves a field unchanged. Usage DTOs contain `StoryId`, `PromptDetailId`, nullable `ChapterId` and `IsDefault`.

For an explicitly approved new alternative, using captured IDs from fresh snapshots:

```csharp
var versions = context.PromptContextService;
var alternative = versions.CreatePromptEntryVersion(
    sourceEntryId,
    new PluginPromptEntryVersionCreateRequest("After the reunion", approvedText));

versions.SetPromptEntryChapterVersion(promptDetailId, chapterId, alternative.Id);
```

This creates persisted shared data and assigns it; it is not a transient editor preview. Request only the capabilities needed by the chosen flow. Inspect mutation results and refresh snapshots before subsequent operations. Main and versions with persisted usages cannot be deleted; inspect `GetPromptEntryVersionUsage` and deliberately resolve assignments first. Passing `main` to an assignment method resets that assignment. The reset methods remove the corresponding override using the host's normal version resolution.

## Prompt setup actions are a separate workflow

`RegisterEntryManagementAction` supplies the current **unsaved** Details or alternative-Version editor text. After a full user review, `ApplyDescription` inserts approved text without saving; the writer uses **Save** or **Save changes**. Main remains read-only in Versions. Host 58 groups these registrations under **AI tools**.

SDK 1.15 supplies `CancellationToken`, `CanApplyDescription` and `IsAlternativeVersion`. SDK 1.17 adds nullable `CreateAlternativeVersion(name, text)` for a captured persisted source. It creates one unused alternative without replacing, selecting or assigning existing text, and shares the description writer's single-use invocation. New sources/older hosts can expose null. See [entry actions and guarded creation](ui-extension-api.md#source-bound-alternative-creation-sdk-117--host-55).

On host 62, `MarkPreparationComplete` can release the AI tools busy state after approved browser insertion while retaining the captured pending writer. Starting another action or choosing Cancel pending AI task cancels it. `SelectBrowserProviderAsync` can select only an owned manual provider in the already-open captured window after approval; it does not open a browser or generate. Both callbacks can be null on older hosts.

Use the captured action callback for editor assistance rather than a broad persisted writer. Check freshness after asynchronous work, respect cancellation and handle rejection when the editor, version, source, workspace or plugin changes.

## Older hosts and deprecated adapters

Default interface implementations of reusable-version methods throw `NotSupportedException` on older hosts. Require a compatible host and provide an actionable message; do not silently replace Main or apply text to another source as a fallback.

`SetPersistedPromptDetailCustomization`, `SetPromptDetailChapterCustomization`, `RemovePromptDetailChapterCustomization` and `ReplacePromptDetailChapterCustomizations` remain obsolete compatibility adapters. Their older examples in [Prompt context API](prompt-context-api.md) are for legacy integrations. New code should use reusable versions and assignments instead.
