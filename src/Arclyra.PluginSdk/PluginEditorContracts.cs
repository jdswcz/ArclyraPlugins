using System.Windows;

namespace Arclyra.PluginSdk;

public enum PluginEditorTarget
{
    ChapterOutline,
    GeneratedChapterDraft,
    PromptDetail,
    StoryOverview
}

public interface IPluginEditorRegistry
{
    void RegisterEditor(PluginEditorRegistration registration);
}

public sealed record PluginEditorRegistration(
    string EditorId,
    string DisplayName,
    PluginEditorTarget Target,
    Func<PluginEditorContext, FrameworkElement> CreateEditor);

public sealed class PluginEditorContext
{
    public PluginEditorContext(
        PluginEditorTarget target,
        string storyId,
        string storyName,
        string? chapterId,
        int? chapterNumber,
        string documentId,
        string title,
        string content,
        Func<string, Task> saveAsync,
        Func<Task> cancelAsync,
        Func<string, Task>? validateAsync = null,
        IReadOnlyDictionary<string, string?>? metadata = null)
    {
        Target = target;
        StoryId = storyId;
        StoryName = storyName;
        ChapterId = chapterId;
        ChapterNumber = chapterNumber;
        DocumentId = documentId;
        Title = title;
        Content = content;
        SaveAsync = saveAsync;
        CancelAsync = cancelAsync;
        ValidateAsync = validateAsync;
        Metadata = metadata ?? new Dictionary<string, string?>();
    }

    public PluginEditorTarget Target { get; }
    public string StoryId { get; }
    public string StoryName { get; }
    public string? ChapterId { get; }
    public int? ChapterNumber { get; }
    public string DocumentId { get; }
    public string Title { get; }
    public string Content { get; set; }
    public IReadOnlyDictionary<string, string?> Metadata { get; }
    public Func<string, Task> SaveAsync { get; }
    public Func<Task> CancelAsync { get; }
    public Func<string, Task>? ValidateAsync { get; }
}
