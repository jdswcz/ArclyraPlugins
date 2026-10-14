# Perchance generator for one Arclyra chapter

## Decision

A dedicated chapter generator is the preferred longer-term design. A cumulative
story textbox contains no durable chapter identity, so comparisons can establish
boundaries only when known prose or a retained session gives unambiguous evidence.
A generator designed for Arclyra can make that identity explicit and keep continuity
context separate from editable chapter prose. This would remove the need to infer
boundaries, prevent earlier chapters from becoming the current draft, and make
outgoing edits substantially simpler.

The current correction remains compatible with the existing Story Writer. It matches
saved working text and recent known revisions, streams the selected chapter, supports
reviewed outgoing edits, and pauses ambiguous or conflicting operations. Arbitrary
word-count splitting is removed from the Perchance toolbar.

## Official implementation surface

Use the documented `ai = {import:ai-text-plugin}` on a normal Perchance page.
The official documentation describes `instruction`, `startWith`, `outputTo`,
`onChunk(data.fullTextSoFar)` and `onFinish(data.generatedText)` for controlled
streaming. It also explains that importing the plugin displays advertisements to
non-logged-in users. Keep those advertisements and use the documented import rather
than calling or reverse-engineering model endpoints.

Sources inspected on 2026-10-14:

- https://perchance.org/ai-text-plugin
- https://perchance.org/text-editor-plugin-v1
- https://perchance.org/ai-story-writing-helper-example#edit
- https://perchance.org/terms-of-service
- https://perchance.org/privacy-policy

## Proposed page and contract

The author sees the story guide, previous-chapter continuity context, current chapter
instructions and one editable current-chapter textbox. Generate, Continue and Stop
remain explicit author-operated website controls. Previous text is supplied as context,
not as a prefix in the editable chapter textbox. Continuation starts from current
chapter prose only. There are no automatic generation chains or hidden background calls.

A small DOM contract identifies the contract version, chapter identity, editor revision,
full current text and generation state. Opaque Arclyra IDs remain DOM metadata; the UI
shows human-readable chapter number and title. Updating the guide or switching chapters
increments the revision. Changing an Arclyra chapter requires the matching identity,
revision, expected previous text, idle state and an explicit browser session.

`onChunk` updates the complete current-chapter field cumulatively. The host reads that
field, never rendered viewport lines. A stale response cannot write a different chapter.
Stop preserves partial text; completion does not automatically create an Arclyra revision
or promote a draft to final. Imported changes use the existing reviewed revision workflow.

A three-way baseline handles edits made on both sides. Independent changes can merge;
changes in the same chapter require a full current-text review. Story guide and continuity
context are updated separately from prose. The page does not silently persist Arclyra
manuscripts or upload unrelated chapters. Any optional browser storage is disclosed and
separate from author-controlled Arclyra persistence.

## Deployment gate

Do not register an arbitrary custom generator URL or replace Story Writer merely because
its origin ends with perchance.org. Publish only after the author approves the actual
source, account and stable URL. Pin the exact generator path in the ordinary plugin and
host policy; verify the unique direct frame and DOM contract. Keep the current generator
as a selectable fallback. No page has been created, published or enabled by this decision.

Before release, test the published page on native Windows/CefSharp: streaming, Stop,
continuation, switching chapters mid-generation, Unicode/newlines, review cancellation,
concurrent edits, browser navigation, advertisements and ordinary signed package update.
Use human-controlled generation. The dedicated page is an architectural next step, not
claimed production functionality in plugin 0.5.4.
