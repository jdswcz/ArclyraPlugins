# Prompt context API examples

The prompt context API lets plugins inspect and extend the guided chapter setup prompt details that Arclyra uses when building chapter-generation prompts. The API is exposed through `IPluginContext.PromptContextService`.

## Required manifest capabilities

Every example below assumes a manifest that can read stories, read prompt details, and write prompt details:

```json
{
  "id": "com.example.arclyra.plugins.prompt-context",
  "name": "Prompt Context Example",
  "version": "0.1.0",
  "entryAssembly": "PromptContextExample.dll",
  "entryType": "PromptContextExample.Plugin",
  "minHostVersion": "1.0.0",
  "capabilities": [
    "story.read",
    "prompt.read",
    "prompt.write"
  ]
}
```

Use only `prompt.read` when you only call read APIs. Add `prompt.write` for creating, updating, deleting, scoping, enabling/disabling, runtime details, or chapter customizations. Add `story.read` when your plugin discovers story or chapter ids through `StoryDataService`.

## Shared setup

```csharp
PluginStoryDto story = context.StoryDataService.GetStories().First();
string? chapterId = story.Chapters.FirstOrDefault()?.Id;
```

## 1. Read prompt details added to a story

```csharp
IReadOnlyList<PluginPromptDetailDto> allDetails =
    context.PromptContextService.GetPromptDetails(story.Id);

IReadOnlyList<PluginPromptDetailDto> persistedOnly =
    context.PromptContextService.GetPersistedPromptDetails(story.Id);

IReadOnlyList<PluginPromptDetailDto> runtimeOnly =
    context.PromptContextService.GetRuntimePromptDetails(story.Id);
```

Pass a chapter id and optional `PluginSelectionScope` to filter the returned details for one chapter or scope.

## 2. Create a persisted prompt detail

Persisted prompt details are saved to the story data file and appear in Arclyra's prompt detail UI.

```csharp
PluginPromptDetailDto persisted = context.PromptContextService.CreatePromptDetail(
    story.Id,
    new PluginPromptDetailCreateRequest(
        PluginGenerationEntryType.Instruction,
        "Keep the scene goal, conflict, and outcome visible in every beat.",
        "Plugin-created writing guidance",
        PluginSelectionScope.Story));
```

## 3. Create a runtime plugin-only prompt detail

Runtime prompt details can be included in rendered prompts, but they are owned by the plugin and are not saved to the story data file.

```csharp
PluginPromptDetailDto runtimeOnly = context.PromptContextService.AddOrUpdateRuntimePromptDetail(
    story.Id,
    new PluginPromptDetailCreateRequest(
        PluginGenerationEntryType.Custom,
        "Plugin runtime checklist",
        "Open with a sensory detail; end with a changed situation.",
        PluginSelectionScope.Story),
    $"com.example.arclyra.plugins.prompt-context.runtimeChecklist.{story.Id}");
```

Remove a runtime contribution when it should no longer participate in prompts:

```csharp
context.PromptContextService.RemoveRuntimePromptDetail(runtimeOnly.Id);
```

## 4. Change whole-story vs chapter-specific scope

Use `UpdatePromptDetailScope` or `UpdatePersistedPromptDetailScope` when only the target scope changes.

```csharp
context.PromptContextService.UpdatePromptDetailScope(
    story.Id,
    persisted.Id,
    new PluginPromptDetailScopeUpdateRequest(
        Scope: PluginSelectionScope.Story,
        IsChapterScoped: false));
```

To make the same persisted detail chapter-specific:

```csharp
if (chapterId != null)
{
    context.PromptContextService.UpdatePromptDetailScope(
        story.Id,
        persisted.Id,
        new PluginPromptDetailScopeUpdateRequest(
            Scope: PluginSelectionScope.Story,
            ChapterIds: [chapterId],
            IsChapterScoped: true,
            ChapterScopeMode: PluginChapterScopeMode.SelectedChapters));
}
```

You can also read just the scope state:

```csharp
PluginPromptDetailScopeDto scope =
    context.PromptContextService.GetPromptDetailScope(story.Id, persisted.Id);
```

## 5. Add or remove a chapter customization

A chapter customization overrides the prompt text for a persisted detail in one chapter while keeping the base detail intact.

```csharp
if (chapterId != null)
{
    context.PromptContextService.SetPromptDetailChapterCustomization(
        story.Id,
        persisted.Id,
        chapterId,
        "For this chapter, emphasize the protagonist's hardest choice.");

    context.PromptContextService.RemovePromptDetailChapterCustomization(
        story.Id,
        persisted.Id,
        chapterId);
}
```

The request form is useful when a plugin UI submits either text or removal through one path:

```csharp
context.PromptContextService.SetPersistedPromptDetailCustomization(
    story.Id,
    persisted.Id,
    new PluginPromptDetailCustomizationRequest(chapterId!, null));
```

## 6. Delete a prompt detail

```csharp
bool deleted = context.PromptContextService.DeletePromptDetail(story.Id, persisted.Id);
```

`DeletePromptDetail` applies to persisted host prompt details. Use `RemoveRuntimePromptDetail` for runtime plugin-only details.

## 7. Update prompt detail text and reorder entries

Use `UpdatePromptDetail` when a plugin intentionally updates text and scope in one operation. Use the narrower persisted helpers when only one facet changes.

```csharp
PluginPromptDetailDto updated = context.PromptContextService.UpdatePromptDetail(
    story.Id,
    persisted.Id,
    new PluginPromptDetailUpdateRequest(
        Content: "Keep the protagonist's scene goal explicit.",
        Detail: "Plugin-updated writing guidance"));

PluginPromptDetailMutationResult textResult = context.PromptContextService.UpdatePersistedPromptDetailText(
    story.Id,
    persisted.Id,
    new PluginPromptDetailUpdateRequest(Content: "Emphasize the hard choice in each beat."));
```

Prompt details can be reordered within one entry type by passing the desired id order:

```csharp
IReadOnlyList<PluginPromptDetailDto> reordered = context.PromptContextService.ReorderPromptDetails(
    story.Id,
    PluginGenerationEntryType.Instruction,
    context.PromptContextService
        .GetPersistedPromptDetails(story.Id)
        .Where(detail => detail.EntryType == PluginGenerationEntryType.Instruction)
        .Select(detail => detail.Id)
        .Reverse()
        .ToList());
```

## 8. Replace all chapter customizations

```csharp
if (chapterId != null)
{
    context.PromptContextService.ReplacePromptDetailChapterCustomizations(
        story.Id,
        persisted.Id,
        new Dictionary<string, string>
        {
            [chapterId] = "For this chapter, focus on the cost of telling the truth."
        });
}
```

`ReplacePromptDetailChapterCustomizations` removes existing chapter customizations that are not present in the supplied dictionary.

## 9. Enable or disable a prompt detail for one chapter

```csharp
if (chapterId != null)
{
    context.PromptContextService.SetPromptDetailEnabledForChapter(
        story.Id,
        persisted.Id,
        chapterId,
        enabled: false);
}
```

This is a convenience for chapter-specific inclusion/exclusion. Use scope APIs when changing the detail's overall chapter targeting model.

## 10. Read templates and render prompt context

```csharp
IReadOnlyList<PluginChapterPromptTemplateDto> templates =
    context.PromptContextService.GetChapterPromptTemplates();

PluginChapterPromptTemplateDto? template =
    context.PromptContextService.GetChapterPromptTemplate(story.Id);

PluginRenderedPromptDetailsDto rendered = context.PromptContextService.RenderChapterPrompt(
    story.Id,
    chapterId,
    chapterContentOverride: "Temporary outline text for preview only.");
```

`RenderChapterPrompt` returns the rendered prompt text, row snapshots, counts by entry type, placeholder metadata, and chapter-plot rows. The optional chapter-content override is for preview/composition scenarios and does not mutate the chapter.

Prompt detail changes can publish prompt-detail notifications; see [Events API](events.md) for event names and DTOs.
