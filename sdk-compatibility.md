# SDK compatibility and migration

Checked against read-only `jdswcz/ArclyraWritingStudio` dev commit `520e219a77c708a362142c323a42a3c57e3bfe1b` (November 8, 2026): **SDK 1.39.0**, host behavior through **150**. Host milestones and SDK package versions are independent. Choose `minHostVersion` based on the newest SDK feature needed; use `150.0` for the complete checked behavioral baseline.

The standalone SDK C# sources, packaging targets and project metadata match upstream; the package README is adapted for this repository. Template and SamplePlugin source now reference SDK 1.39.0; the minimal template retains host 78, while SamplePlugin requires host 150 for registered Kitty help. The SDK recipes reference the local project and cover the newer API contracts. Historical signed/unsigned archives in `SamplePlugins/packaged/` remain untouched. Neither NuGet publication nor any change to ArclyraWritingStudio is implied.

## Additive milestones

| SDK | Minimum host where documented | Addition |
| --- | --- | --- |
| 1.1 | 38 | Host-owned persistent story conversation transactions. |
| 1.2 | 39 | Separate transient thinking trace. |
| 1.3 | 40 | Writable plugin data directory. |
| 1.4 | 41 | Smart Builder completed-draft actions. |
| 1.5 | 42 | Prompt-detail editor actions; host 48 also presents them for alternative Versions. |
| 1.7 | 43 | `CurrentDraftContent` for saved working manuscript text. |
| 1.8 | 45 | Request-local generation menu choices, including chapter-context overrides. |
| 1.9 | 46 | Cancellable draft actions and guarded revision application. |
| 1.10 | 47 | Plugin-isolated, workspace-scoped encrypted storage. |
| 1.11 | 49 | Transient generation-stage user actions. |
| 1.12 | 50 | Reserved first-party UI placement; unavailable to ordinary plugins. |
| 1.13 | 51 | Ordinary owned-provider toolbar replacement and manual browser workflow. |
| 1.14 | 52 | Approved direct-frame scripts, full-text review and reviewed draft import. |
| 1.15 | 53 | Prompt-detail cancellation, freshness preflight and alternative-version indication. |
| 1.16 | 54 | Host-owned inline diff with explicit review after each proposal edit. |
| 1.17 | 55 | Single-use, source-bound creation of an unused alternative version. |
| 1.18 | 56 | Read-only chapter-editor snapshot freshness check. |
| 1.19 | 57 | Explicitly armed host-owned browser live preview. |
| 1.20 | 59 | Chapter-sized preview and reviewed lossless split import. |
| 1.21 | 60 | Stored-chapter mapping and reviewed, conflict-aware outgoing chapter synchronization. |
| 1.21, unchanged signatures | 61 | Owner-bound prompt-detail browser invocation against the already-visible manual provider. |
| 1.22 | 62 | One task/language/notes/request dialog, preparation-complete callback and captured owned-provider selection. |
| 1.23 | 63 | Automatic owned-browser observation, compact contextual toolbars, reviewed full-story loading, incremental preview envelopes and chapter-editor browser selection. |
| 1.24 | 65 | Raw chapter outline/number, rendered prompt without outline, explicit draft boundaries and guarded browser draft discard. |
| 1.25 | 66 | Template-rendered outline and optional review checkbox with independent edited buffers. |
| 1.26 | 67 | Captured story/chapter/manuscript identities, source validation and observation rebasing. |
| 1.27 | 68 | Captured manuscript including unsaved staging, known browser boundaries and reviewed compact context. Host 69 fixes selected-chapter eligibility without changing signatures. |
| 1.28 | 70 | Capability-gated local summary requests (`ai.summarize`) and cancellable toolbar progress. |
| 1.29 | 71 | Selection-or-document editor replacement with guarded one-step Undo and manual saving. |
| 1.30 | 72 | Optional native continuity memory reads and on-demand freshness. |
| 1.31 | 77 | Explicit host-owned active browser automation leases, acknowledged staging and dynamic Start/Stop toolbar controls. |
| 1.32 | 86 | Optional `IPluginStorySummaryProgress`: typed transient cumulative previews and source-character progress. |
| 1.33 | 92 | Opt-in source-bound direct generation lease and unapproved interrupted-preview exception; host 93 adds explicit keep/restart choices. |
| 1.34 | 102 | Trusted-only read-only prompt-review contracts; host 104 optionally streams validated cumulative suggestion snapshots. |
| 1.35 | 115 | Optional `PluginChapterDto.CurrentDraftId` identifies the saved structured working/displayed revision supplying `CurrentDraftContent`; null for legacy-only text. |
| 1.36 | 134 | Verified-first-party `IPluginArclyraAiOnboarding` provides on-demand Compact model setup, readiness, download approval, progress and cancellation. |
| 1.37 | 136 | `PluginArclyraAiSetupStage.RefusalRepair` and first-launch preparation of runtime/refusal repair, including GPU with CPU fallback when supported. |
| 1.38 | 140 | Verified-first-party local Kitty assistant backend: `IPluginAssistantChatService`, bounded sources/history, streaming status and revocable registration. |
| 1.39 | 150 | Ordinary plugins register up to 24 public, static `PluginKittyHelpTopic` records via optional `IPluginContext.KittyHelp`, without exposing chat or user data. |

SDK 1.6 also adds built-in [chapter editor actions](editor-actions.md). Reusable [prompt-entry versions](prompt-entry-versions.md) and their assignments are part of the current baseline; older default interface implementations throw `NotSupportedException`. Host 58 groups prompt-detail registrations into **AI tools** and supplies an owned-toolbar return action without changing SDK signatures.

The current SDK assembly identity is `1.2.0.0`, while its package version is `1.39.0` and its file version is `1.39.0.0`. Do not infer API support from assembly identity alone. Additive init properties preserve earlier positional constructors; they can be null when a context on the current SDK does not supply an optional service. Default interface members preserve older implementations on the current SDK; they cannot add missing members to an older installed SDK binary. New consumers need the documented minimum host as well as missing-service/callback checks. Do not use a null check as a substitute for minimum-host validation.

## Migration choices

- **Saved prose:** read `PluginChapterDto.CurrentDraftContent` from a fresh story snapshot. On host 115+ use `CurrentDraftId` to identify the structured working/displayed revision; a null ID can still accompany nonempty legacy prose. A stable ID alone does **not** prove unchanged text, and unsaved editor content is excluded. `Content` is the chapter outline; old generated-answer caches and alternative revisions are not authoritative manuscript text.
- **Prompt versions:** prefer reusable version APIs over the obsolete per-chapter customization adapters. Main is virtual and read-only through version APIs. Inspect usage before deletion and preserve the distinction between source IDs and story prompt-detail IDs.
- **Delayed UI actions:** use the supplied cancellation and freshness callbacks, then use the single-use apply callback only after approval. A preflight is not a lease; apply revalidates independently. Release retained callbacks on completion/unload.
- **Sensitive data:** persist prompts, generated answers, feedback and learned data through protected workspace storage. Use the writable data directory for model/runtime downloads and non-sensitive settings. Do not log creative content or secrets.
- **Browser integrations:** declare `ui.aiGenerationToolbar`, `prompt.read` and `aiGeneration.browserAccess` only as needed. Register an owned manual provider, retain **Other providers**, and use approved-frame operations. Preview polling never means generation completed and never saves revisions. Require host 61 for prompt-detail-origin browser handoff, or host 62 for the structured task form, preparation completion and approved provider selection in the captured window. Host 63 is required for automatic observation preparation, loaded-story boundaries, incremental polling and chapter-editor browser selection. Optional callbacks may be null; detect support before invoking them.
- **Summary consumers:** distinguish caller-supplied [summary proposals](story-summary-api.md) (host 70) from persisted [native memory](story-memory-api.md) (host 72). Check memory status, preserve author edits, and generate only on demand. `ai.summarize` grants no general manuscript writer or summary-provider registration.
- **Active browser workflows:** require host 77, acquire `BeginAutomation` only in an explicit owned toolbar action and return immediately. Own the session/task until completion, observe its cancellation and dispose it; acknowledge each complete current-chapter preview before another DOM cycle. Passive observers cannot initiate active generation.
- **Editor tools:** host 71 adds `ReplaceSelectionOrDocument`; zero selection means whole document, while whitespace-only selection never escalates. Keep explicit proposal review and handle every apply status.
- **Typed summary progress:** host 86 and SDK 1.32 add cumulative provisional preview status, not a trustworthy completed summary. A successful final response remains authoritative.
- **Source-bound generation:** host 92 + SDK 1.33 supply a cheap invalidation predicate and token; interrupted previews must remain unapproved and uncommitted. Host 93 requires an explicit keep/restart decision.
- **Local prompt review:** SDK 1.34 / host 102 enables the trusted-only `IPluginPromptReviewService`, with strictly scoped editable targets and host-owned approval. Host 104 can report fully validated cumulative results through a dual-interface progress object. Ordinary plugins cannot register it.
- **Kitty help:** ordinary plugins can supply only static public help topics (SDK 1.39 / host 150). Register on initialization and let the host revoke topics on unload. Never include manuscript text, secrets, or dynamic user context in public topics; no additional manifest capability is required.
- **Trusted Kitty chat and onboarding:** host 134–140 enables setup and local assistant registration only on the existing verified Arclyra AI first-party path; an ordinary plugin signature or manifest cannot opt in. Review [trusted components](trusted-components.md) and [host services](host-services-api.md).
- **Distribution:** ordinary developer signing does not grant Trusted Component authority. Ordinary plugins must use their own IDs and omit reserved trusted profiles and first-party placement.

## Verification

Build a plugin against the matching SDK, install it in the minimum supported host, and exercise cancel, stale editors, workspace/provider changes, unload and normal save/import. New browser APIs also require real Windows/CefSharp qualification. Source inspection and synthetic tests do not qualify live website behavior or hardware performance.
