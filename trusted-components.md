# Ordinary plugins and Trusted Components

Ordinary plugins use the `.arcplugin` format, capability declarations and developer
signing described in [Packaging](packaging.md) and [Plugin signing tool](signtool.md).

The host reserves certain identities and signing profiles for first-party Trusted
Components. A `trustedComponent` manifest declaration does not grant authority:
the host requires an allowlisted identity/profile, a pinned release key and dedicated
integrity verification. An ordinary developer signature or user approval cannot
satisfy these checks. Ordinary plugins must omit this field and use their own IDs.

SDK 1.12 / host 50 adds optional `IPluginContext.FirstPartyUiService` for reserved
first-party placement. Ordinary plugins receive null and should use the standard
UI registrations for settings, panels, editor actions and owned browser toolbars;
see [UI extension API](ui-extension-api.md). No capability declaration grants access
to this service, and trusted status does not grant unrestricted host-service access.

All plugins remain in-process .NET code with the current user's privileges. Neither
capabilities nor signature classification provide an operating-system sandbox.
Signature verification establishes package identity and integrity, not correctness
of arbitrary plugin code.

## Read-only prompt reviewer registration (SDK 1.34 / host 102)

Only the verified allowlisted first-party component receives the optional `IPluginFirstPartyUiService.RegisterPromptReviewer(IPluginPromptReviewService)` registration. It permits local, read-only prompt-review recommendations, never direct modification or implicit approval. `IsReady` allows the host to offer setup. Host 104 optionally accepts validated cumulative result progress. Registration and outstanding work are revoked on component unload; older hosts reject the optional registration method.

Ordinary plugins cannot unlock this service through capability declarations, an ordinary developer signature or self-declared trusted status. See [local prompt review contracts](smart-builder-api.md#local-prompt-review-sdk-134--host-102).

## First-launch AI onboarding (SDK 1.36/1.37 / host 134/136)

The host accepts `IPluginArclyraAiOnboarding` **only** from an already loaded, signature-verified Arclyra AI Trusted Component; ordinary plugins cannot register this interface or acquire trust by naming it. `IsReadyForOnboarding` reports whether the local model, runtime and refusal repair are ready. `InstallCompactForOnboardingAsync(confirm, progress, cancellationToken)` reuses ready assets or sets up Compact with an explicit download/disk approval callback. Approval is required before downloads. Cancellation retains resumable partial downloads; callers must handle cancellation and setup errors.

`PluginArclyraAiDownloadPlan` reports model name, remaining download bytes and disk requirements. `PluginArclyraAiSetupProgress` reports message, optional completion fraction, stage, ready state and optional byte counters. SDK 1.37 / host 136 includes `PluginArclyraAiSetupStage.RefusalRepair`. The first-run setup prepares CUDA plus CPU fallback when applicable and the managed CPU refusal-repair helper before marking setup ready. Normal Arclyra AI model selection and benchmarking remain separate.

## Kitty assistant registration (SDK 1.38 / host 140)

The same first-party verification gate controls `IPluginFirstPartyUiService.SupportsAssistantChat` and `RegisterAssistantChat(IPluginAssistantChatService)`. Older hosts return `false` or throw `NotSupportedException`. The single local chat backend receives only bounded host-approved evidence and is canceled/revoked when the verified component unloads. The API is read-only and grants no plugin-wide story, file or UI-write permissions.

Public plugin help registration via `IPluginContext.KittyHelp` (SDK 1.39 / host 150) is a **separate ordinary-plugin API**. It grants no trusted component identity or access to chat contents. See [host services](host-services-api.md#plugin-help-for-kitty-sdk-139--host-150).
