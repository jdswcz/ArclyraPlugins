using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arclyra.PluginSdk;

/// <summary>A text message in provider-neutral conversation order.</summary>
public sealed record PluginAiChatMessage(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content);

/// <summary>
/// Host-bound conversation transaction. Persistence callbacks must be atomic, story/plugin scoped,
/// and reject commits after the story or active data file has changed.
/// </summary>
public sealed class PluginAiConversation(
    string storyId, Func<string, string?> read, Action<string, string> commit, SemaphoreSlim gate)
{
    /// <summary>Maximum UTF-16 characters in message content; this is not a token estimate.</summary>
    public const int MaxContentCharacters = 131072;
    /// <summary>Maximum number of retained complete chapter turns.</summary>
    public const int MaxTurns = 32;
    /// <summary>Bound on a serialized conversation, including JSON escaping.</summary>
    public const int MaxStoredCharacters = MaxContentCharacters * 6 + 16384;

    /// <summary>
    /// Sends temporary messages and commits one complete turn only after successful generation.
    /// Requests without a host conversation remain isolated, one-shot requests.
    /// The scope must identify profile, normalized endpoint and model, never credentials.
    /// </summary>
    public static Task<PluginAiGenerationResult> GenerateAsync(
        PluginAiGenerationRequest request, string scope,
        Func<IReadOnlyList<PluginAiChatMessage>, CancellationToken, Task<PluginAiGenerationResult>> generate,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(generate);
        return request.Conversation is { } conversation
            ? conversation.GenerateCoreAsync(request, scope, generate, cancellationToken)
            : generate(CreateInitialMessages(request), cancellationToken);
    }

    /// <summary>Creates an isolated request, retaining both guide and current chapter prompt.</summary>
    public static IReadOnlyList<PluginAiChatMessage> CreateInitialMessages(PluginAiGenerationRequest request)
    {
        ValidatePrompt(request);
        List<PluginAiChatMessage> messages = [];
        if (!string.IsNullOrWhiteSpace(request.StoryGuide)) messages.Add(new("user", request.StoryGuide));
        if (!string.IsNullOrWhiteSpace(request.ChapterPrompt)) messages.Add(new("user", request.ChapterPrompt));
        return messages.AsReadOnly();
    }

    private async Task<PluginAiGenerationResult> GenerateCoreAsync(PluginAiGenerationRequest request, string scope,
        Func<IReadOnlyList<PluginAiChatMessage>, CancellationToken, Task<PluginAiGenerationResult>> generate,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(storyId) || request.StoryId != storyId)
            throw new InvalidOperationException("The AI conversation does not belong to this story.");
        ValidatePrompt(request);
        if (string.IsNullOrWhiteSpace(scope) || scope.Length > 8192)
            throw new InvalidOperationException("A bounded provider/profile/model conversation scope is required.");
        string key = Hash(scope);
        string guideHash = Hash(request.StoryGuide);
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ConversationDocument document = Load(read(key), storyId, guideHash);
            List<ChapterTurn> turns = document.Turns;
            // A successful regeneration replaces this chapter and its dependent later turns.
            int replacement = string.IsNullOrEmpty(request.ChapterId) ? -1
                : turns.FindIndex(turn => turn.ChapterId == request.ChapterId);
            if (replacement >= 0) turns.RemoveRange(replacement, turns.Count - replacement);
            Trim(turns, request.StoryGuide.Length + request.ChapterPrompt.Length, MaxTurns - 1);
            List<PluginAiChatMessage> messages = [];
            if (!string.IsNullOrWhiteSpace(request.StoryGuide)) messages.Add(new("user", request.StoryGuide));
            foreach (ChapterTurn turn in turns)
            {
                messages.Add(new("user", turn.Prompt));
                messages.Add(new("assistant", turn.Response));
            }
            if (!string.IsNullOrWhiteSpace(request.ChapterPrompt)) messages.Add(new("user", request.ChapterPrompt));
            PluginAiGenerationResult result = await generate(messages.AsReadOnly(), cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(result.GeneratedChapterDraft))
                throw new InvalidOperationException("The provider returned no completed chapter draft.");
            if ((long)request.StoryGuide.Length + request.ChapterPrompt.Length + result.GeneratedChapterDraft.Length > MaxContentCharacters)
                throw new InvalidOperationException("The completed turn exceeds the conversation safety limit. Reduce the Story Guide, chapter prompt or output limit and retry. History was not changed.");
            turns.Add(new(request.ChapterId, request.ChapterPrompt, result.GeneratedChapterDraft));
            Trim(turns, request.StoryGuide.Length, MaxTurns);
            string serialized = JsonSerializer.Serialize(document);
            if (serialized.Length > MaxStoredCharacters)
                throw new InvalidOperationException("The serialized conversation exceeds its safety limit. History was not changed.");
            cancellationToken.ThrowIfCancellationRequested();
            commit(key, serialized);
            return result;
        }
        finally { gate.Release(); }
    }

    private static void ValidatePrompt(PluginAiGenerationRequest request)
    {
        if (request.StoryGuide is null || request.ChapterPrompt is null
            || (string.IsNullOrWhiteSpace(request.StoryGuide) && string.IsNullOrWhiteSpace(request.ChapterPrompt)))
            throw new InvalidOperationException("Arclyra did not supply a story guide or chapter prompt.");
        if ((long)request.StoryGuide.Length + request.ChapterPrompt.Length > MaxContentCharacters)
            throw new InvalidOperationException("The Story Guide and current chapter prompt exceed the 131,072-character conversation safety limit. Shorten them before generating.");
    }

    private static ConversationDocument Load(string? json, string story, string guideHash)
    {
        if (json is not null && json.Length <= MaxStoredCharacters)
        {
            try
            {
                ConversationDocument? document = JsonSerializer.Deserialize<ConversationDocument>(json);
                if (document is { Version: 1, Turns: not null } && document.StoryId == story && document.GuideHash == guideHash
                    && document.Turns.Count <= MaxTurns
                    && document.Turns.All(turn => turn is not null && turn.Prompt is not null && !string.IsNullOrWhiteSpace(turn.Response))
                    && document.Turns.Sum(turn => (long)turn.Prompt.Length + turn.Response.Length) <= MaxContentCharacters)
                    return document;
            }
            catch (JsonException) { /* Unknown/corrupt history starts a fresh conversation. */ }
        }
        return new(1, story, guideHash, []);
    }

    private static void Trim(List<ChapterTurn> turns, int reservedCharacters, int maxTurns)
    {
        long characters = reservedCharacters + turns.Sum(turn => (long)turn.Prompt.Length + turn.Response.Length);
        while (turns.Count > 0 && (characters > MaxContentCharacters || turns.Count > maxTurns))
        {
            characters -= (long)turns[0].Prompt.Length + turns[0].Response.Length;
            turns.RemoveAt(0);
        }
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    private sealed record ChapterTurn(string? ChapterId, string Prompt, string Response);
    private sealed record ConversationDocument(int Version, string StoryId, string GuideHash, List<ChapterTurn> Turns);
}
