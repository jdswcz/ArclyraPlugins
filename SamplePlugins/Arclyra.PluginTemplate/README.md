# Arclyra Plugin Template

This project is a minimal starting point for Arclyra Writing Studio plugin developers. It builds a WPF class library that implements `IArclyraPlugin`, includes a `plugin.json` manifest, and packages the output as an `.arcplugin` archive through the Arclyra Plugin SDK targets.

## Customize the template

Before publishing a plugin, update these values:

1. Rename the project, namespace, assembly, and `PluginTemplatePlugin` type.
2. Replace the example reverse-DNS plugin id in `PluginTemplatePlugin.Id` and `plugin.json`.
3. Update `plugin.json` metadata such as `name`, `version`, `description`, `author`, `entryAssembly`, and `entryType`.
4. Add only the capabilities your plugin needs. See `Arclyra.PluginSdk/README.md` for available capability constants and extension points.
5. Add commands, UI registrations, event subscriptions, or Smart Builder integrations in `InitializeAsync`.

## Build

From the repository root, run:

```bash
dotnet build Arclyra.PluginTemplate/Arclyra.PluginTemplate.csproj
```

The SDK packaging target runs `package-arcplugin.ps1` after build and writes an `.arcplugin` archive to the project output folder when packaging is enabled.
