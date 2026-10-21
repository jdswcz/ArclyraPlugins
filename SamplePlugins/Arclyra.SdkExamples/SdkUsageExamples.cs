using Arclyra.PluginSdk;

namespace Arclyra.SdkExamples;

/// <summary>Opt-in recipes for ordinary plugins; no startup inference or manuscript saving.</summary>
public static class SdkUsageExamples
{
    // story.read; SDK 1.30 / host 72. Reading never triggers generation.
    public static Task<PluginStoryMemoryDto> ReadMemoryAsync(
        IPluginContext context, PluginStoryMemoryRequest request, CancellationToken cancellationToken)
    {
        IPluginStoryMemoryService memory = context.StoryMemoryService
            ?? throw new NotSupportedException("Native continuity memory requires host 72 or newer.");
        return memory.GetAsync(request, cancellationToken);
    }

    // story.read + ai.summarize. Call only in response to an explicit refresh request.
    public static Task<PluginStoryMemoryDto> RefreshMemoryAsync(
        IPluginContext context, PluginStoryMemoryRequest request,
        IProgress<string>? progress, CancellationToken cancellationToken)
    {
        IPluginStoryMemoryService memory = context.StoryMemoryService
            ?? throw new NotSupportedException("Native continuity memory requires host 72 or newer.");
        return memory.RequestFreshAsync(request, progress, cancellationToken);
    }

    // ai.summarize; SDK 1.28 / host 70. Returns a proposal; the caller owns source freshness/review.
    public static async Task<PluginStorySummaryResult> SummarizeAsync(
        IPluginContext context, string historicalProse, int maximumWords,
        IProgress<string>? progress, CancellationToken cancellationToken)
    {
        IPluginStorySummaryService summarizer = context.StorySummaryService
            ?? throw new NotSupportedException("Local summaries require host 70 or newer.");
        PluginStorySummaryProfile profile = await summarizer.GetProfileAsync(cancellationToken);
        return await summarizer.SummarizeAsync(
            new PluginStorySummaryRequest(historicalProse, null, maximumWords, profile.Identity),
            progress, cancellationToken);
    }

    public sealed record PromptFields(string MainPrompt, string SeparateOutline);

    // ui.dialogs plus the owned toolbar's prompt.read; SDK 1.25 / host 66.
    // The recipe returns fields only: it does not insert into a website or save anything.
    public static PromptFields? ReviewPromptFields(
        IPluginContext context, PluginAiGenerationPasteActionContext action)
    {
        action.CancellationToken.ThrowIfCancellationRequested();
        if (action.CurrentRenderedPrompt is not { } completePrompt)
            return null;

        string outline = action.CurrentRenderedChapterOutline ?? string.Empty;
        PluginTextReviewOption? option = null;
        if (!string.IsNullOrWhiteSpace(outline) &&
            action.CurrentRenderedPromptWithoutOutline is { } withoutOutline)
        {
            option = new PluginTextReviewOption("Insert outline in a separate field", withoutOutline)
            {
                AllowEmptyTextWhenSelected = true
            };
        }

        PluginDialogInputResult reviewed = context.DialogService.ShowTextReview(
            new PluginTextReviewRequest("Review prompt", "Edit the prompt before insertion.", completePrompt)
            {
                Option = option
            }, action.CancellationToken);
        action.CancellationToken.ThrowIfCancellationRequested();
        return reviewed.IsConfirmed
            ? new PromptFields(reviewed.InputText, reviewed.IsReviewOptionSelected == true ? outline : string.Empty)
            : null;
    }

    // ui.editorActions + ui.dialogs; SDK 1.29 / host 71. Explicit review, one guarded edit, no save.
    public static void RegisterTrimAction(IPluginContext context)
    {
        IPluginChapterEditorActionService actions = context.ChapterEditorActionService
            ?? throw new NotSupportedException("Built-in chapter editor actions are unavailable.");
        actions.RegisterActionMenu(new PluginChapterEditorActionMenuRegistration(
            "com.example.sdkrecipes.editor", "SDK recipes",
            [new PluginChapterEditorActionRegistration(
                "com.example.sdkrecipes.trim", "Trim outer whitespace",
                PluginChapterEditorEditKind.ReplaceSelectionOrDocument, RequiresSelection: false,
                ExecuteAsync: async (action, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    if (action.IsSnapshotCurrent?.Invoke() != true)
                        return;
                    PluginChapterEditorSnapshot snapshot = action.Snapshot;
                    string original = snapshot.SelectionLength > 0 ? snapshot.SelectedText : snapshot.Content;
                    if (string.IsNullOrWhiteSpace(original) || original == original.Trim())
                        return;
                    string scope = snapshot.SelectionLength > 0 ? "selected text" : "whole document";
                    PluginDialogInputResult reviewed = context.DialogService.ShowTextReview(
                        new PluginTextReviewRequest("Review trimmed text", $"Target: {scope}.", original.Trim())
                        {
                            OriginalText = original
                        }, token);
                    token.ThrowIfCancellationRequested();
                    if (!reviewed.IsConfirmed || action.IsSnapshotCurrent?.Invoke() != true)
                        return;
                    PluginChapterEditorApplyResult result = await action.ApplyAsync(reviewed.InputText, token);
                    if (!result.IsApplied)
                        context.Logger.LogWarning($"Editor proposal was not applied: {result.Status}.");
                })]));
    }

    // ui.aiGenerationToolbar + prompt.read + aiGeneration.browserAccess; SDK 1.31 / host 77.
    // Acquire during an explicit toolbar action; return from that action before awaiting lease work.
    public static IPluginBrowserAutomationSession BeginAutomation(
        PluginAiGenerationPasteActionContext action, string configurationId, string frameElementId)
    {
        action.CancellationToken.ThrowIfCancellationRequested();
        var begin = action.BeginAutomation
            ?? throw new NotSupportedException("Active browser automation requires host 77 and an explicit toolbar action.");
        var result = begin(new PluginBrowserAutomationSessionRequest(
            configurationId, frameElementId, TimeSpan.FromMinutes(5)));
        if (result.Success && result.Value is { } session)
            return session;
        throw new InvalidOperationException(result.StatusMessage ?? "The host rejected the browser session.");
    }
}
