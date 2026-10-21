# Native continuity memory — SDK 1.30 / host 72

The optional `IPluginContext.StoryMemoryService` exposes host-owned persisted
continuity summaries. `StorySummaryService` remains the independent compatible
utility for caller-supplied prose; it does not grant access to canonical memory.
SDK assembly identity remains 1.2.0.0. Existing plugins/contexts need no new
implementation. New consumers declare minimum host 72 and reference the host SDK
without bundling a copy. A context on the current SDK that lacks the service
returns null; a genuinely older SDK binary lacks this contract, so consumers must
not invoke it there. Minimum-host validation is the supported compatibility gate.

| Operation | Capabilities | Behavior |
| --- | --- | --- |
| `GetAsync` | `story.read` | Read a detached immutable DTO; never infer or download. |
| `RequestFreshAsync` | `story.read`, `ai.summarize` | Reuse current native memory or request the verified local component. |

```csharp
IPluginStoryMemoryService memory = context.StoryMemoryService
    ?? throw new NotSupportedException("Native memory is unavailable in this host context.");
var request = new PluginStoryMemoryRequest(storyId, MaximumWords: 500);
PluginStoryMemoryDto stored = await memory.GetAsync(request, cancellationToken);
// Only an explicit feature/user request should trigger local inference:
PluginStoryMemoryDto current = await memory.RequestFreshAsync(request, progress, cancellationToken);
```

Use `ChapterId` **and** `DraftId` for one exact saved revision (including an
alternative). With neither set, use the whole story, or set `BeforeChapterId` for
an exclusive boundary. Boundary and exact-draft modes cannot be combined. The host
selects one canonical revision per included chapter: Final, otherwise Current,
otherwise latest saved revision. Outlines and later chapters are excluded. Invalid,
ambiguous or divergent revision storage fails without repairing or changing it.

The word ceiling is 100–4,000; complete source is limited to 262,144 UTF-16
characters and summaries to 65,536. Oversized source is rejected without truncation.
Within-request token chunking uses the local summarizer and its selected language/model
profile; continuity is the only generated summary kind in
this API. `Missing`, `Outdated` and `Current` are derived at read time against a
versioned source fingerprint and captured model/language/algorithm/word-budget
profile. Unknown or unavailable model profiles cannot certify currentness. A DTO
may expose outdated text for review; check `Status` before using it as continuity.
No persistent Generating flag or background job is introduced.

Source edits, workspace changes, cancellation, provider unload, requester unload,
manual edits and intervening summary writes invalidate late work. Requests are
serialized at the host: concurrent on-demand requests reuse the first successful
current result. One requester's cancellation does not cancel another requester.
A failed request never changes manuscript prose or replaces a stored summary.

Author-edited summaries are protected. A current author-edited summary may be
read/reused; an outdated one requires explicit author refresh approval in the host
UI. No plugin parameter authorizes that approval; `story.write` does not override
it. DTOs contain no model objects, collection references, workspace secrets or file
paths. Plugin capability checks complement the existing package/signature checks;
capabilities are not an OS sandbox.

Automatic background generation, cross-request incremental reconstruction,
provider-specific migrations and plugin summary mutation are deferred.
