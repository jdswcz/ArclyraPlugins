# Arclyra Plugin SDK

SDK **1.31.0** provides .NET 8 Windows/WPF contracts, DTOs, helpers and MSBuild packaging targets for Arclyra Writing Studio extensions. Assembly identity stays **1.2.0.0**; file version is **1.31.0.0**.

Sources match ArclyraWritingStudio **master** commit `af732dfbb4d9d56b5dc08ab060db1c50582e5451` (October 21, 2026), host **78**. The package README is adapted for this standalone repository. This source update does not publish a NuGet package.

Since 1.23, additions include outline-aware browser prompt preparation and draft discard, review checkbox buffers, source-bound derived browser context, local summary requests, selection-or-document editor actions, native continuity memory and explicit active browser automation sessions. Each feature has its own minimum host; active sessions require host **77.0**, native memory **72.0** and local summaries **70.0**. Use **78.0** for the complete checked baseline.

See the [developer guide](https://github.com/jdswcz/ArclyraPlugins/blob/master/README.md), [compatibility matrix](https://github.com/jdswcz/ArclyraPlugins/blob/master/sdk-compatibility.md) and [compilable recipes](https://github.com/jdswcz/ArclyraPlugins/tree/master/SamplePlugins/Arclyra.SdkExamples) for setup, capabilities, cancellation, freshness and explicit review. The [SDK source](https://github.com/jdswcz/ArclyraPlugins/tree/master/src/Arclyra.PluginSdk) is the complete contract reference.

Do not bundle `Arclyra.PluginSdk.dll`; the host supplies the shared assembly. Default interface implementations do not add APIs to older SDK binaries, so set the appropriate minimum host and check optional services/callbacks. Packaging targets and the PowerShell helper are included. See [packaging](https://github.com/jdswcz/ArclyraPlugins/blob/master/packaging.md) for local build/pack and signing.
