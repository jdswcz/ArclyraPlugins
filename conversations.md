# Persistent story conversations (SDK 1.1 / host 38)

Direct generators can opt into a host-owned conversation transaction through
`PluginAiConversation.GenerateAsync(request, scope, generate, cancellationToken)`.
Require host 38 / SDK 1.1 for conversation transactions, or host 39 / SDK 1.2
when also reporting separate thinking previews. Existing plugins retain the same
request constructor, delegate and service APIs.

## Identity and ownership

Smart Builder supplies `StoryId`, `ChapterId`, `ChapterNumber`, `StoryGuide` and
`ChapterPrompt` from the selected story/chapter. Names never identify stories.
The generation registry binds `request.Conversation` to that story and the
registered plugin, without granting general story read/write capabilities.
Legacy requests without a story ID or host conversation remain isolated one-shot
requests and never share an implicit global conversation.

Within each story, the key combines a hash of the owning plugin ID and a hash of
the provider scope. Define a stable scope from the configuration identity,
normalized endpoint and exact model ID so unrelated configurations cannot share
history. An unambiguous encoding such as a JSON array avoids concatenation
collisions. Do not include credentials in the scope. Use stable IDs rather than
profile display names so renaming a profile does not reset history.

The host stores an opaque, versioned JSON document in `Story.AiConversationState`
inside the active story data file. Replacing this immutable string notifies the
existing automatic save machinery. History survives closing the UI and, after
the normal automatic save, restarting the application. Existing data-file
encryption and history snapshots apply; there is no separate plaintext sidecar.
As with other story edits, an abrupt process termination before autosave can lose
the latest in-memory change. Deleting a story removes its live conversation state;
existing backups/history snapshots follow the application's usual retention.
Duplicated stories start with a new ID and empty history.

## Message order and transaction boundary

The first request sends the nonempty Story Guide as a user message, followed by
the current chapter prompt. A successful response atomically appends the chapter
prompt and final assistant draft. Subsequent requests send:

```text
user: Story Guide
user: Chapter 1 prompt
assistant: Chapter 1 draft
user: Chapter 2 prompt
assistant: Chapter 2 draft
user: Chapter 3 prompt
```

The guide occurs once in the logical context, although stateless APIs receive the
retained messages again on each request. Identical guide/prompt text remains two
distinct sections. Empty guides are omitted. Requests serialize through a weakly
story-owned asynchronous gate, preventing concurrent lost updates. Partial previews,
failed requests, canceled requests and premature stream termination never commit.
An explicit provider output-limit finish remains a successful draft with a warning;
it is retained as returned.

Regenerating a retained chapter temporarily removes that chapter and all later
turns from the request. Only success replaces that suffix; failure leaves the old
history intact. If the chapter was already trimmed out, a new successful turn is
appended. Editing a saved draft does not silently rewrite AI history.

## Reset and invalidation

In host 40+, choose **Start new AI conversation…** only from Smart Builder’s
**Generate with AI plugin** menu. Confirm **Clear history** to remove all direct-plugin
conversation scopes for the current story across provider profiles and models.
Chapters/drafts, other stories and provider settings are unchanged. The next request starts with the current Story Guide and chapter prompt.

The reset clears `Story.AiConversationState` in the active data file (default:
`%APPDATA%\strapp\stdata.json`) and uses normal autosave/encryption. It also
invalidates pending/active transactions so old responses cannot restore cleared
history, and gives new requests a fresh gate. Existing data-file backups/snapshots
and browser conversation links are not erased.

- A changed SHA-256 of the exact Story Guide starts fresh, including whitespace edits.
- A different profile, endpoint or model selects an independent conversation.
  Switching back resumes its compatible retained history; deleting/recreating a
  profile should give it a new stable ID. Old inactive scopes are evicted by the bounds below.
- Missing, unknown-version, oversized or malformed history is treated as empty;
  a successful generation replaces it. The story itself is unaffected.
- The host rejects commits if the story/chapter was deleted, its guide changed,
  cancellation was requested, or the active data object/file was replaced during
  generation. UI previews/results are not applied to a newly selected chapter.

## Context bounds

These are safety bounds in **UTF-16 characters, not estimated tokens**:

- At most 131,072 message-content characters and 32 complete turns per conversation.
- Before sending, remove the oldest complete user/assistant pairs to make room
  for the full guide and current chapter prompt. Neither mandatory section is cut.
- After success, trim oldest complete pairs again to fit the response. If the
  guide plus current prompt/response alone cannot fit, report an error and retain
  the previous history; the preview is not committed.
- Serialized per-conversation state is capped at 802,816 characters (escaping and
  metadata included). Per story, keep at most 16 scopes and 8 Mi characters of
  serialized state, evicting least-recently-committed scopes first.

These bounds cannot guarantee a particular model's context window. Provider
400/422 or chat errors explain that context length may require a shorter guide/
prompt or a larger-context model. There is no automatic retry or hidden billing.

## Integration validation

When implementing a direct generator, verify message order, persistence after
normal autosave and restart, story/profile/model isolation, guide invalidation,
regeneration, cancellation, failed or incomplete responses, context bounds and
concurrent requests. Ensure thinking previews never enter saved conversation turns.

Generate chapters 1 and 2, close and reopen the story, then generate chapter 3.
Inspect a local test transport's messages to confirm the retained guide and earlier
turns. Repeat with another story and provider scope, reset history during a pending
request, and change the guide to verify stale results cannot restore old history.
