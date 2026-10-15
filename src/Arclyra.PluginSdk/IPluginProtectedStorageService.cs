namespace Arclyra.PluginSdk;

/// <summary>
/// Host-owned, workspace-scoped storage for plugin data that may contain user content.
/// The host owns encryption keys and persistence; plugins never receive workspace
/// passwords or key material.
/// </summary>
public interface IPluginProtectedStorageService
{
    /// <summary>Opaque stable id of the currently open Arclyra workspace.</summary>
    string WorkspaceId { get; }

    /// <summary>
    /// Returns true when the named blob exists in the current workspace.
    /// Names are plugin-relative logical paths, for example "feedback/feedback.json".
    /// </summary>
    bool Exists(string name);

    Task<bool> ExistsAsync(
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads and decrypts one plugin-owned blob from the current workspace.
    /// Returns null when it does not exist.
    /// </summary>
    Task<byte[]?> ReadAsync(
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Atomically encrypts and replaces one plugin-owned blob in the current workspace.
    /// Plaintext bytes are never written to the destination or its temporary file.
    /// </summary>
    Task WriteAtomicAsync(
        string name,
        ReadOnlyMemory<byte> data,
        CancellationToken cancellationToken = default);

    /// <summary>Deletes one plugin-owned protected blob from the current workspace.</summary>
    Task DeleteAsync(
        string name,
        CancellationToken cancellationToken = default);
}
