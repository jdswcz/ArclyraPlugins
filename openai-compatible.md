# OpenAI-compatible API plugin

`Arclyra.OpenAiPlugin` is the first-party direct API provider. It uses only the public Plugin SDK and adds each saved profile as **API · profile name (model)** to the normal generation provider selector. Version 1.3.1 requires host 39 / SDK 1.2 and supports persistent story conversations and separate thinking previews. Existing browser and third-party providers are unchanged. No API request is made during plugin initialization.

## Build and install (Windows, .NET 8)

Build an unsigned local test package from the repository root (PowerShell or cmd, one line):

`dotnet build Arclyra.OpenAiPlugin/Arclyra.OpenAiPlugin.csproj -c Debug -p:ArclyraPluginSignToolPath=`

The output is `Arclyra.OpenAiPlugin/bin/Debug/net8.0-windows/ArclyraOpenAi.arcplugin`. Install through **Plugins → Install Plugin**, review permissions, and enable it. Unsigned test packages retain the normal host trust/review requirements; do not bypass them. Release builds use the same developer signing properties and external `plugin-keys` directory as Ollama. See [signing](signtool.md).

The project imports the SDK packaging target. DPAPI is supplied by the Windows Desktop framework; no additional package dependency is required. The archive contains `plugin/plugin.json`, the entry assembly and any copy-local dependencies, but excludes the host-shared `Arclyra.PluginSdk.dll`. No installer or browser settings are modified.

## Configure and generate

1. Open **Settings → OpenAI-compatible API**, choose **New profile**, and enter a unique profile name in **Connection and model**. **Saved profiles** selects an existing profile to edit.
2. Enter the API base URL, including its API prefix. Do not append `/chat/completions` or `/models`.
3. Enable API-key authentication and enter a key when your endpoint requires one. Disable it for a local server configured to accept unauthenticated requests. Disabling authentication omits the Authorization header entirely.
4. Type a text/chat model ID, or choose **Discover models** and select one. Discovery uses the form's endpoint/key without saving them; it is optional and does not prove that every listed model supports chat or that your account may generate with it.
5. Expand **Advanced settings · optional** to adjust generation parameters if needed, then choose **Save profile**.
6. Use the normal chapter/Smart Builder generation workflow and select **API · profile name (model)**. Text appears progressively; successful results enter Arclyra's normal draft workflow. **Stop generation** cancels the HTTP request and stream.

All saved profiles are available together; selecting one in Settings only chooses which profile to edit. Each profile has its own URL, model, parameters and protected key. Add separate profiles for different services or models. **Delete profile** removes only that profile and its provider entry. Switching profiles with unsaved edits offers Save, Discard or Keep editing.

The masked API-key input follows the application theme and sits immediately below the URL. Model discovery/cancel controls are grouped with the model selector, buttons have larger targets, and advanced fields start collapsed.

| Service | Example API base URL | Notes |
| --- | --- | --- |
| OpenAI | `https://api.openai.com/v1` | API key and a Chat Completions-capable model accessible to your account. ChatGPT browser subscriptions are separate from API credentials. |
| OpenRouter | `https://openrouter.ai/api/v1` | API key; model IDs usually contain a provider prefix. |
| LM Studio | `http://localhost:1234/v1` | Start the local server and load a chat model. Enable authentication if the server requires it. |
| vLLM | `http://localhost:8000/v1` | Use the served model name and an appropriate chat template; authentication follows server configuration. |
| Other compatible servers | Their documented API prefix | Requires streaming `POST chat/completions`; `GET models` is optional. |

Only HTTPS remote endpoints and HTTP loopback endpoints are accepted. For another machine, configure HTTPS. Redirects are disabled: enter the final URL directly. URL-embedded credentials, query strings and fragments are rejected. Requests do not use cookies or browser sessions.

Temperature (0–2), top P (0–1), and maximum output tokens are optional. Blank fields are omitted so provider/model defaults apply; some models reject sampling parameters. Choose `max_completion_tokens` for current OpenAI models; select `max_tokens` only for servers/models requiring the older field. Only the selected field is sent. Maximum completion tokens can include reasoning tokens. Timeout covers both connection and stream (10–3600 seconds). This plugin intentionally does not support tools, vision, audio, Responses-only models, or provider-specific request extensions.

## Credential and data handling

Settings belong to the **active data file** and are stored through SDK plugin-owned metadata under `openai-compatible.profiles.v2`. Normal host automatic persistence applies. No story-read or story-write capability is requested; metadata is already scoped by the SDK to this plugin. Generation reads the selected profile anew each time. The previous single-profile format (`openai-compatible.settings.v1`) is read as **Default**, preserving its protected key and generator identity; the next save/delete migrates it to the profile collection. Deleted legacy profiles cannot reappear. The plugin subscribes to story navigation events to refresh the provider list and uses `aiProviders.write` only to remove its own obsolete entries.

The SDK has no general secret store. The plugin uses the smallest isolated Windows solution: DPAPI `CurrentUser`, with plugin identity and the normalized endpoint as entropy. Only the protected Base64 ciphertext is serialized. There is no plaintext fallback, and temporary key byte arrays are cleared. Settings UI never reveals a saved key. Keys and raw server error bodies are not logged or included in errors. Decryption protects data at rest, not against other trusted code running as the same Windows user.

A blank key field retains the saved key only for the same profile and endpoint. Endpoint changes require a new key when authentication is enabled. To delete a key, disable authentication, tick **Remove this profile's saved key**, and save; or use **Reset all API profiles** in Advanced settings. Reset affects only this plugin's metadata. Moving a data file to another Windows account/machine can make its protected key unreadable; enter the key again. Credentials are not synchronized in plaintext.

Generation sends the retained story conversation plus the current chapter prompt as ordered Chat Completions `messages`. The Story Guide is the first user message, followed by previous user/assistant chapter turns. Each story + stable profile ID + normalized endpoint + model has independent history. History is stored by the host with the story, separately from credentials. Failed/canceled requests never append prompts or partial drafts. See [persistence, reset and context trimming](conversations.md). These messages go only to the configured endpoint during generation; model discovery sends no story text. Data handling and API charges depend on that endpoint.

## Errors and validation

- 401/403: check key and model access; 429: check quota/billing or retry later.
- 400/422: check model, context size, token-limit field and optional settings. Clear optional fields if unsupported.
- 404: verify URL prefix/model. A missing model-list endpoint does not prevent manual model entry.
- Interrupted, malformed, refused, or error streams are not returned as successful drafts. A completed `length` response returns text with an explicit token-limit warning. Empty output is an error.
- SSE comments, multi-line data, role-only chunks, usage-only chunks and repeated terminal usage frames are supported. Both a text finish reason and `[DONE]` are required; response size is bounded. No automatic generation retry duplicates a potentially billable request.

Run focused regression tests on Windows:

`dotnet test Arclyra.Tests/Arclyra.Tests.csproj -c Debug -p:ArclyraPluginPackageOnBuild=false --filter FullyQualifiedName~OpenAiPluginTests`

Tests use fake HTTP handlers; they do not spend API credit. Coverage includes request mapping, endpoint validation, optional settings, model discovery, progressive SSE parsing, cancellation, truncated/error streams, sanitized failures, DPAPI round trips, endpoint binding, migration, independent profile credentials, stable identities and generator registration/removal. Full solution/CI validation also builds the plugin through its solution entry and the tests' project reference.

Manual Windows acceptance: install the package, configure a local or authenticated endpoint, discover/select or type a model, generate a chapter with progressive output, cancel a second run, restart to check persistence, re-enter a key after changing endpoint, create a second service profile and verify both are selectable, rename/delete a profile, check the key field in light/dark themes and advanced-field expansion, and verify browser/Ollama providers still work. Live provider access and UI behavior require this smoke test; fake transport tests do not certify every service/model.

## API references checked 2026-09-25

- [OpenAI Chat Completions](https://developers.openai.com/api/reference/resources/chat/subresources/completions/methods/create)
- [OpenRouter streaming and errors](https://openrouter.ai/docs/api_reference/streaming)
- [LM Studio OpenAI compatibility](https://lmstudio.ai/docs/developer/openai-compat)
- [vLLM OpenAI-compatible server](https://docs.vllm.ai/en/latest/serving/online_serving/openai_compatible_server/)

## Thinking preview

The stream reader displays provider-supplied `reasoning_content`, `reasoning`, or
text/summary entries in `reasoning_details` in the shared **Model thinking** panel.
Aliases emitted together are not duplicated; encrypted reasoning is not displayed.
Thinking can appear before the first draft token. It stays separate from chapter
text and persistent conversation history, and is limited to 131,072 characters
without stopping generation. Models/endpoints that do not emit public reasoning
show only the draft. The panel supports collapse, text selection and scrolling
without forcing readers back to the end after they scroll upward.

If the plugin is installed but no API generator profile is available, Smart Builder’s
**Generate with AI plugin → Set up OpenAI-compatible API…** explains where to add
and save a profile (or enable the plugin if its settings page is unavailable).
Use **Start new AI conversation…** to clear the current story’s saved direct-plugin
history without changing drafts or credentials; see [reset behavior](conversations.md#reset-and-invalidation).
