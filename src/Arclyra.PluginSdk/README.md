# Arclyra Plugin SDK

SDK **1.23.0** provides the public .NET 8 Windows/WPF contracts, DTOs, helpers and MSBuild packaging targets for Arclyra Writing Studio extensions. The SDK assembly identity remains **1.2.0.0**; file version is **1.23.0.0**. Require host **63.0** for the new 1.23 browser behavior.

The sources are synchronized from Arclyra Writing Studio `dev` commit `d98236ec0f6990731d068763f83af60ac6d47e89` (October 15, 2026). This source update does not publish a package to NuGet.

See the [Plugin Developer Guide](https://github.com/jdswcz/ArclyraPlugins/blob/master/README.md) for setup and local build/pack commands, [compatibility and migration](https://github.com/jdswcz/ArclyraPlugins/blob/master/sdk-compatibility.md) for minimum host versions, and [SDK source](https://github.com/jdswcz/ArclyraPlugins/tree/master/src/Arclyra.PluginSdk) for the complete contract surface.

SDK 1.23 adds optional automatic browser observation preparation, compact contextual toolbar actions, reviewed full-story loading, incremental chapter previews and source-bound browser selection from chapter editor actions. Existing positional constructors remain compatible. Observation does not accept content or save revisions. See the [browser contracts](https://github.com/jdswcz/ArclyraPlugins/blob/master/ui-extension-api.md#automatic-browser-observation-and-story-loading-sdk-123--host-63).

Reference the SDK from your plugin project without bundling a private copy of `Arclyra.PluginSdk.dll`; the host supplies the shared contract assembly at runtime. Packaging targets and the PowerShell helper are included in the NuGet package. See [packaging and signing](https://github.com/jdswcz/ArclyraPlugins/blob/master/packaging.md).
