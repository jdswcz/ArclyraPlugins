# Packaging

Arclyra installs `.arcplugin` packages into writable app-data plugin folders. Plugin data belongs under the plugin data root; do not install plugins into the executable directory, Program Files, or package install directories.

## Archive layout

A package is a zip archive with a `.arcplugin` extension and a root-level `plugin/` folder:

```text
plugin/
  plugin.json
  ExamplePlugin.dll
  ExamplePlugin.deps.json
  runtimes/
  assets/
```

`plugin.json` and the entry assembly must be directly under `plugin/`. Do not include absolute paths or entries that escape the `plugin/` folder.

## Build and pack

```bash
dotnet publish ExamplePlugin.csproj -c Release -o publish/plugin
(cd publish && zip -r ExamplePlugin.arcplugin plugin)
```

When using `Arclyra.PluginSdk`, reference the host SDK but do not bundle a private SDK copy in the package unless a documented compatibility scenario requires it.

## Manifest checklist

- `id` is reverse-DNS-style and matches `IArclyraPlugin.Id`.
- `name` matches `IArclyraPlugin.Name`; `version` is the manifest package version.
- `entryAssembly` points to the plugin assembly in `plugin/`.
- `entryType` is the full type name of the public plugin class.
- `capabilities` are least-privilege and documented for users.

## Signing

SDK 1.13 adds opt-in `ArclyraPluginRequireSigning=true`. It rejects an empty
signing-tool setting, checks the external key/signature files before creating a
package, then runs normal `sign-package` and `verify-package --developer-name ...`.
Verification uses the host's pinned ordinary developer trust root and validates
the content hash/signature; it rejects Trusted Component declarations. A signing
or verification failure removes the incomplete archive. Existing unsigned sample
builds retain the default false. The Perchance project requires signing in both
Debug and Release and uses the same external developer key triplet as OpenAI/Ollama.

Use the signing metadata workflow described in [Plugin signing tool](signtool.md) when distributing reviewed packages. Keep signing metadata compatible with host package inspection and validation.

First-party Trusted Components use a separate [trusted signing profile](trusted-components.md)
inside the same `.arcplugin` layout. A declared trusted package must pass dedicated
key/identity/integrity verification before replacement or load; ordinary developer
signatures and user review cannot satisfy it. Do not place detached signature
metadata both at the archive root and under `plugin/`.

Arclyra AI additionally runs its own trusted release-signing target after packaging
in Debug/Release Visual Studio builds. Its external private key must match the
public key pinned in the host; failed signing removes the unverified package.
See [the Trusted Component authority boundary](trusted-components.md). Ordinary SDK targets
and developer signing are unchanged.
