# Perchance integration by Arclyra

Ordinary optional plugin `com.arclyra.plugins.perchance`, package
`PerchanceIntegration.arcplugin`, plugin 0.6.0, SDK 1.22 / minimum host 62. Its display name is exactly
**Perchance integration by Arclyra**. It is unofficial and text only.

## Implemented Story Writer workflow

The ordinary plugin adds a runtime browser provider and settings explanation. In
Smart Builder's chapter generation actions, select **Perchance**
to open the visible browser for the saved chapter. It opens the site without generating.
The AI Generation Window replaces the entire provider/paste toolbar with **Insert Prompt**, **Start streaming**, **Sync chapter edits**, **Import Result**, **Import Detail**, **Import Ideas**, **Import Analysis**, **Cancel Text Task** and the host-owned **Other providers**.
The toolbar shows Story Writer / Text Assistant / Creative Builder / Story Analyst and the current story/chapter. Selected field names/scopes appear in review. Import
Result remains disabled without a valid saved chapter target; **Import Detail** can apply only an explicitly started detail task and never falls back to chapter import. Tasks begin with the ordinary visible browser already open. Other providers restores the
standard controls; switching provider, navigating away, unload or callback failure
also restores them. Ordinary plugins cannot acquire Trusted Component placement.

1. **Insert Prompt** opens an editable full-text review, character count and selected
   private fields. Only explicit confirmation writes `textarea#storyOverviewEl`.
   Existing overview/story requires a second reuse confirmation. The story is never
   cleared. A changed overview/story/context causes rejection rather than replacement.
2. Generate and Continue are author-controlled website actions. Perchance can write
   incrementally; Arclyra does not infer that unchanged text is a completed chapter.
3. **Import Result** reads the complete public `#storySoFarEl.value` document, excludes
   marked `SUMMARY^` site-summary paragraphs, and preserves remaining Unicode,
   whitespace and line endings. It never reads visible CodeMirror lines, page text,
   overview or continuation inputs. Visible Stop state blocks import while generating.
4. A host-owned full editable result preview identifies the captured story/chapter.
   **Save new revision** re-reads the source, checks the original chapter/workspace
   snapshot and appends through `ChapterDraftCompatibilityService.AddGeneratedDraft`.
   Existing/final revisions and the unsaved local preview remain intact. Metadata
   records `Browser import`, provider, UTC creation/modification time and parent draft.
   Duplicate, empty, oversized, stale and missing-target imports save nothing.

Each text field is bounded to 262,144 UTF-16 characters; all script arguments use
JSON serialization. The plugin keeps no persistent manuscript cache or private-text
logs. Generation remains a manual website action. After a chapter prompt is inserted,
the host polls cumulative prose every 1.5 seconds, including partial output while
generating. **Start streaming** attaches an existing story without reinserting.
Stored chapter working text and recent known revisions establish chapter boundaries;
only the selected chapter section is published to the unsaved Smart Builder draft.
New text after the last matched chapter goes to the selected new chapter. Unknown,
repeated or reordered matches reject instead of guessing from a word count.
The pre-generation hidden editor waits instead of canceling capture.

**Import Result** reviews and saves only the mapped selected chapter as a new revision.
An unknown boundary refuses import; it never silently saves the full cumulative story
as one chapter. Text/detail/creative/analysis tasks keep their existing separate flows.
The toolbar has eight actions, within the ordinary host limit. The old length-split
action is removed. The earlier SDK 1.20 length-review contract remains compatible for
other plugins, but Perchance now uses chapter boundaries exclusively.

**Sync chapter edits** is an explicit opt-in. After reading the current page, the host
shows the complete proposed website story and inline comparison. Confirming enables
outgoing changes in mapped chapters and future automatic propagation while Perchance
is idle. Working chapter edits and user edits in the active Smart Builder draft are
tracked independently from website output. Our own preview writes are recognized as
echoes and never sent back as author changes. Saving a revision and clearing its staging preview neither deletes website prose nor repopulates the cleared staging field. Changes in different mapped chapters
merge; changes on both sides of the same chapter pause until a new Sync chapter edits
review. During generation, outgoing changes wait without overwriting the author draft.
Cancel sends nothing. No synchronization poll saves an Arclyra revision or changes final
selection, creates chapters, clicks Generate/Continue, or sends chapters absent from
this browser story.

Every outgoing write checks the exact source story, overview, frame/document and host
chapter snapshot before and after execution. It uses the complete public editor value,
not viewport DOM. The website must acknowledge the exact text, allowing only textarea
newline canonicalization. Ambiguous/cross-chapter website rewrites, topology changes,
provider unload, navigation, context switch, close and Stop preview stop the session.
Mapped edits spanning Perchance SUMMARY memory blocks pause rather than delete or move
those blocks. These cases require reconciliation on the website. Session mappings are
transient, bounded, and discarded on stop; inactive alternative revisions are matching
evidence, never independently mirrored into the story.

Insertion binds subsequent operations to the actual overview after browser CRLF/CR-to-LF
canonicalization; all other prose/whitespace is retained. The native Windows/CefSharp
human-generated round trip and production-key signing remain release gates. Build host
62 with plugin 0.6.0. A dedicated one-chapter generator is the preferred next architecture;
see [design and deployment gate](perchance/chapter-generator-design.md). No custom page
has been published or enabled by this change.

## Selected-text Text Assistant (0.3.0)

1. Open the ordinary AI Generation Window from the chapter workflow and select
   **Perchance integration by Arclyra** first. Keep that visible browser open. This
   SDK has no general plugin method for opening/focusing a generation window; the
   plugin does not reach into host controls or create another browser.
2. In the built-in chapter editor, select a non-empty passage and choose **Rewrite**,
   **Expand**, **Shorten**, **Adjust Tone**, **Improve Dialogue** or **Grammar / Style**
   from the plugin menu. Choose an output-language instruction and edit the full
   request/tone/notes. Only the selection and those instructions are prepared;
   the rest of the chapter and Story Guide are not added. Nothing is sent yet.
3. Confirm **Use in Perchance toolbar**. That one task takes priority over the normal
   chapter prompt until it finishes or is canceled. Use **Insert Prompt** in the Perchance toolbar; insertion still requires explicit review and
   reuse confirmation, and never clears the existing website story. Choose Generate
   or Continue yourself on the visible website.
4. **Import Result** now targets the captured editor selection, not a new chapter
   draft. It requires this task's own inserted overview and a story different from
   the insertion baseline. First review the complete read-only original, then the
   complete editable proposal with a read-only inline diff. Choose **Review changes**
   before approval and again after any edit, even a changed-and-reverted edit.
   The author may cancel either review and retry without applying.
5. **Replace captured selection** re-reads the source and invokes only the existing
   host-bound `ApplyAsync` once. Story/chapter/document/revision/content/selection/
   caret changes, closed editors and unavailable plugin registrations reject the
   write. The host retains dirty tracking, undo/redo and normal save behavior; the
   plugin does not automatically save or overwrite an entire chapter.

Story Writer is a story-oriented page, **not a verified dedicated rewrite tool**.
Its captured result can contain previous prose or unrelated continuation. Review
the full result and remove those parts before accepting the replacement. The plugin
does not infer answer boundaries, silently drop prose or certify model compliance.
The exact live rewriting behavior still needs a human Windows/CefSharp qualification.

Only one selected-text interaction per plugin is allowed. **Cancel Text Task** in
the toolbar, closing the editor or unloading the plugin releases it without writes.
An in-flight canceled browser import closes it too. Outside an import, switching
providers does not submit or apply the task; return to the visible owned provider
or cancel explicitly. Each browser operation still uses the host's current scoped
toolbar invocation and cancellation/navigation checks. No background read/polling
is introduced. Closing the task restores normal Story Writer prompt/import routing.

`PerchanceTextAssistantTests` runs 44 deterministic cases with local text/source
fixtures and the actual host selection guard, no browser or network. It covers
Unicode/quotes/large text, selected-only privacy boundaries, language instructions,
cancel/retry, exclusive lifetime, source changes, stale editor states and concurrent
import. Existing 50 DOM and 32 qualification-contract fixtures remain in CI.
The selected-text feature was introduced with only `ui.editorActions` on SDK 1.14 /
host 52. The current package requires host 56 for read-only snapshot preflight,
retaining host 55 source-bound creative creation,
retaining host 54 inline review and host 53 detail lifetime. Standard signing is unchanged; live qualification remains open.

### Prompt-detail safety prerequisite (SDK 1.15 / host 53)

The host now gives Details and alternative Versions a single-use bounded invocation
lifetime, including cancellation/preflight and source/editor/workspace guards; see
[long-running entry interactions](ui-extension-api.md#long-running-entry-interactions-sdk-115--host-53).
`PluginPromptDetailActionGuardTests` adds 25 deterministic cases and focused Windows
CI also runs the existing 10 actual WPF version-editor cases. Perchance 0.3.1 now uses
this prerequisite; no additional host/SDK changes were needed for the detail workflow.

## Prompt-detail and alternative-version assistant (0.3.1)

1. Keep the ordinary visible Perchance browser open and selected. In **Details** or
   an alternative **Versions** editor, invoke **Perchance integration by Arclyra**.
   Writing guidance, World detail, Character, Item and Custom detail are supported.
   Main remains read-only in Versions; edit its primary fields in Details instead.
2. Choose `rewrite`, `expand`, `shorten`, `tone`, `dialogue` or `style`, then an output
   language. Review the complete editable request before **Insert Prompt**.
   Details includes only its current unsaved description/info and applicable name,
   age and category reference fields. An alternative includes only its full unsaved
   version text, not Main metadata. No other details, chapters, guide, templates or
   version collection are added. Confirming **Insert Prompt** immediately inserts the
   approved request into the visible browser; no additional toolbar click is needed.
   Reusing an existing overview/story still requires confirmation, and story prose is preserved.
3. Choose Generate/Continue manually on the visible website. Selected-text and detail tasks share one exclusive transient slot;
   finish or **Cancel Text Task** before starting another. Insert/Import bind
   cancellation to both the current toolbar and the captured editor lifetime.
4. **Import Detail** reviews the complete read-only original followed by a full
   editable proposal. It requires this task's inserted overview and changed website
   story, then rechecks both source and host editor before a single approved
   `ApplyDescription`. Cancel keeps the task available for retry. Without a pending
   detail task, the button reads nothing and never imports into a chapter/selection.
   **Import Result**, when available, also routes a pending detail task explicitly.
5. Approval changes only the captured unsaved description/info or alternative editor.
   Save / Save changes is a separate author action. Names/age/category, other versions
   and Main stay unchanged. Navigation, manual source edits, workspace changes,
   removal/revert, unavailable registration or unload reject stale application.
   New suggestions do not become canon without approval. Sequential original/proposal
   comparison now includes a read-only inline diff of the current editable proposal.

The story-oriented page is not a dedicated detail generator: its full result may
contain prior prose. Remove unrelated text in the proposal yourself; no automatic
answer segmentation or completion claim is made. The plugin adds only ordinary
`ui.entryManagement` and `prompt.write`; it requires the host 53 guarded callback
and fails closed on older/missing lifetime contexts. Signing and browser adapter
are unchanged. There is no persistence, autosave or general prompt-entry writer.

`PerchancePromptDetailTests` adds 51 deterministic cases for all five types and both
editor surfaces, exact source/privacy/Unicode/size boundaries, shared exclusivity,
cancel/retry, double-import and stale browser/editor rejection. Two additional STA
`PerchancePromptDetailVersionEditorTests` exercise the complete import through the
actual WPF alternative editor, preserving saved/Main/other text and rejecting a
review-time switch to Main. These fixtures send no manuscript and run no generation.
Manual Windows/CefSharp and production-key qualification remain open.

## Current inline diff (0.3.2 / SDK 1.16 / host 54)

Selection and detail/version proposals opt into the existing host-owned review via
the additive `PluginTextReviewRequest.OriginalText` property. Null retains the old
ordinary dialog; no other plugin/provider is forced into comparison. The original
and proposal are independently bounded to 262,144 UTF-16 characters. The original
is never edited, and no diff markup is used as replacement text.

The **Proposal** tab remains editable. **Review changes** computes a complete
read-only **Changes** tab; added text is bold/underlined, removed text struck through,
with contrasting backgrounds and a legend. Changed spaces/tabs/CR/LF use visible
display-only markers. Confirmation is unavailable until the current exact proposal
has been reviewed; any edit clears the diff and its approval state, even after revert.
Cancel/disposal and editor/navigation lifetime protections still apply independently.
Unchanged/blank/oversized proposals remain rejected by the existing import guards.

Matching preserves complete Unicode graphemes, quotes, whitespace and line endings.
The transient local algorithm limits its matching matrix to 250,000 cells and display
to 512 segments. When either limit is reached it shows the full changed blocks and
an explicit coarse-matching notice, without truncation or omitted text. It does not
claim semantic/canon validation or optimal matching. There are no timers, model
requests, new capabilities, dependencies, persisted manuscript or private-text logs.

`PluginTextComparisonTests` adds 33 deterministic cases (including 100 fixed-seed
Unicode fixture comparisons): lossless reconstruction, grapheme boundaries, maximum
input, detailed/coarse matching, visual budget, cancellation, privacy-safe errors,
per-field bounds and legacy constructor/deconstruction. Four actual STA WPF
`PluginTextReviewComparisonTests` cover unchanged legacy review, styled read-only
diff, edit/revert invalidation, full-block fallback and pre-window rejection.
Existing selection/detail import cases assert the exact original is supplied.

## Scoped Creative Builder candidates (0.4.0–0.4.1)

1. Keep the ordinary visible Perchance browser open and selected. In editable
   **Details** or an alternative **Versions** editor, invoke
   **Perchance integration by Arclyra · Creative Builder**. Choose `character`,
   `world`, `item`, `scene`, `outline` or `blurb` for Character Expansion, Worldbuilding,
   Item Detail, Scene/Plot Ideas, Chapter Outline Alternatives or Synopsis/Blurb.
   Defaults follow the actual source type; the author can choose another request.
2. Choose the output language and edit the complete request/notes. Only the captured
   unsaved description/info and applicable name/age/category are included. Alternative
   versions contribute only their own full unsaved text, never Main metadata. Empty
   source is allowed for editable new ideas, with explicit missing-context instructions.
   The complete story dataset, other details, guide, chapters/templates/versions are
   not added. A blurb based on one excerpt is not claimed to summarize the whole book.
3. Confirm **Insert Prompt** to insert the approved request directly into the visible
   browser. Reusing an existing overview/story still requires confirmation. Operate
   Generate/Continue yourself on the website. Revision and creative
   tasks share one exclusive transient slot; **Cancel Text Task** discards either.
4. **Import Ideas** requires this task's own inserted overview and changed story.
   Review the full read-only source, then the editable full candidate with current
   inline diff. Remove old website prose/unrelated continuation manually. The plugin
   re-reads both raw source and cleaned story and checks the captured editor lifetime.
5. Choose **Manual copy only**, or use the same copy flow when no creation callback
   is available (for example a new unsaved source). A final read-only **Reviewed candidate / manual copy** surface shows the exact
   edited candidate and source/destination limitations. Select/copy yourself using
   normal text controls. **Finish without saving** closes the task: it saves, applies,
   assigns and automatically copies nothing. Cancel any review to retry. Use ordinary
   Arclyra New / Duplicate / New from Main and explicit save controls separately if
   you decide to accept a candidate. Suggestions are never automatically canon.
6. On a supported persisted source, alternatively choose **Create alternative version**,
   enter a unique 1–120-character name (not Main), then review the complete exact
   edited candidate/diff and captured source with **Create unused alternative**.
   After that final explicit approval the plugin re-reads both raw/clean website
   text and rechecks the editor lifetime. The source-bound host callback appends
   one unused alternative with a new ID using normal host persistence/autosave.
   Existing unsaved editor text, its selection/read-only state, Main, old versions
   and default/chapter assignments remain unchanged; the new alternative is not
   selected. The version picker gains its new option without resetting the editor.
   Assignment is a separate normal author action. No new source entry is created.
   Cancel destination/name/final approval permits retry; a submitted rejected callback
   is consumed and requires restarting the task. Duplicate names never overwrite.

**Import Ideas** never falls back to a chapter/selection/detail import. **Import Detail**
rejects creative tasks. **Import Result**, when available, routes an active creative
task to this same reviewed destination flow rather than a chapter writer. Empty/unchanged/
oversized candidates, source changes, navigation, missing host lifetime, duplicate
imports and cancellation cannot reach a data writer. Existing fields, Main, saved
and unsaved versions, outlines and assignments remain intact, even for duplicate names.
No clipboard API, general story/prompt writer, new capability or persistent manuscript
cache is introduced; all nine capabilities/signing targets stay unchanged. SDK 1.17 /
host 55 adds only the optional source-bound creation callback; its constructor,
deconstruction and assembly identity remain compatible. It shares the existing
single-use description invocation and rejects stale/duplicate attempts. The plugin
never synthesizes opaque ids or expands to whole-dataset `story.read` discovery.
Main remains read-only in Versions; missing creation support always retains manual copy.

`PerchanceCreativeBuilderTests` adds 63 deterministic cases for all six presets,
actual source types/defaults, registration ownership/capabilities, selected-fields
privacy/Unicode/bounds, copy-only delivery, edit/cancel/retry, duplicate-name preservation,
exclusive lifetime, stale browser/source rejection and double-import/cancellation.
Two additional actual WPF version-editor cases prove copy delivery leaves the
read-only alternative/Main and saved versions unchanged and rejects navigation to
Main during review. Another 50 deterministic creation/host-guard cases cover all five
types/two surfaces, optional legacy callback, explicit copy/create/name/final approval,
duplicate/invalid names, single-use sharing, precise Unicode/bounds, final website
revalidation, late target cancellation, double import and private-error redaction.
Four additional actual WPF cases exercise successful read-only/unsaved alternative
creation, duplicate rejection and final-review navigation to Main; old text and
default/chapter assignments are preserved, and the picker refresh cannot reset edits.
These local fixtures perform no generation or network requests.
Live Windows/CefSharp and production-key installation remain open gates.

## Story Analyst suggestions (0.5.0)

1. Keep the ordinary visible Perchance Story Writer browser open and selected.
   Select a non-empty passage in the built-in chapter editor and choose
   **Perchance integration by Arclyra · Story Analyst**: Literary Critique, Pacing,
   Dialogue Quality or Suggested Continuity Checks. This is a separate ordinary
   selection-only menu, not an automatic story-wide audit or replacement action.
2. Choose an editable output-language instruction and edit the complete questions.
   Only the selected passage, language and questions are prepared; the rest of the
   chapter, book, guide, details and versions are excluded. Explicit missing-context,
   observed-evidence/uncertain-interpretation and author-acceptance limitations stay
   visible. No text has been sent yet. Confirm Use in Perchance toolbar.
3. Separately use Insert Prompt, then Generate/Continue manually on the
   unmodified visible website. This uses the already tested Story Writer DOM adapter,
   not a new analyst page or model endpoint. The story-oriented site is not a
   dedicated critic; its live usefulness still requires human Windows/CefSharp testing.
4. **Import Analysis** reads only an explicitly pending analyst task. Review the
   full selected source, then full editable website result. Remove old prose or
   unrelated continuation yourself; no automatic answer segmentation is claimed.
   Analysis is suggestions, not replacement prose, so its review intentionally does
   not show a misleading manuscript-replacement diff. Check quoted evidence and
   limitations yourself; the plugin cannot certify literary quality or continuity.
5. A final read-only Reviewed analysis / manual copy surface shows the exact edited
   notes and source identity. Select/copy manually if desired; Finish without applying
   writes/saves/assigns/automatically copies nothing. After the last review, raw/clean
   website text, inserted overview/baseline and captured source are revalidated.
   Cancel any review permits explicit retry; a changed source closes the task.

SDK 1.18 / host 56 adds a minimal optional chapter-action `IsSnapshotCurrent`
preflight. The previous token covers editor close but not revision changes; calling
a writer merely to check source currency would be incorrect. The new read-only
guard validates workspace/registration/document/revision/content/selection/caret
without consuming ApplyAsync and releases its capture on invocation completion.
Analysis fails closed if preflight/cancellable lifetime is absent. Existing chapter
writers remain independently guarded and unchanged. No new capability/signing
profile, timer, storage, direct model call or browser generation is introduced.

Analysis shares the exclusive transient text/detail/creative slot. Import Result,
when available, routes a pending analyst task to this same copy-only flow, never to
a chapter writer. Import Detail / Import Ideas reject analyst tasks; Import Analysis
without a task reads nothing and never falls back. Closing/unloading/canceling releases
the interaction. The new ordinary toolbar action keeps within the host's eight-action
budget and preserves Other providers and all restoration behavior.

`PluginChapterEditorActionReviewGuardTests` adds 15 pure preflight/legacy/lifetime
cases; `PerchanceStoryAnalystTests` adds 47 deterministic prompt/privacy/language/
bounds/ownership/source/review/cancel/double-import cases. Four additional actual STA
WPF text-editor cases keep unsaved text untouched and reject final-review text,
selection and invocation-completion changes. One additional Windows registration
case exercises actual ordinary UI/editor registries, seven scoped toolbar actions
and empty Import Analysis with every browser/data/chapter fallback forbidden.
Fixtures generate nothing and send no
manuscript. Live usefulness, signed installation and production keys remain open.

## Character Chat reconnaissance (2026-10-14; integration still open)

Opened the ordinary interactive browser at
<https://perchance.org/ai-character-chat>. The visible direct generator iframe was
`iframe#outputIframeEl`, with observed source
`https://04f1a4119d30b5672a3e08fddd12fce4.perchance.org/ai-character-chat?__generatorLastEditTime=1791003894561`.
This hash is an observation, not an adapter host pin. No chat text/profile was typed,
no Send/Generate/Continue or image/upload/export/import control was activated,
and no data was submitted. Opened Edit character, recorded control metadata only,
then Cancel; this is reconnaissance, not CefSharp/manual generation qualification.

The form has name, description/personality/instruction, reminder, general writing
instructions and initial-message fields, but its visible profile inputs/textareas
have no stable id/name attributes. The separate composer is `textarea#messageInput`
and Send is `button#sendButton`; input commands can themselves request AI replies.
Never treat profile insertion as a Send or execute chat commands. The page exposes
local-data export/import and deletion controls; do not use them as a shortcut to
discovering or scraping all conversations. Text-only integration must leave avatar,
image generation, advertisements and other fields alone. The visible field help
links to <https://perchance.org/ai-character-chat-docs#instruction-and-reminder>.

No Character Chat provider, toolbar or conversation scraper is enabled yet. The
internal profile adapter foundation below is implemented, but not wired to a user
action. Author-selected conversation/excerpt import and the bounded source/review
workflow remain independent M5 work. The existing Story Writer adapter's exact-path
allowlist has not been broadened to this page or its composer.

### Internal text-only profile adapter foundation (2026-10-14)

A second visible-browser inspection found public `section.structuredInputSection`
metadata inside one `.promptModalInnerContainer > ... .sectionsContainer`:
`data-spec-key="name"` and `data-spec-key="roleInstruction"`, with recognizable
Character name / Character description labels. The form has separate `button.submit`
Save character and `button.cancel` controls. Sidebar `.thread.selected[data-thread-id]`
and `.characterName [title="Character ID"]` identify the selected chat context.
The form was opened/canceled without typing, saving, sending or generation. These
are mutable observed DOM contracts, not a claim of stable site ids or CefSharp support.

`CharacterChatProfileAdapter` embeds a separate versioned `character-profile-v1`
script. It permits only inspect / explicitly approved insert / release at exact
HTTPS `/ai-character-chat` on Perchance or its subdomains. It requires one visible
modal, unique validated sections/labels/text controls, Save/Cancel structure and
public selected-chat ids. Missing/ambiguous/hidden/disabled/read-only/fieldset/ARIA/
inert controls fail closed; the adapter never unlocks or opens anything.
Inspection returns only the complete current name/profile and public chat ids.
The name is bounded to 200 and profile to 262,144 UTF-16 characters, without trimming
or truncating approved content. JSON serialization keeps user text inert as data.

The host-side helper creates a fresh opaque token. A document-local WeakMap binds
it to the actual modal/field/sidebar nodes, with no profile strings stored there.
Reinspection revokes old approval; profile input/change events invalidate even a
changed-and-reverted field. Insertion compares all captured values/ids/node identities
and consumes approval before any mutation/reentrant event. A reopened identical form,
replaced nodes, changed current chat or repeated insertion rejects it. Release removes
the matching token/listeners; weak keys allow removed forms to be collected. Future UI
must release the snapshot on cancellation/completion and bind every operation to the
SDK toolbar/source lifetime; this foundation itself retains no host/browser handle.

**Identity limitation:** public sidebar ids identify the selected chat, not the
private save target of the modal. The adapter deliberately does not inspect website
JavaScript objects/storage or assert that matching names prove hidden character
identity. The future author review must identify the complete already-open form and
both existing/approved fields, explain this distinction, and require explicit approval.
Save character remains a separate site action on the author-chosen form. A collapsed
sidebar/current identity, new-chat setup without a selected thread, or DOM drift is
unsupported and rejected; no guessed target or broad fallback is attempted.

Insertion updates only native text `value` properties of name and roleInstruction,
then ordinary input/change events. It never clicks Save/Send, submits, types commands,
reads conversation text, opens an image/upload/export control, writes clipboard/storage,
or changes avatar/user/reminder/style/initial-message/custom-code/advertising fields.
If site handlers refuse exact text, approval remains consumed and recovery asks the
author to inspect both fields; no retry, rollback over newer edits or saved-data claim.

`Test-PerchanceCharacterProfileAdapter.cjs` adds 103 deterministic modeled DOM cases;
`PerchanceCharacterProfileTests` adds 30 C# serialization/approval/response/bounds/
privacy/release cases. Windows CI runs both and the regression suite. No website
insertion/model request/private manuscript was used for tests. Host/SDK, package
version 0.5.0, all nine capabilities, existing scoped toolbar and ordinary external-key
signing targets are unchanged. The resource is internal and unused by runtime UI;
profile source/review wiring, selected-excerpt import and live manual qualification
must be completed before exposing Chat controls. M5 remains open.

## Browser evidence and adapter design (2026-10-13)


Observed with an interactive Chromium tab at
<https://perchance.org/ai-story-generator>. No Generate/Continue button was clicked
and no private manuscript was supplied during reconnaissance.

| Surface | Observed identity | Consequence |
| --- | --- | --- |
| Outer page | `https://perchance.org/ai-story-generator` | Exact HTTPS origin/path allowlist; query/fragment do not select a different page. |
| Generator frame | `iframe#outputIframeEl` | Frame URL was `https://d4da62dba2215e294c0d13d0fd16ebe5.perchance.org/ai-story-generator?__generatorLastEditTime=1789650407585`; it is cross-origin. The hash is an observation, not a permanent pinned hostname. |
| Prompt | `textarea#storyOverviewEl` | Overview placeholder describes keywords, characters, plot, writing style and lore. Insert only after explicit author review; never clear story text. |
| Initial generation | `button#storyBeginBtn`, label “begin story” | Author operates this on the visible site. |
| Continuation | `button#generateBtn`, label “continue story”; `#continueTextBtn` | No plugin clicks, hotkeys or timers. |
| Result editor | `#storySoFarEl`, a DIV containing `.cm-content[contenteditable]` | `text-editor-plugin-v1` exposes the complete textarea-compatible `.value`; rendered CodeMirror lines are virtualized. |
| Continuation instructions | `#whatHappensNextEl`, `#storyArcInstructionEl` | These are input and must never be included in imported prose. |
| Site export helper | `#copyStoryTextWithoutSummariesBtn`, label “copy story (no summaries)” | Site has special summaries; do not infer complete document text from visible DOM lines. |

Existing main-frame scripts cannot cross the generator iframe's origin. The new
host-mediated path validates the active owned manual provider, exact top-level
HTTPS origin/path, exactly one visible iframe id and exactly one direct CefSharp
frame matching its current source URL, including the specifically observed same-endpoint redirect that removes the sole numeric `__generatorLastEditTime` cache parameter. Actual loaded-document identity and the outer source remain independently pinned across script execution. Only the configured host or its subdomains,
same HTTPS port and exact path are accepted. Ads/other frames are excluded and their own navigation does not cancel review. Native
frame handles are disposed; the browser instance remains host-owned. Revalidation
after awaits, cancellation, navigation revision and workspace/context snapshots
reject stale results. Script audits contain operation names only. Each invocation routes across awaits
to its originating window; closing the action cancels its token and rejects retained
callbacks, even when another window/action for the same plugin is active.

The publicly editable generator source declares `createTextEditor =
{import:text-editor-plugin-v1}`, sets `storySoFarEl.id = "storySoFarEl"`, and its
copy/export function reads `storySoFarEl.value`. This is the public DOM interface
used by the adapter, without inspecting private CodeMirror internals. The site's
export excludes `SUMMARY^` paragraphs; the adapter removes those marked blocks
while keeping retained prose/line endings. Chromium DOM/source inspection confirms
the structure; it does not qualify CefSharp or a human-generated round trip.

`tools/fixtures/perchance-story-writer.html` documents the local DOM contract;
`node tools/Test-PerchanceAdapter.cjs` runs 50 deterministic cases with a modeled
DOM and no network. `PerchanceWorkflowTests` adds 30 cases for frame policy,
serialization, duplicate/size/Unicode handling and revision transactions. The
existing 32 foundation/signature cases remain. Windows CI builds the actual app,
runs these focused cases and the full regression suite.

## Story Writer recovery (2026-10-14)

The adapter requires exactly one `stopBtn` generation indicator and exactly one
editable story document. A missing or duplicate indicator is an incompatible-page
error rather than evidence that generation has finished. Hidden/collapsed output,
disabled or read-only controls, disabled fieldsets, `aria-disabled`, inert ancestors
and a CodeMirror document that is no longer editable are rejected. The plugin never
changes those states to make insertion or import succeed. Prompt insertion still
preserves the existing story and checks both reviewed values immediately before writing.

Malformed JSON and oversized text fields from the page produce a recovery dialog
instead of an exception that dismisses the toolbar. `PerchanceAdapterResultTests`
adds 20 deterministic cases for response parsing, UTF-16 limits, Unicode and safe
recovery messages. Unrecognized codes and page values are never echoed in diagnostics.
The focused Windows CI step includes these tests.

| Error | Author-controlled recovery |
| --- | --- |
| Missing/ambiguous generation indicator or editor structure | Reload Story Writer, open the full editor and retry the explicit action. |
| Hidden editor | Open the full story editor on the website, then choose Import Result again. |
| Locked overview/editor | Finish the website operation or unlock the relevant control on the website before retrying. |
| Changed story/overview during review | Review the current page again; no reviewed value is overwritten. |
| Overview not retained after insertion | Inspect it on the website and review the prompt before retrying. |
| Wrong page or incompatible response | Return to Story Writer or use Other providers to restore normal provider selection. |

The DOM harness models the documented fixture; it does not run CefSharp or certify
live website compatibility. Manual recovery, toolbar/lifecycle and signed-install
checks remain part of the M1–M2 qualification below.

## SDK and host design freeze

`IPluginUiRegistry.RegisterAiGenerationToolbar` is an additive default-interface
method. Older implementations reject it with a compatibility message. The host
creates controls from a bounded action list rather than giving ordinary plugins
private WPF controls or visual-tree access. Registration requires
`ui.aiGenerationToolbar` plus `prompt.read`, because action snapshots contain the
already prepared prompt. Lists are copied at registration and duplicate ids are
rejected. Match both plugin owner and stable plugin configuration id. A normal
provider with the same URL/name is not eligible.

All placement, action invocation and restoration run on the WPF dispatcher.
The host retains an always-available Other providers button, removes handlers on
restoration, cancels active action tokens and rejects stale actions. Back suppresses
the replacement until the selected provider changes; ordinary registry refreshes
must not immediately reopen it. Navigation away restores the original toolbar.

The internal `PluginAiGenerationToolbarSurface` owns the WPF placement, button
handlers and cancellation lifetime, not the browser or provider policy. It restores
the whole standard bar, clears replacement controls/tags and detaches Click handlers
**before** canceling plugin callbacks. Any exception while creating the bar or
refreshing its chapter context/actions triggers the same safe fallback; exception
text is not exposed because it may contain private story data. An obsolete lifetime
is disposed without disposing a new bar created during cancellation.

`PerchanceToolbarLifecycleTests` supplies 12 deterministic Windows STA cases using
actual WPF controls: whole-bar replacement, Other providers, retained handlers/tags,
creation/context failures, replacement/repeated cleanup, canceled browser invocation,
throwing/reentrant cancellation and owned registry removal/reinstallation with a
foreign contribution preserved. These tests create no Chromium browser, send no
manuscript and do not call Perchance. They are included in focused Windows CI and
the regression suite; they do not replace the complete live-window/install smoke gate.

`PluginAiConfiguration.ManualBrowserWorkflow` defaults to false. The runtime-only
host copy disables unsolicited capture, automatic revision saving and the host generation command even when
the replacement toolbar has been dismissed. Existing providers keep their behavior.
An old readback await cannot apply content after switching to a manual provider.
This flag is not persisted into unrelated user provider settings.

Prompt/detail integration follows existing SDK contracts: Smart Builder already
prepares selected prompt rows; toolbar actions receive the current guide/command.
Chapter selection edits must use `ChapterEditorActionService` and its bounded,
single-use revision-checked apply callback. Main/alternative detail versions use
`RegisterEntryManagementAction` / `ApplyDescription`; Main is read-only in Versions.
Browser capture must add a new guarded draft review transaction before any save.
The plugin asks for no story.write, direct-generation or network capability.
`prompt.write` is used only for bounded host-owned description/creation callbacks after
review; it grants no bypass of current-editor checks. Creation uses normal host model
tracking/autosave only after explicit approval and never replaces/assigns existing data. `aiGeneration.browserAccess`
permits only explicit reviewed frame operations in its owned browser workflow;
chapter import uses the separate host-owned new-revision transaction.

## Ordinary signing

Debug and Release import the same SDK packaging target as OpenAI/Ollama, with
`ArclyraPluginDeveloperName=Arclyra` and a build-only reference to PluginSignTool.
The external directory is exactly
`NormalizePath($(MSBuildProjectDirectory), '..', '..', 'plugin-keys')`:

- `arclyra-developer-private.key`
- `arclyra-developer-public.key`
- `arclyra-developer.sig`

The private key stays outside the checkout and archive. No new operator key is
created. `ArclyraPluginRequireSigning=true` prevents an empty signing-tool setting
from producing an unsigned package. Missing files fail before packaging. Signing
uses `sign-package`; `verify-package` then reuses the host's ordinary developer
identity/content verifier and pinned master public key. Failed signing or
verification removes the incomplete archive. Payload remains root `plugin/` with
detached root `plugin.package.json`; the SDK DLL is excluded.

Real external key presence and successful Debug/Release signing have not been
qualified in this environment. Windows/operator qualification remains required;
local negative fixtures are not substitutes for a valid production signature.

## Terms and privacy review

Sources inspected on 2026-10-13:
<https://perchance.org/terms-of-service> (dated July 18, 2026) and
<https://perchance.org/privacy-policy>.

Perchance says AI output can be used commercially without required attribution
and does not claim its copyright. That does not constitute approval of this
integration: the terms also restrict automated/nonhuman access. The intended
workflow is author-operated browsing with separate user-triggered insertion and
import. This is an integration design interpretation, not a legal determination
or evidence of Perchance endorsement. Resolve distribution/permission questions
before release; do not represent official permission as already granted.

Perchance's policy says its text plugin does not retain generation data server-side,
while optional sharing/upload features can store it. It describes separated ad
iframes and recommends care with arbitrary community pages. These are provider
claims, not Arclyra guarantees. Inference still sends approved text externally;
browser storage can retain story data. Keep advertisements/cookies/navigation
unchanged, explain selected data in prompt review and avoid private-text logs or
plaintext plugin caches. Never invoke site sharing/upload controls.

## Qualification still required for M1–M3

The [Windows operator qualification procedure](perchance-qualification.cs.md) runs
`tools/Test-PerchanceQualification.ps1` for both normal developer-signed Debug/Release
packages, verifies the existing host trust root, checks identity/tamper rejection,
and writes a bounded non-secret report. It never installs or starts a browser.
`Test-PerchanceQualificationContract.ps1` adds 32 synthetic contract cases to CI;
passing these fixtures is not evidence of real-key signing or manual qualification.

1. Windows toolbar smoke: owned provider only, Other providers, navigation,
   plugin enable/disable/uninstall/update, failure and window reopening; no duplicate
   toolbar and no leaked handlers. Existing providers/direct Arclyra AI keep working.
2. Real external-key Debug/Release builds, verified ordinary installation, tamper
   rejection, invalid/missing keys and mismatched identity; no unsigned artifacts.
3. Live CefSharp iframe execution, insertion, full-document capture, review cancellation
   and chapter/workspace switching. Deterministic DOM/revision cases are automated;
   native browser/website behavior still needs manual qualification.
4. Human-generated Windows/CefSharp round trip with advertisements visible; verify
   the new revision, Unicode, summaries, long/offscreen text and older/final drafts.

## Embedded browser lifetime and responsive startup (2026-10-14)

The initial shutdown fix dispatched `Cef.Initialize` to the WPF UI thread. This
was superseded after user feedback because native initialization can block that
thread. `CefRuntimeThread` now owns one persistent dedicated STA thread for both
`Cef.Initialize` and `Cef.Shutdown`, with `MultiThreadedMessageLoop = true`.
Callers await readiness without occupying the UI dispatcher or a thread-pool
worker for CEF's lifetime. Concurrent calls share one initialization; failed or
stopped runtimes cannot initialize a second time.

`App.OnStartup` disables `CefSharpSettings.ShutdownOnExit` before any browser
static constructor can register its WPF shutdown hooks. Normal close closes the
generation windows and disposes WPF browser resources on their owning dispatcher,
then awaits native shutdown on the lifetime thread while the UI continues pumping.
`App.OnExit` covers explicit early application shutdown as a final exit fallback.
The fallback may wait at process exit for an already-running native initialization;
native initialization cannot be interrupted safely. Normal loading never performs
that wait on the UI thread.

The AI generation window initially contains a loading surface. Only after explicit
readiness does it construct `AIGenerationControl` and its Chromium control, preventing
the browser constructor's implicit UI-thread initialization fallback. A window closed
during loading creates no browser. Validated pending conversation/provider requests
are applied after readiness; provider ownership is revalidated by the existing
control. Loading callbacks use asynchronous dispatcher posts and are detached before
disposal so native shutdown does not wait on a blocked UI callback.

Nine `CefRuntimeThreadTests` cover thread ownership, slow startup, concurrent calls,
early close, close during initialization, failure transport, exactly-once shutdown
and rejection after stopping. Five `AiGenerationBrowserStartupTests` use real STA/WPF
window surfaces with a fake content factory: no browser before readiness, no browser
after close/failure, safe pending URLs and UI heartbeat during blocked initialization.
These fixtures do not start native Chromium and run in focused Windows CI and the
regression suite.

The CEF maintainer [describes the persistent-thread contract in multi-threaded mode](https://www.magpcss.org/ceforum/viewtopic.php?f=6&t=270).
The pinned [CefSharp 146 WPF source](https://github.com/cefsharp/CefSharp/blob/cefsharp/146/CefSharp.Wpf/ChromiumWebBrowser.cs)
shows the automatic shutdown hooks and implicit constructor initialization; the
[146 lifetime implementation](https://github.com/cefsharp/CefSharp/blob/cefsharp/146/CefSharp.Core.Runtime/Cef.h)
checks initialization/shutdown thread identity.

Manual Windows smoke: drag/type in Arclyra during cold preload; immediately open a
generation window and confirm the loading surface remains responsive; close it before
readiness, then reopen; open Perchance and another provider; close the entire app both
during preload and with a browser open. Check pending saved conversations/provider
selection, the reported shutdown exception and leftover browser subprocesses.
Native Windows/CefSharp qualification remains open.

## 0.5.1 browser and toolbar corrections

Preview Prompt was removed because Insert Prompt already supplies editable review.
Host buttons use the existing toolbar template that honors Padding, caption text,
a compact minimum height and centered vertical alignment.

Frame operations no longer depend on the whole browser loading flag, which also
tracks unrelated ad frames. They validate the uniquely matched visible Story Writer
iframe and document revision instead. Browser-context validation is separate from
the chapter import transaction: reads/insertion do not require a manuscript save
lease, while reviewed imports still validate it before committing. Canonical URL
comparison retains origin, path and query identity. Recovery messages identify the
failing stage without including prompt, page values or URLs.

Retest on Windows: Insert Prompt, manually Generate/Continue, observe partial prose
in the chapter preview, wait for website completion, Import Result and Save new
revision. Also test ad loading, Stop preview, manual draft edits, chapter/workspace
switch, provider exit and plugin disable. Native CefSharp/UI/signing qualification
remains an operator gate; deterministic fixtures do not certify a live round trip.

## 0.5.2 observed iframe redirect and compact action menus

On 2026-10-14 the visible Story Writer iframe source was
`https://d4da62dba2215e294c0d13d0fd16ebe5.perchance.org/ai-story-generator?__generatorLastEditTime=1789650407585`,
but its loaded document URL was the same origin/path with no query. Previous exact
source-vs-loaded URL matching therefore rejected the real editor before running
any adapter. Only that sole numeric Perchance cache parameter may now disappear
when matching one direct native frame. The loaded URL is captured and must remain
exact throughout execution, as must the outer iframe source; arbitrary query
removal, another endpoint, nested frames and ambiguity still fail closed.
The real visible editor was verified to expose complete `storySoFarEl.value` as a
string and one editable component. No model request or generation was performed.

The AI Generation provider and toolbar are labeled **Perchance**. The ordinary
installed plugin retains its full name **Perchance integration by Arclyra** and id.
**Other providers** shows the standard toolbar with an explicit **Perchance tools**
button to return without changing the provider or reloading its page. The return
button is unavailable on foreign pages/providers or after plugin unload. Selecting
Perchance again after switching providers still restores its normal toolbar.

Prompt setup Details and alternative Versions expose one **AI tools** menu for all
registered providers/actions. Original action names, ordering and tooltips are
retained; **Save** remains separate. Menu dispatch uses the unchanged source-bound
guarded workflows. Main Versions remains ineligible; busy/disabled editors cannot
invoke retained menu items, and refresh/unload closes menus and removes callbacks.
Resolver regressions use actual CefSharp interfaces through native-free proxies;
menu tests use real WPF STA controls. These tests cover routing and lifecycle,
while the full native Windows/CefSharp prompt-to-stream workflow remains a manual
qualification gate. Build the host 58 and plugin 0.5.2 together.

## Prompt-detail browser handoff (0.5.5 / host 61)

Both Details/alternative Versions actions now insert directly after the request
review's **Insert Prompt** confirmation, rather than waiting for another toolbar
click. The host binds this user action to the already-open owned manual browser,
its active toolbar, document revision, workspace and captured editor lifetime.
Browser service calls use the captured window and plugin identity across awaits;
no global-window fallback or retained post-completion access is allowed.
Each frame operation checks that context before and after execution and links
its cancellation to both the plugin service and user action. Navigation or
a concurrent toolbar operation invalidates the detail handoff. Existing iframe
identity, adapter bounds and exact overview/story checks are retained.

Failed/canceled insertion releases the transient task so a retry can start.
Successful insertion records the acknowledged canonical overview and existing
story baseline, enabling Import Detail/Import Ideas without a second insertion.
Generate/Continue, import reviews and Save remain author-controlled. SDK signatures,
capabilities, ordinary signing and the eight-action toolbar are unchanged.
Registered-action regression tests cover Details and alternatives, both tools,
edited Unicode/newlines, reviewed import, cancellation, stale/read-only targets,
foreign providers, failed frame operations and empty-task no-fallback behavior.
Native human-generated Windows/CefSharp qualification remains open.

## Dedicated detail workflow (0.6.0 / host 62)

The previous prompt-detail workflow through cumulative Story Writer is superseded.
Revision tasks now use **Perchance · Text Rewriter** (`/ai-text-rewriter`);
Creative Builder uses **Perchance · Text Generator** (`/ai-text-generator`).
The existing chapter provider remains Perchance on Story Writer. These are
ordinary owned manual browser providers with separate four-action text toolbars,
not a custom published generator.

1. Open the ordinary AI Generation Window and select any Perchance provider.
   If it is closed or another provider is selected, the plugin reports that before
   editor validation or any setup form.
2. In Prompt details / an alternative version, select a Perchance AI tool. One
   custom host form contains the task selector, editable output language, additional
   instructions and the complete request. Only the captured unsaved detail/version
   and applicable reference fields are shared. The complete preview updates with
   these fields; Cancel allows editing the original source before trying again.
3. **Insert Prompt** explicitly approves replacing the text tool's instructions/input
   and selecting the dedicated page in the already-open browser. An existing result
   is preserved. The form replaces the old sequence of task/language/text boxes.
   Rewriter uses custom mode, customModeInstructionEl and inputTextEl; the general
   generator uses instructionEl. Both read only responseEl. No story editor, page
   text, viewport lines, advertisements or model endpoints are involved.
4. Preparation ends immediately after successful insertion, so AI tools is usable
   while the task waits for manual generation/import. **Cancel pending AI task**
   in that menu or Cancel Text Task in the browser discards it. Starting a new task
   cancels the old pending task without applying it. Old asynchronous cleanup cannot
   dispose the newer task. Source changes/navigation/unload still reject its writer.
5. Generate / Convert text manually on the website. **Import Detail** and **Import
   Ideas** keep the existing full source/result/diff and approved destination flows.
   Import binds to the acknowledged mode/instruction/input snapshot and requires a
   response different from the preserved insertion baseline. Website input edits
   invalidate import; output is never assumed complete just because it is unchanged.

The adapters were inspected on the public visible pages on 2026-10-15 without
typing, generation or network calls to models. Both stopBtn and bottomStopBtn must
be uniquely present and idle. Locked/hidden/ambiguous fields, wrong exact page/origin,
changed request/result and bounded-size violations fail visibly. Responses preserve
Unicode and textarea newline canonicalization. The serialized combined input
snapshot and each text field must fit 262,144 characters. No persistent cache is
added. Ordinary signing, nine capabilities and SDK assembly identity stay unchanged.

Full-text review buttons now opt into PrimaryButtonStyle / SecondaryButtonStyle,
whose templates honor padding, with a minimum 36-pixel height, 100-pixel width and
18/8 horizontal/vertical padding. Native Windows/CefSharp human-generated page
switching, conversion/import and real-key signing remain release qualification gates.

Public references: https://perchance.org/ai-text-rewriter and
https://perchance.org/ai-text-generator.
