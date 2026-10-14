# Arclyra AI language implementation and qualification

Arclyra AI 0.3.1 uses the 35-code catalog declared in M3 of DEVELOPMENT_PLAN.md for Full (22B) and Compact (9B). English is the default and first selector item. The application's interface remains English. Generation language controls the internal writer, creative planning, prompt details, feedback analysis/revision and final research synthesis; source titles and quotations remain intact.

## Settings and compatibility

The primary **Generation language** selector saves a stable code in the plugin data directory under `settings/language.json`. Changes apply to the next generation. Missing, unknown or malformed language values use English; unreadable files remain untouched until an explicit selection is saved. The former `settings/system-prompt.json` is retained but no longer read. A raw `SystemPrompt` JSON property cannot replace the product template. The internal settings type keeps its old name to limit implementation churn; its prompt is derived and read-only.

One language-neutral product writer template inserts the selected language directive. Internal JSON keys and research control/schema tokens remain stable. Private captured system prompts remain available to replay the exact original correction context; they are not an editable product setting. Existing protected story, correction and feedback formats remain intact.

Historical corrections, feedback sources/lessons, repair checkpoints and continuity digests without language metadata are conservatively treated as Czech. New correction vectors are scoped to their original generation language. New grammar/language/style lessons carry a language code; newly analyzed language-independent lessons and story/factual memories remain reusable. Switching language preserves all data and excludes incompatible language-derived learning. Resume/feedback requires the original generation language. `GlobalCzech` remains the serialized enum identity for compatibility; the visible label is **Global language**.

Czech legacy lexical matching remains available for Czech learning. Other languages keep Unicode marks and use no Czech stemming/stop-word rules. Content persistence, protected storage, HTTP and local temporary inputs retain Unicode. Chapter output, draft preview and editable chapter content choose direction from Unicode letters, without switching the surrounding interface. Refusal patterns add selected-language matching for Arabic, Cyrillic, Greek, Hindi and CJK plus several Latin languages; custom Unicode markers and manual classification remain available for every language. Fast patterns are heuristic, not a semantic quality guarantee. Non-Czech vector contrasts use language-specific bootstrap continuations and captured/approved outputs instead of Czech boilerplate. Native correction stability still requires model qualification. Technical quality checks accept Unicode letters from every supported script, count character/syllable units for scripts without reliable whitespace boundaries, and retain encoding/control-token/completion/repetition guards. They do not classify the actual natural language or certify literary quality.

## Automated validation

The deterministic regression matrix exercises all 35 language directives through mocked planning and writing HTTP requests using both model configurations. Additional tests cover stable-code persistence, corrupt/legacy settings, absence of a raw-prompt editor, Unicode round trips, language-scoped correction and feedback retrieval, native-script refusal matching and Arabic content direction. CI builds the Windows host, SDK/plugin projects, packages and native-qualification harness; it does not download model weights or certify translation quality.

## Real model release matrix

Close Arclyra. On the Windows machine with both approved GGUFs/runtimes installed, run:

```powershell
.\tools\Test-ArclyraAIQualification.ps1 -PluginDataDirectory 'C:\path\to\plugin-data' -Backend Automatic -LanguageSmoke -Model Both -OutputDirectory 'C:\ArclyraQualification\Languages'
```

Add `-Prepare` only when components need preparation; each proposed download asks for confirmation. `-Model Full` and `-Model Compact` allow separate runs. Ctrl+C cancels the owned runtime. Original language and performance files are restored byte for byte, including after failure. Language smoke uses an isolated research-settings store with web research off. Existing package/component verification stays enabled.

The harness collects prose, prompt-detail and an in-memory feedback revision through the actual generation pipeline for 70 model/language cases, saves per-case UTF-8 files including `revision.txt` and local trace plus `language-smoke.json`, and records raw outputs for human review. It never declares release approval. Outputs/trace can contain generated writing and should remain local unless deliberately shared.

Review every language for language fidelity, grammar/style, literary form, formatting and Unicode. Additionally exercise poetry, multilingual story/source context, correction/feedback/refusal stop/mark/resume, source synthesis with approved research providers, and Arabic display/edit/save/reload in the Windows UI. A completed collection is evidence for review, not proof these gates passed. Auto performance calibration, operator-owned signing key/public pin, signed package/catalog publication and first-click release smoke remain independent gates.
