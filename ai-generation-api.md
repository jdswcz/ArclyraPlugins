# AI generation API

Plugins can integrate with AI generation in three ways: direct generation handlers, paste actions, and audited browser automation.

## Direct AI generation

Capability: `ai.generation`.

```csharp
context.AiProviderService.AddOrUpdateGenerator(
    new PluginAiConfiguration(
        Id: "com.example.generator",
        Name: "Example Generator",
        Url: "https://example.invalid",
        PromptPasteScript: string.Empty,
        ReadGeneratedChapterScript: string.Empty,
        PluginConfigurationId: "example.generator"),
    async (request, progress, cancellationToken) =>
    {
        progress.Report(new PluginAiGenerationProgress("Generating draft…"));
        string draft = await GenerateDraftAsync(request.ChapterPrompt, cancellationToken);
        return new PluginAiGenerationResult(draft, "Draft generated.");
    });
```

## AI provider configurations

Capabilities: `aiProviders.read` for snapshots and `aiProviders.write` for plugin-owned configuration mutations.

`IPluginContext.AiProviderService` exposes the current provider list, raises `ConfigurationsChanged` when provider settings change, and lets a plugin add, update, or remove only the configurations it owns. Host/user-owned configurations can be read with `aiProviders.read` but not removed by plugins.

```csharp
IReadOnlyList<PluginAiConfiguration> providers =
    context.AiProviderService.GetConfigurations();

PluginAiConfiguration saved = context.AiProviderService.AddOrUpdateConfiguration(
    new PluginAiConfiguration(
        Id: string.Empty,
        Name: "Example Browser Provider",
        Url: "https://example.invalid",
        PromptPasteScript: "/* host-approved paste script */",
        ReadGeneratedChapterScript: "/* host-approved read script */",
        PluginConfigurationId: "example.browserProvider"));

bool removed = context.AiProviderService.RemovePluginConfiguration(saved.Id);
```

Use `PluginAiConfiguration.PluginConfigurationId` as your stable provider id. Arclyra populates `Id` and `PluginOwnerId` when it persists the plugin-owned configuration.

## Paste actions

Capability: `ui.aiGenerationPasteActions`.

```csharp
context.UiRegistry.RegisterAiGenerationPasteAction(
    new PluginAiGenerationPasteActionRegistration(
        "example.copyPromptSummary",
        "Copy prompt summary",
        "Copies a short summary of the current rendered prompt.",
        0,
        async pasteContext => await CopySummaryAsync(pasteContext.CurrentRenderedPrompt, pasteContext.CancellationToken)));
```

The paste-action context exposes provider/story/prompt snapshots and a cancellation token. It does not expose the live browser.

## Browser automation

Capability: `aiGeneration.browserAccess`.

Use `IPluginContext.AiGenerationWindowService` for audited, host-mediated operations such as `IsWindowOpen`, `SelectedProvider`, `ReadSelectedProviderAsync`, `PasteCurrentPromptAsync`, `TriggerGenerationAsync`, `ReadGeneratedContentAsync`, and `RunApprovedScriptAsync`.

AI provider and AI generation window notifications are documented in the [Events API](events.md).
