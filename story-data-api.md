# Story data API

`IPluginContext.StoryDataService` exposes capability-gated story snapshots and explicit host-owned mutations through `IPluginStoryDataService`. Mutating through the service lets Arclyra preserve observable collection tracking, automatic saves, computed counts, and event publication.

## Required capabilities

- `story.read` for `GetStories`, `GetStory`, global entry reads, and source-entry reads exposed by the prompt context service.
- `story.write` for create/update/delete methods, global writing guidance/custom-entry additions, plugin metadata writes, and source-entry mutations.

## Snapshot reads

```csharp
IReadOnlyList<PluginStoryDto> stories = context.StoryDataService.GetStories();
PluginStoryDto? story = context.StoryDataService.GetStory(storyId);
IReadOnlyList<string> writingGuidance = context.StoryDataService.GetGlobalInstructions();
IReadOnlyList<PluginCharacterDto> characters = context.StoryDataService.GetGlobalCharacters();
IReadOnlyList<PluginItemDto> items = context.StoryDataService.GetGlobalItems();
IReadOnlyList<PluginCustomEntryDto> customEntries = context.StoryDataService.GetGlobalCustomEntries();
```

DTOs are snapshots, not live host objects. Re-read after mutating if you need current data.

## Story and chapter mutations

```csharp
PluginStoryDto created = context.StoryDataService.CreateStory(
    new PluginCreateStoryRequest("New Story", "A short premise."));

PluginStoryDto renamed = context.StoryDataService.UpdateStory(
    new PluginUpdateStoryRequest(created.Id, "Renamed Story", created.BaseStoryInfo));

PluginChapterDto chapter = context.StoryDataService.CreateChapter(
    new PluginCreateChapterRequest(created.Id, "The protagonist discovers the map."));

PluginChapterDto updatedChapter = context.StoryDataService.UpdateChapter(
    new PluginUpdateChapterRequest(
        created.Id,
        chapter.Id,
        Content: "The protagonist follows the map into the storm."));

context.StoryDataService.DeleteChapter(created.Id, updatedChapter.Id);
context.StoryDataService.DeleteStory(created.Id);
```

`PluginCreateChapterRequest.GeneratedContent` and `PluginUpdateChapterRequest.GeneratedContent` replace the generated-draft collection when supplied.

## Entry mutations

Arclyra supports host-owned mutations for writing guidance, characters, items, world details, custom entries, and generated drafts.

```csharp
context.StoryDataService.AddInstruction(
    "Use close third-person narration.",
    storyId: story.Id);

PluginCharacterDto character = context.StoryDataService.CreateCharacter(
    new PluginCreateCharacterRequest(
        "Mira",
        Age: "29",
        Description: "A cartographer who distrusts magic.",
        StoryId: story.Id));

PluginItemDto item = context.StoryDataService.CreateItem(
    new PluginCreateItemRequest(
        "Brass compass",
        ItemInfo: "Points toward the holder's most costly choice.",
        StoryId: story.Id));

PluginWorldDetailDto worldDetail = context.StoryDataService.CreateWorldDetail(
    new PluginCreateWorldDetailRequest(story.Id, "The capital is built around a dormant crater."));

PluginCustomEntryDto custom = context.StoryDataService.CreateCustomEntry(
    new PluginCreateCustomEntryRequest("Scene motif", "Rain on old stone", story.Id));

PluginGeneratedDraftDto draft = context.StoryDataService.CreateGeneratedDraft(
    new PluginCreateGeneratedDraftRequest(story.Id, chapter.Id, "Generated chapter draft text."));
```

Each model family has matching update and delete methods:

| Model family | Create | Update | Delete |
| --- | --- | --- | --- |
| Stories | `CreateStory` | `UpdateStory` | `DeleteStory` |
| Chapters | `CreateChapter` | `UpdateChapter` | `DeleteChapter` |
| Characters | `CreateCharacter` | `UpdateCharacter` | `DeleteCharacter` |
| Items | `CreateItem` | `UpdateItem` | `DeleteItem` |
| World details | `CreateWorldDetail` | `UpdateWorldDetail` | `DeleteWorldDetail` |
| Custom entries | `CreateCustomEntry` | `UpdateCustomEntry` | `DeleteCustomEntry` |
| Generated drafts | `CreateGeneratedDraft` | `UpdateGeneratedDraft` | `DeleteGeneratedDraft` |

For `DeleteCharacter`, `DeleteItem`, and `DeleteCustomEntry`, pass `PluginScopedEntryRequest` with the entry id and optional story id. Omitting the story id targets global entries.

## Plugin-owned metadata

Plugin metadata is key/value data owned by the current plugin. It does not grant access to host story content and should be used for small state such as migration markers, last-used options, or plugin feature flags.

```csharp
IReadOnlyDictionary<string, string> metadata = context.StoryDataService.GetPluginMetadata();

context.StoryDataService.SetPluginMetadata("lastTemplate", "three-act-summary");
context.StoryDataService.SetPluginMetadata("lastTemplate", null); // remove the key
```

## Prompt source entries

`IPluginPromptContextService` exposes a normalized source-entry API for writing guidance, world details (`StoryInformation` in SDK identifiers), characters, items, and custom entries. Use it when your plugin works across entry types.

```csharp
IReadOnlyList<PluginPromptSourceEntryDto> entries =
    context.PromptContextService.GetPromptSourceEntries(
        story.Id,
        PluginGenerationEntryType.Character,
        PluginSelectionScope.Story);

PluginPromptSourceEntryMutationResult result = context.PromptContextService.CreatePromptSourceEntry(
    new PluginPromptSourceEntryCreateRequest(
        PluginGenerationEntryType.StoryInformation,
        PluginSelectionScope.Story,
        "The royal archivists erase maps that show forbidden roads.",
        StoryId: story.Id));
```

`CreatePromptSourceEntry`, `UpdatePromptSourceEntry`, and `DeletePromptSourceEntry` require `story.write` and preserve the same host limits and observable model behavior as the typed story-data methods.

Story mutations can publish story, chapter, generated-draft, and transfer-related notifications; see [Events API](events.md) when your plugin needs to react to host changes.
