# Arclyra Plugin Template

This project is a minimal starting point for Arclyra Writing Studio plugin developers. It builds a WPF class library that implements `IArclyraPlugin`, includes a `plugin.json` manifest, and packages the output as an `.arcplugin` archive through the Arclyra Plugin SDK targets.

## Customize the template

Before publishing a plugin, update these values:

1. Rename the project, namespace, assembly, and `PluginTemplatePlugin` type.
2. Replace the example reverse-DNS plugin id in `PluginTemplatePlugin.Id` and `plugin.json`.
3. Update `plugin.json` metadata such as `name`, `version`, `description`, `author`, `entryAssembly`, and `entryType`.
4. Add only the capabilities your plugin needs. See [capabilities](../../capabilities.md) for available capability constants and extension points.
5. Add commands, UI registrations, event subscriptions, or Smart Builder integrations in `InitializeAsync`.

## Build

From the repository root, run:

```bash
dotnet build SamplePlugins/Arclyra.PluginTemplate/Arclyra.PluginTemplate.csproj
```

The SDK packaging target runs `package-arcplugin.ps1` after build and writes an `.arcplugin` archive to the project output folder when packaging is enabled.

## Signing and unsigned installs

Before distributing your `.arcplugin` publicly, sign it with `Arclyra.PluginSignTool` so Arclyra Writing Studio can associate the package with your validated developer identity and detect package tampering. Signed packages let Arclyra show the validated developer name from the signature metadata during installation and plugin management.

Arclyra Writing Studio supports installing unsigned plugin packages for development and private sharing, but users must complete an extra confirmation step. Unsigned packages display **Unknown author** even if `plugin.json` includes an `author` value.

The template references SDK **1.35.0**. Its minimal entry point does not use SDK 1.32–1.35 additions, so its manifest deliberately retains the **78.0** baseline rather than unnecessarily requiring host 115. Set `minHostVersion` to the newest API your implementation actually calls: 86 for typed summary progress, 92 for source-bound generation, 102/104 for privileged prompt review (first-party only), and 115 when reading `PluginChapterDto.CurrentDraftId`. [SDK recipes](../Arclyra.SdkExamples/README.md) give usage patterns. Pack the SDK locally before building if 1.35.0 is not on your configured NuGet feed; see [sample build commands](../README.md). The package excludes the host-owned SDK DLL. The template's `0.1.3` version is an example version, not the SDK version.
