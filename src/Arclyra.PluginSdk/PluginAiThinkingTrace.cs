using System.Text;

namespace Arclyra.PluginSdk;

/// <summary>Bounds an optional provider-supplied reasoning preview without interrupting generation.</summary>
public sealed class PluginAiThinkingTrace
{
    public const int MaxCharacters = 131072;
    private readonly StringBuilder _text = new();
    private bool _truncated;
    public bool HasContent => _text.Length > 0;
    public int Length => _text.Length;

    public void Append(string? text)
    {
        if (string.IsNullOrEmpty(text)) return;
        int remaining = MaxCharacters - _text.Length;
        _text.Append(text.AsSpan(0, Math.Min(remaining, text.Length)));
        _truncated |= text.Length > remaining;
    }

    public string Snapshot() => _text.ToString() + (_truncated ? "\n\n[Thinking preview truncated at 131,072 characters. Generation continues.]" : string.Empty);
}
