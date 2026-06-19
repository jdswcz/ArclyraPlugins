# Plugin terminology

Use user-facing Arclyra terms in plugin UI, and use SDK identifiers only when documenting code or manifest details.

| UI term | SDK identifiers | Meaning |
| --- | --- | --- |
| World details | `PluginStoryDto.StoryInformation`, `PluginWorldDetailDto`, `PluginGenerationEntryType.StoryInformation` | Setting, lore, rules, background facts, and worldbuilding entries. |
| Writing guidance | `PluginStoryDto.Instructions`, `IPluginStoryDataService.GetGlobalInstructions`, `AddInstruction`, `PluginGenerationEntryType.Instruction` | Voice, style, pacing, restrictions, and other writing directions. |
| Chapter outline | `PluginChapterDto.Content`, `PluginPromptRowSource.ChapterPlotDescription` | Planned events, goals, and turning points for a chapter. |
| Prompt setup | `IPluginUiRegistry.RegisterEntryManagementPanel`, `PluginEntryManagementPanelRegistration`, `PluginCapabilities.UiEntryManagement` | The UI area for organizing reusable prompt details. |
| Guided chapter setup | `IPluginSmartBuilderService`, `IPluginPromptContextService`, `PluginPromptDetailDto` | The primary chapter prompt-building workflow. SDK names retain Smart Builder for compatibility. |
| Prompt detail | `PluginGenerationSelectionDto`, `PluginPromptDetailDto`, `PluginPromptSourceEntryDto` | A reusable selected detail included in generated prompts. |
| Generated draft | `PluginGeneratedDraftDto` | A generated chapter draft attached to a chapter. |

Compatibility names such as `Instruction` and `StoryInformation` are intentionally preserved in SDK contracts and persisted data. Do not rename them in manifests or code.
