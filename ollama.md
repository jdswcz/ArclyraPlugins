# Ollama direct generation

Ollama plugin 1.2 requires Arclyra host 39 / SDK 1.2. It continues to discover
installed local models through `/api/tags` and register them in Smart Builder.
Generation now uses `POST http://localhost:11434/api/chat` with `model`, ordered
`messages` and `stream: true`, rather than the former single-prompt `/api/generate`.

Each story/model has its own [persistent conversation](conversations.md). The
Story Guide initializes it, and successful chapter prompts/drafts become subsequent
user/assistant turns. History stays in the story data file, including its normal
encryption and autosave behavior. Another model uses a separate scope; guide edits
reset it. Credentials are never stored in conversation state.

The NDJSON parser reads `message.content`, previews draft text progressively and
requires `done: true` before success. `message.thinking` is displayed separately
in a collapsible, selectable Model thinking panel and excluded from drafts/history. Errors, empty
output, tool calls, malformed events and truncated streams do not commit history.
Generation has no time limit by default. In **Settings → Ollama**, save a generation
timeout of `0` (unlimited) or 10–86,400 seconds. It applies to connection and response
streaming on the next request and is stored in plugin metadata in the active data
file. There is no additional HttpClient generation timeout. **Stop generation**
and plugin shutdown cancel active requests. Model discovery and availability checks
retain separate five-second limits. A `length` finish returns a warned draft.

Thinking previews are temporary, limited to 131,072 characters, and never written
to story conversation history. Reaching that preview limit does not stop generation.
Only reasoning actually emitted by the model can be displayed. The panel appears
in Smart Builder, legacy chapter setup and the standalone AI generation window;
it follows new text unless the reader scrolls upward.

See the shared guide for exact key construction, regeneration, trimming limits,
reset rules and tests. Local generation still needs a running Ollama installation
and an installed chat-capable model. Context safety limits are not token estimates
and do not guarantee the model can accept the entire retained history.

Reference: [Ollama chat API](https://docs.ollama.com/api/chat), checked 2026-09-29.
