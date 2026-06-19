# Smart Builder API

`IPluginContext.SmartBuilderService` registers guided chapter setup UI contributions, row actions, validators, and transforms through `IPluginSmartBuilderService`.

## Capabilities

- `ui.smartBuilder` for toolbar actions, sidebar panels, below-prompt content, chapter actions, and prompt row actions.
- `prompt.read` for prompt validators.
- `prompt.read` and `prompt.write` for prompt transforms.

## Registration points

```csharp
context.SmartBuilderService.RegisterToolbarAction(
    new PluginSmartBuilderToolbarRegistration(
        "example.refresh",
        "Refresh plugin hints",
        async actionContext => await RefreshHintsAsync(actionContext)));

context.SmartBuilderService.RegisterPromptRowAction(
    new PluginPromptRowActionRegistration(
        "example.inspectRow",
        "Inspect row",
        async rowContext => await InspectRowAsync(rowContext.Row)));
```

Available registration methods are `RegisterToolbarAction`, `RegisterSidebarPanel`, `RegisterBelowPromptContent`, `RegisterChapterAction`, `RegisterPromptRowAction`, `RegisterPromptValidator`, and `RegisterPromptTransform`.

## Validators and transforms

Validators inspect the composed prompt and return `PluginPromptValidationMessage` values.

```csharp
context.SmartBuilderService.RegisterPromptValidator(
    new PluginPromptValidatorRegistration(
        "example.minimumLength",
        "Minimum prompt length",
        transformContext => transformContext.Prompt.RenderedText.Length < 200
            ? [new PluginPromptValidationMessage(
                PluginPromptValidationSeverity.Warning,
                "Prompt is short; consider adding more story context.")]
            : []));
```

Transforms receive `PluginPromptTransformContext` and return `PluginPromptTransformResult`. Use transforms sparingly because they can alter generated-command text.
