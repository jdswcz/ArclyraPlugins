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

When using `Arclyra.PluginSdk`, reference the host SDK without bundling a private SDK copy in the package. Arclyra supplies the shared contract assembly at runtime.

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
builds retain the default false. Set this property in each build configuration
that must produce a developer-signed package.

Use the signing metadata workflow described in [Plugin signing tool](signtool.md) when distributing reviewed packages. Keep signing metadata compatible with host package inspection and validation.

First-party Trusted Components use a separate [trusted signing profile](trusted-components.md)
inside the same `.arcplugin` layout. A declared trusted package must pass dedicated
key/identity/integrity verification before replacement or load; ordinary developer
signatures and user review cannot satisfy it. Do not place detached signature
metadata both at the archive root and under `plugin/`.

## SDK build targets

The NuGet package includes matching `build` and `buildTransitive` targets plus
`tools/package-arcplugin.ps1`. On a normal build, `PackArclyraPlugin` stages the
manifest and top-level output DLLs under `plugin/`, excludes host-shared assemblies,
and creates the archive in the project output directory. The helper does not stage
arbitrary content, native runtime folders or dependency JSON automatically; extend
your packaging workflow when these are required.

| MSBuild property | Default / purpose |
| --- | --- |
| `ArclyraPluginPackageOnBuild` | `true`; set to `false` for builds that should not create an archive. Design-time builds never package. |
| `ArclyraPluginPackageName` | `$(AssemblyName).arcplugin`. |
| `ArclyraPluginManifestFile` | `plugin.json` in the plugin project directory. |
| `ArclyraPluginPackageScript` | The SDK's bundled packaging helper; override for custom staging. |
| `ArclyraPluginPowerShellExe` | `powershell` on Windows, `pwsh` elsewhere. |
| `ArclyraPluginRequireSigning` | `false`; `true` requires signing inputs and verifies the signed archive. |
| `ArclyraPluginSignToolPath` | Path to the ordinary developer signing tool; empty keeps optional-signing builds unsigned. |
| `ArclyraPluginDeveloperName` | Validated developer identity used by the signing tool. |
| `ArclyraPluginDeveloperPrivateKey` | External private-key file; keep it outside the repository and package. |
| `ArclyraPluginDeveloperPublicKey` | External developer public-key file. |
| `ArclyraPluginDeveloperSignature` | External Arclyra-issued developer signature file. |

`ArclyraPluginHostSharedAssembly` is an MSBuild item list, defaulting to
`Arclyra.PluginSdk.dll`; it identifies assemblies excluded from staging. Keep the
SDK in that exclusion list when adding more host-shared assemblies. See [signing](signtool.md)
for the developer identity and key workflow.
