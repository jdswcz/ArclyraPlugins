# Local summary requests — SDK 1.28 / host 70

`IPluginContext.StorySummaryService` is an optional `IPluginStorySummaryService` for summarizing caller-supplied prose. Declare `ai.summarize`. This service is separate from [native continuity memory](story-memory-api.md): it returns a proposal and does not persist a memory record, edit a manuscript or change a browser document.

```csharp
IPluginStorySummaryService service = context.StorySummaryService
    ?? throw new NotSupportedException("Host 70 or newer is required.");
PluginStorySummaryProfile profile = await service.GetProfileAsync(cancellationToken);
var request = new PluginStorySummaryRequest(
    HistoricalText: completeHistoricalProse,
    PreviousSummary: null,
    MaximumWords: 500,
    ExpectedIdentity: profile.Identity);
PluginStorySummaryResult proposal = await service.SummarizeAsync(request, progress, cancellationToken);
// Recheck captured source freshness, then review before applying anywhere.
```

The profile identifies the selected local model, language and algorithm. `ExpectedIdentity` is required and at most 512 UTF-16 characters; the result identity must match. For incremental requests, send only newly historical prose with `PreviousSummary` from the same profile. Changes to earlier source, model, language, algorithm or word budget require a full reconstruction; never reuse an incompatible summary. Persist derived user content only through protected workspace storage, with enough provenance to detect these changes.

| Input/output | Limit |
| --- | --- |
| `HistoricalText` | Nonempty; at most 262,144 UTF-16 characters. |
| `PreviousSummary` | Optional; at most 65,536 UTF-16 characters. |
| `MaximumWords` | 100–4,000. |
| Result text | Nonempty plain text; at most 65,536 UTF-16 characters. |

Handle invalid requests, missing local provider/model and generation failures without applying partial output. Cancellation and requester/provider unload revoke work and reject late results. Requests do not download components or start chapter-generation/research workflows. Provider registration is restricted to a host-verified component; an ordinary plugin signature or capability cannot grant it.

SDK 1.28 also exposes optional `PluginAiGenerationPasteActionContext.Progress` for the current cancellable toolbar invocation. Report status there and pass `action.CancellationToken` to summary work. **Cancel action** cancels this invocation; a separate browser preview has its own lifetime. Do not retain progress callbacks or invocation tokens for long-lived observation.

Default interface members preserve implementations running against the current SDK. They do not add new members to an older installed SDK binary. Set `minHostVersion` to `70.0` for this API and check for a missing service. See [compilable recipes](SamplePlugins/Arclyra.SdkExamples/README.md).
