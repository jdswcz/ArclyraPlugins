# Ordinary plugins and Trusted Components

Ordinary plugins use the normal `.arcplugin` format, capability declarations and developer signing described in [Packaging](packaging.md) and [Plugin signing tool](signtool.md). Perchance, Ollama and OpenAI-compatible integrations are ordinary plugins.

Arclyra AI has a separate, host-verified **Trusted Component** identity and dedicated release signature. Its reserved ID is `com.arclyra.plugins.ai`, its profile is `arclyra-ai-v1`, and its detached signature uses `arclyra-trusted-component-v1` with `RSASSA-PSS-SHA256`. The host verifies an allowlisted identity/profile and pinned release key. An unsigned package, ordinary developer signature, user approval or manifest capability cannot grant that identity.

SDK 1.12 / host 50 adds optional `IPluginContext.FirstPartyUiService`, exposing only Arclyra AI panel registration and open/focus. Ordinary plugins receive null. Use ordinary UI registrations for settings, panels, editor actions or owned browser toolbars instead; see [UI extension API](ui-extension-api.md).

Do not request a fabricated trusted capability, adopt Arclyra AI's reserved ID, or reuse its signing path for an ordinary plugin. Trusted status grants only the host's defined placement and workflows, not unrestricted service access.

All plugins remain in-process .NET code with the current user's privileges. Neither capabilities nor signature classification provide an operating-system sandbox. Signature verification establishes package identity and integrity, not correctness of arbitrary plugin code.
