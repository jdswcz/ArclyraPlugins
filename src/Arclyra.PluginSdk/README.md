# Arclyra Plugin SDK

**SDK 1.39.0** provides .NET 8 Windows/WPF contracts, DTOs, helpers and MSBuild packaging targets. Assembly identity remains **1.2.0.0**, and file version is **1.39.0.0**.

The C# sources, project metadata and packaging targets are synchronized with read-only `jdswcz/ArclyraWritingStudio` dev commit `520e219a77c708a362142c323a42a3c57e3bfe1b` (November 8, 2026). This repository-specific README replaces the upstream package README. This does not publish a NuGet package.

| SDK | Minimum host | New contracts |
| --- | --- | --- |
| 1.32 | 86 | `IPluginStorySummaryProgress`, `PluginStorySummaryProgress`: optional transient local summary preview and completed-source counts. |
| 1.33 | 92 | `RequireCurrentSourceSnapshot`, request validity predicate and cancellation, `PluginAiGenerationIncompleteException` for unapproved interrupted previews. Host 93 adds reviewed restart decisions. |
| 1.34 | 102 | `IPluginPromptReviewService` and related request/target/suggestion/result contracts, registered only via trusted `FirstPartyUiService`. Host 104 adds optional cumulative validated result progress. |
| 1.35 | 115 | `PluginChapterDto.CurrentDraftId` identifies the saved structured revision behind `CurrentDraftContent`; null for legacy-only text. |
| 1.36 | 134 | `IPluginArclyraAiOnboarding` first-launch setup, verified first party only. |
| 1.37 | 136 | Setup progress can report `RefusalRepair`; trusted install prepares CPU fallback and managed repair components. |
| 1.38 | 140 | `IPluginAssistantChatService` local read-only Kitty chat, verified first party only. |
| 1.39 | 150 | `IPluginContext.KittyHelp` for public static plugin usage topics; available to ordinary plugins. |

See [developer guide](https://github.com/jdswcz/ArclyraPlugins/blob/master/README.md), [compatibility matrix](https://github.com/jdswcz/ArclyraPlugins/blob/master/sdk-compatibility.md), [Kitty help and host services](https://github.com/jdswcz/ArclyraPlugins/blob/master/host-services-api.md), [story data](https://github.com/jdswcz/ArclyraPlugins/blob/master/story-data-api.md), [local summaries](https://github.com/jdswcz/ArclyraPlugins/blob/master/story-summary-api.md), [AI generation](https://github.com/jdswcz/ArclyraPlugins/blob/master/ai-generation-api.md) and [Smart Builder / prompt review](https://github.com/jdswcz/ArclyraPlugins/blob/master/smart-builder-api.md).

The host supplies `Arclyra.PluginSdk.dll`: **never package a second copy**. Default interface members do not introduce missing APIs into older host SDK binaries. Check the matching host version, availability, cancellation and source freshness. Packaging helpers are included; local packing does not publish to NuGet.
