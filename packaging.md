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

Use the signing metadata workflow described in [Plugin signing tool](signtool.md) when distributing reviewed packages. Keep signing metadata compatible with host package inspection and validation.
