# Arclyra Sample Plugin — SDK 1.39.0

This intentionally harmless demo illustrates ordinary SDK UI, data, command, event and direct-generation extension points. The source project and manifest version are **1.6.4**, and the minimum host is **150.0** because this version actively registers a Kitty help topic (SDK 1.39). The demo generator separately uses SDK 1.33 source leases (host 92). This remains an **ordinary plugin**, not a trusted first-party component.

## Source-bound demo generation

`CreateDemoAiGeneratorConfiguration` sets `RequireCurrentSourceSnapshot = true`. `GenerateSampleChapterDraftAsync` requires `request.IsSourceSnapshotCurrent`, links the supplied generation token with `request.SourceSnapshotInvalidationToken`, and rechecks validity between its demo-only stages and before returning. When the author edits the source, changes chapter or switches workspaces, the host invalidates the lease and the handler does not return a completed draft. This demo does **not** emit a partial generation result; real generators that provide interrupted previews must throw `PluginAiGenerationIncompleteException`, not treat the partial text as approved.

The example generator does not call a remote model. The optional browser provider uses `about:blank` and does not contact a real AI service. No host-supplied SDK assembly is included in plugin output.

## Public help for Arclyra Kitty

`InitializeAsync` registers a single static `PluginKittyHelpTopic` through `context.KittyHelp`. When the sample plugin is active on host 150+, Kitty can answer questions about its demo `about:blank` provider using public instructions. No model call is initiated by registration. The host owns discovery, relevance, citation and revocation when the plugin unloads. The topic contains no prompts, story data or account details.

Unlike the help registry, `IPluginAssistantChatService` is **first-party only**; this ordinary sample cannot register, read or intercept Kitty conversations. See [host services API](../../host-services-api.md#plugin-help-for-kitty-sdk-139--host-150).

## Build

From the repository root on Windows with the .NET 8 SDK, first create the local SDK NuGet package if SDK 1.39.0 is not published:

```powershell
dotnet pack src/Arclyra.PluginSdk/Arclyra.PluginSdk.csproj -c Release -o artifacts/nuget
dotnet restore SamplePlugins/Arclyra.SamplePlugin/Arclyra.SamplePlugin.csproj --source ./artifacts/nuget --source https://api.nuget.org/v3/index.json
dotnet build SamplePlugins/Arclyra.SamplePlugin/Arclyra.SamplePlugin.csproj -c Release --no-restore
```

`SamplePlugins/packaged/` contains historical, prebuilt signed/unsigned archives, **not** the current 1.6.4 source output. Leave those archives unchanged and sign newly built packages with your own developer identity; never reuse a historical signature for altered content.

## Safety and compatibility

The manifest includes capabilities for its numerous optional UI/data demos; it is a learning sample, not a recommended least-privilege manifest. Real extensions should remove unused capabilities and explain any retained sensitive access. SDK 1.34's `IPluginPromptReviewService` registration is first-party only: this ordinary sample intentionally does not access `FirstPartyUiService` or declare reserved trusted identity. Typed summary previews (host 86), working-revision identity reads (host 115), and Kitty help registration (host 150) are demonstrated in [SDK recipes](../Arclyra.SdkExamples/README.md). [Compatibility matrix](../../sdk-compatibility.md) covers feature minima.
