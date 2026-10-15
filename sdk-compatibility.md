# SDK compatibility and migration

This documentation is checked against `jdswcz/ArclyraWritingStudio` commit `d98236ec0f6990731d068763f83af60ac6d47e89` (October 15, 2026): SDK **1.23.0**, host **63**. Host versions below are the host's `System.Version` compatibility milestones, not SDK package versions. Set `minHostVersion` for the behavior your plugin actually requires; for the entire current baseline use `63.0`.

The SDK source under `src/Arclyra.PluginSdk` is synchronized to **1.23.0**, including the package project, contracts, helpers and packaging targets. Build or pack it locally on Windows with .NET 8; see the [build commands](README.md#validation-command). This repository update does not establish NuGet or host release availability. Existing sample projects and signed/unsigned sample binaries remain introductory SDK 1.0.2 examples and have not been updated or rebuilt.

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

SDK 1.6 also adds built-in [chapter editor actions](editor-actions.md). Reusable [prompt-entry versions](prompt-entry-versions.md) and their assignments are part of the current baseline; older default interface implementations throw `NotSupportedException`. Host 58 groups prompt-detail registrations into **AI tools** and supplies an owned-toolbar return action without changing SDK signatures.

The current SDK assembly identity is `1.2.0.0`, while its package version is `1.23.0` and its file version is `1.23.0.0`. Do not infer API support from assembly identity alone. Additive init properties preserve earlier positional constructors; they can be null on older hosts. Existing signatures remain compatible, but new behavior still needs an appropriate minimum host and explicit missing-service checks.

## Migration choices

- **Saved prose:** read `PluginChapterDto.CurrentDraftContent` from a fresh story snapshot. `Content` is the chapter outline; old generated-answer caches and alternative revisions are not authoritative manuscript text.
- **Prompt versions:** prefer reusable version APIs over the obsolete per-chapter customization adapters. Main is virtual and read-only through version APIs. Inspect usage before deletion and preserve the distinction between source IDs and story prompt-detail IDs.
- **Delayed UI actions:** use the supplied cancellation and freshness callbacks, then use the single-use apply callback only after approval. A preflight is not a lease; apply revalidates independently. Release retained callbacks on completion/unload.
- **Sensitive data:** persist prompts, generated answers, feedback and learned data through protected workspace storage. Use the writable data directory for model/runtime downloads and non-sensitive settings. Do not log creative content or secrets.
- **Browser integrations:** declare `ui.aiGenerationToolbar`, `prompt.read` and `aiGeneration.browserAccess` only as needed. Register an owned manual provider, retain **Other providers**, and use approved-frame operations. Preview polling never means generation completed and never saves revisions. Require host 61 for prompt-detail-origin browser handoff, or host 62 for the structured task form, preparation completion and approved provider selection in the captured window. Host 63 is required for automatic observation preparation, loaded-story boundaries, incremental polling and chapter-editor browser selection. Optional callbacks may be null; detect support before invoking them.
- **Distribution:** ordinary developer signing does not grant Trusted Component authority. Ordinary plugins must use their own IDs and omit reserved trusted profiles and first-party placement.

## Verification

Build a plugin against the matching SDK, install it in the minimum supported host, and exercise cancel, stale editors, workspace/provider changes, unload and normal save/import. New browser APIs also require real Windows/CefSharp qualification. Source inspection and synthetic tests do not qualify live website behavior or hardware performance.
