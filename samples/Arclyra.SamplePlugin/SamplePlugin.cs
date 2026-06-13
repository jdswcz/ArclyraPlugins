using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Arclyra.PluginSdk;

namespace Arclyra.SamplePlugin;

/// <summary>
/// Demonstrates the finalized Arclyra plugin SDK extension points with demo-only behavior.
/// </summary>
public sealed class SamplePlugin : IArclyraPlugin
{
    internal const string DemoAiProviderConfigurationId = "sample.demo-ai-provider";

    private const string ShowCapabilitiesCommandId = "com.arclyra.plugins.sample.showCapabilities";
    private const string WriteDiagnosticsCommandId = "com.arclyra.plugins.sample.writeDiagnostics";
    private const string RegisterDemoAiProviderCommandId = "com.arclyra.plugins.sample.registerDemoAiProvider";
    private const string RemoveDemoAiProviderCommandId = "com.arclyra.plugins.sample.removeDemoAiProvider";
    private const string SmartBuilderToolbarActionId = "com.arclyra.plugins.sample.smartBuilder.showPromptSummary";
    private const string SmartBuilderSidebarPanelId = "com.arclyra.plugins.sample.smartBuilder.sidebar";
    private const string SmartBuilderBelowPromptContentId = "com.arclyra.plugins.sample.smartBuilder.belowPrompt";
    private const string SmartBuilderRowActionId = "com.arclyra.plugins.sample.smartBuilder.inspectRow";
    private const string SmartBuilderValidatorId = "com.arclyra.plugins.sample.smartBuilder.validator";
    private const string SmartBuilderTransformId = "com.arclyra.plugins.sample.smartBuilder.transform";
    private const string StoryOverviewPanelId = "com.arclyra.plugins.sample.storyOverview.panel";
    private const string NewStoryWizardStepId = "com.arclyra.plugins.sample.newStoryWizard.step";
    private const string EntryManagementPanelId = "com.arclyra.plugins.sample.entryManagement.crud";
    private const string StoryListActionButtonId = "com.arclyra.plugins.sample.storyList.summary";
    private const string RuntimePromptDetailPrefix = "com.arclyra.plugins.sample.runtimePromptDetail";

    private IPluginContext? _context;

    /// <inheritdoc />
    public string Id => "com.arclyra.plugins.sample";

    /// <inheritdoc />
    public string Name => "Arclyra Sample Plugin";

    /// <inheritdoc />
    public Task InitializeAsync(IPluginContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        _context = context;
        context.Logger.LogInformation("Arclyra Sample Plugin initialized with demo-only extension points.");

        RegisterCommands(context);
        RegisterMenuItems(context);
        RegisterSettingsPage(context);
        RegisterPanel(context);
        RegisterSmartBuilderExtensions(context);
        RegisterStoryOverviewExtensions(context);
        RegisterStoryListActions(context);
        RegisterNewStoryWizardExtensions(context);
        RegisterRuntimePromptDetails(context);
        RegisterEntryManagementExtensions(context);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ShutdownAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _context?.Logger.LogInformation("Arclyra Sample Plugin shutting down.");
        _context = null;
        return Task.CompletedTask;
    }

    internal static PluginAiConfiguration CreateDemoAiProviderConfiguration()
    {
        return new PluginAiConfiguration(
            Id: string.Empty,
            Name: "Sample Plugin Provider",
            Url: "about:blank",
            PromptPasteScript: "console.info('Arclyra sample plugin demo provider: prompt paste script invoked. No external service is contacted.');",
            ReadGeneratedChapterScript: "return 'Demo-only sample provider. Replace this provider with a real plugin-owned configuration before using AI generation.';",
            PluginConfigurationId: DemoAiProviderConfigurationId);
    }

    internal static string GetDemoProviderStatus(IPluginContext context)
    {
        try
        {
            PluginAiConfiguration? configuration = context.AiProviderService
                .GetConfigurations()
                .FirstOrDefault(item => string.Equals(
                    item.PluginConfigurationId,
                    DemoAiProviderConfigurationId,
                    StringComparison.OrdinalIgnoreCase));

            return configuration == null
                ? "Demo provider is not registered. Use the sample command or settings button to add a harmless about:blank provider owned by this plugin."
                : $"Demo provider is registered as '{configuration.Name}' with host id '{configuration.Id}'. It is safe demo content and does not contact an AI service.";
        }
        catch (UnauthorizedAccessException exception)
        {
            return $"AI provider access is not permitted for this plugin manifest: {exception.Message}";
        }
        catch (InvalidOperationException exception)
        {
            return $"AI provider data is not currently available: {exception.Message}";
        }
    }

    internal static string GetReadOnlyDataSummary(IPluginContext context)
    {
        List<string> lines = new()
        {
            "Demo-only read-only data access:"
        };

        AddStorySummary(context, lines);
        AddPromptSummary(context, lines);
        AddSmartBuilderSummary(context, lines);
        AddAiProviderSummary(context, lines);

        return string.Join(Environment.NewLine, lines);
    }

    internal static string RegisterDemoAiProvider(IPluginContext context)
    {
        try
        {
            PluginAiConfiguration savedConfiguration = context.AiProviderService.AddOrUpdateConfiguration(CreateDemoAiProviderConfiguration());
            string message = "Registered a demo-only about:blank AI provider owned by the sample plugin. It is intentionally harmless and should be replaced by real plugin code before production use.";
            context.Logger.LogInformation($"Registered demo-only AI provider '{savedConfiguration.Name}' ({savedConfiguration.Id}).");
            return message;
        }
        catch (UnauthorizedAccessException exception)
        {
            string message = $"AI provider registration is not permitted for this plugin manifest: {exception.Message}";
            context.Logger.LogWarning(message);
            return message;
        }
        catch (InvalidOperationException exception)
        {
            string message = $"AI provider registration is unavailable: {exception.Message}";
            context.Logger.LogWarning(message);
            return message;
        }
    }

    internal static string RemoveDemoAiProvider(IPluginContext context)
    {
        try
        {
            bool removed = context.AiProviderService.RemovePluginConfiguration(DemoAiProviderConfigurationId);
            string message = removed
                ? "Removed the sample plugin demo-only AI provider."
                : "The sample plugin demo-only AI provider was not installed.";
            context.Logger.LogInformation(message);
            return message;
        }
        catch (UnauthorizedAccessException exception)
        {
            string message = $"AI provider removal is not permitted for this plugin manifest: {exception.Message}";
            context.Logger.LogWarning(message);
            return message;
        }
        catch (InvalidOperationException exception)
        {
            string message = $"AI provider removal is unavailable: {exception.Message}";
            context.Logger.LogWarning(message);
            return message;
        }
    }

    private static void RegisterCommands(IPluginContext context)
    {
        context.CommandRegistry.RegisterCommand(new PluginCommandRegistration(
            ShowCapabilitiesCommandId,
            "Show Sample Plugin Capabilities",
            _ =>
            {
                context.DialogService.ShowInfo(
                    "Arclyra Sample Plugin",
                    "This demo-only plugin contributes:\n\n" +
                    "• Menu commands\n" +
                    "• A settings page\n" +
                    "• A workspace panel\n" +
                    "• Smart Builder toolbar, row, sidebar, below-prompt, validator, and transform hooks\n" +
                    "• Story overview title/synopsis editing through host-mediated UI context\n" +
                    "• A story-list row action button with the clicked story context\n" +
                    "• New-story wizard draft field reading and editing through host-mediated UI context\n" +
                    "• Entry Management UI customization with prompt detail CRUD buttons\n" +
                    "• Optional plugin-owned AI provider registration\n" +
                    "• Read-only story/prompt/provider snapshots when permitted\n\n" +
                    GetReadOnlyDataSummary(context));
                return Task.CompletedTask;
            },
            "Shows a dialog summarizing the finalized SDK surfaces used by the sample plugin."));

        context.CommandRegistry.RegisterCommand(new PluginCommandRegistration(
            WriteDiagnosticsCommandId,
            "Write Sample Plugin Diagnostics",
            _ =>
            {
                context.Logger.LogInformation($"Sample plugin directory: {context.PluginDirectory}");
                context.Logger.LogInformation($"Arclyra host version: {context.HostVersion}");
                context.Logger.LogInformation(GetReadOnlyDataSummary(context));
                context.Logger.LogInformation(GetDemoProviderStatus(context));
                context.Logger.LogInformation("Smart Builder demo hooks plus story overview and new-story wizard demo UI hooks are registered.");
                context.Logger.LogWarning("Demo-only warning log entry from the Arclyra Sample Plugin.");
                return Task.CompletedTask;
            },
            "Writes demo-only information and warning messages to the plugin log."));

        context.CommandRegistry.RegisterCommand(new PluginCommandRegistration(
            RegisterDemoAiProviderCommandId,
            "Register Demo AI Provider",
            _ =>
            {
                string message = RegisterDemoAiProvider(context);
                context.DialogService.ShowInfo(
                    "Arclyra Sample Plugin",
                    message);
                return Task.CompletedTask;
            },
            "Adds or updates the sample plugin's harmless demo AI provider configuration."));

        context.CommandRegistry.RegisterCommand(new PluginCommandRegistration(
            RemoveDemoAiProviderCommandId,
            "Remove Demo AI Provider",
            _ =>
            {
                string message = RemoveDemoAiProvider(context);
                context.DialogService.ShowInfo(
                    "Arclyra Sample Plugin",
                    message);
                return Task.CompletedTask;
            },
            "Removes the sample plugin's demo AI provider configuration if it exists."));
    }



    private static void RegisterEntryManagementExtensions(IPluginContext context)
    {
        try
        {
            context.UiRegistry.RegisterEntryManagementPanel(new PluginEntryManagementPanelRegistration(
                EntryManagementPanelId,
                "Sample prompt-detail CRUD",
                entryContext => CreateEntryManagementCrudPanel(context, entryContext),
                "Demonstrates host-mediated create, read, update, and delete access for prompt details.",
                SortOrder: 10));
        }
        catch (UnauthorizedAccessException exception)
        {
            context.Logger.LogWarning($"Entry Management UI demo registration is not permitted for this plugin manifest: {exception.Message}");
        }
    }

    private static FrameworkElement CreateEntryManagementCrudPanel(IPluginContext context, PluginEntryManagementPanelContext entryContext)
    {
        StackPanel panel = new() { Margin = new Thickness(0, 0, 0, 4) };
        TextBlock summary = new()
        {
            Text = entryContext.SelectedStory == null
                ? "Select a story scope to run CRUD operations against real prompt details."
                : $"Story: {entryContext.SelectedStory.Name} • {entryContext.PromptDetails.Count} prompt detail(s) currently visible to plugins.",
            TextWrapping = TextWrapping.Wrap,
            Foreground = Brushes.DimGray,
            Margin = new Thickness(0, 0, 0, 6)
        };
        panel.Children.Add(summary);

        WrapPanel buttons = new();
        panel.Children.Add(buttons);

        buttons.Children.Add(CreateCrudButton("Create", () =>
        {
            if (entryContext.SelectedStory == null)
                return "Select a story scope first.";

            PluginPromptDetailDto created = context.PromptContextService.CreatePromptDetail(
                entryContext.SelectedStory.Id,
                new PluginPromptDetailCreateRequest(
                    entryContext.ActiveEntryType,
                    $"Sample plugin-created prompt detail at {DateTime.Now:t}.",
                    "Created by Sample Plugin",
                    PluginSelectionScope.Story));
            return $"Created prompt detail {created.Id}. Refresh Entry Management to see the persisted host entry.";
        }));

        buttons.Children.Add(CreateCrudButton("Read", () =>
        {
            if (entryContext.SelectedStory == null)
                return "Select a story scope first.";

            IReadOnlyList<PluginPromptDetailDto> details = context.PromptContextService.GetPromptDetails(entryContext.SelectedStory.Id);
            PluginPromptDetailDto? first = details.FirstOrDefault();
            return first == null
                ? "No prompt details are available for this story yet."
                : $"Read {details.Count} prompt detail(s). First: {first.EntryType} / {first.Detail ?? first.Content}";
        }));

        buttons.Children.Add(CreateCrudButton("Update", () =>
        {
            if (entryContext.SelectedStory == null)
                return "Select a story scope first.";

            PluginPromptDetailDto? target = context.PromptContextService.GetPromptDetails(entryContext.SelectedStory.Id)
                .FirstOrDefault(detail => !detail.Id.Contains(':', StringComparison.Ordinal));
            if (target == null)
                return "No persisted host prompt detail was available to update. Create one first.";

            PluginPromptDetailDto updated = context.PromptContextService.UpdatePromptDetail(
                entryContext.SelectedStory.Id,
                target.Id,
                new PluginPromptDetailUpdateRequest(
                    Detail: "Updated by Sample Plugin",
                    Content: $"{target.Content}\n\n[Sample plugin update at {DateTime.Now:t}]"));
            return $"Updated prompt detail {updated.Id}.";
        }));

        buttons.Children.Add(CreateCrudButton("Delete", () =>
        {
            if (entryContext.SelectedStory == null)
                return "Select a story scope first.";

            PluginPromptDetailDto? target = context.PromptContextService.GetPromptDetails(entryContext.SelectedStory.Id)
                .LastOrDefault(detail => string.Equals(detail.Detail, "Created by Sample Plugin", StringComparison.Ordinal));
            if (target == null)
                return "No sample-created prompt detail was found to delete.";

            bool deleted = context.PromptContextService.DeletePromptDetail(entryContext.SelectedStory.Id, target.Id);
            return deleted ? $"Deleted sample prompt detail {target.Id}." : "The sample prompt detail was already removed.";
        }));

        return panel;
    }

    private static Button CreateCrudButton(string label, Func<string> action)
    {
        Button button = new()
        {
            Content = label,
            Margin = new Thickness(0, 0, 6, 6),
            Padding = new Thickness(10, 4, 10, 4),
            MinWidth = 64
        };
        button.Click += (_, _) => MessageBox.Show(action(), "Arclyra Sample Plugin", MessageBoxButton.OK, MessageBoxImage.Information);
        return button;
    }

    private static void RegisterRuntimePromptDetails(IPluginContext context)
    {
        try
        {
            foreach (PluginStoryDto story in context.StoryDataService.GetStories())
            {
                context.PromptContextService.AddOrUpdateRuntimePromptDetail(
                    story.Id,
                    new PluginPromptDetailCreateRequest(
                        PluginGenerationEntryType.Instruction,
                        "Sample plugin guidance: make every scene goal, conflict, and outcome clear before moving to the next beat.",
                        "Sample Plugin Writing Guidance",
                        PluginSelectionScope.Global),
                    $"{RuntimePromptDetailPrefix}.guidance.{story.Id}");

                context.PromptContextService.AddOrUpdateRuntimePromptDetail(
                    story.Id,
                    new PluginPromptDetailCreateRequest(
                        PluginGenerationEntryType.StoryInformation,
                        "Sample plugin world detail: rumors spread through courier moths that glow brighter when a message is urgent.",
                        "Sample Plugin World Detail",
                        PluginSelectionScope.Global),
                    $"{RuntimePromptDetailPrefix}.world.{story.Id}");

                context.PromptContextService.AddOrUpdateRuntimePromptDetail(
                    story.Id,
                    new PluginPromptDetailCreateRequest(
                        PluginGenerationEntryType.Custom,
                        "Sample Plugin Scene Checklist",
                        "Open with a concrete sensory detail; include one character choice; end with a changed situation.",
                        PluginSelectionScope.Global),
                    $"{RuntimePromptDetailPrefix}.checklist.{story.Id}");
            }

            context.Logger.LogInformation("Registered sample runtime prompt details for Smart Builder add-details search.");
        }
        catch (UnauthorizedAccessException exception)
        {
            context.Logger.LogWarning($"Runtime prompt detail demo registration is not permitted for this plugin manifest: {exception.Message}");
        }
        catch (InvalidOperationException exception)
        {
            context.Logger.LogWarning($"Runtime prompt detail demo registration is unavailable: {exception.Message}");
        }
    }

    private static void RegisterSmartBuilderExtensions(IPluginContext context)
    {
        try
        {
            context.SmartBuilderService.RegisterToolbarAction(new PluginSmartBuilderToolbarRegistration(
                SmartBuilderToolbarActionId,
                "Sample Prompt Summary",
                actionContext =>
                {
                    context.DialogService.ShowInfo(
                        "Sample Smart Builder Toolbar Action",
                        BuildPromptSummary(actionContext.Prompt));
                    return Task.CompletedTask;
                },
                SortOrder: 10,
                ToolTip: "Shows a sample plugin summary of the current Smart Builder prompt."));

            context.SmartBuilderService.RegisterSidebarPanel(new PluginSmartBuilderSidebarRegistration(
                SmartBuilderSidebarPanelId,
                "Sample Smart Builder Sidebar",
                () => CreateSmartBuilderDemoCard(
                    "Sample Smart Builder Sidebar",
                    "Plugins can add contextual Smart Builder panels beside prompt review surfaces. Use this area for guidance, checklists, or analysis tied to your own plugin services."),
                SortOrder: 10,
                Description: "Demonstrates a plugin-owned Smart Builder sidebar panel."));

            context.SmartBuilderService.RegisterBelowPromptContent(new PluginSmartBuilderBelowPromptContentRegistration(
                SmartBuilderBelowPromptContentId,
                "Sample Below-Prompt Content",
                () => CreateSmartBuilderDemoCard(
                    "Sample Below-Prompt Content",
                    "Plugins can place read-only or interactive content below rendered prompt rows. This sample keeps the UI safe and demo-only."),
                SortOrder: 10,
                Description: "Demonstrates content hosted below Smart Builder prompt rows."));

            context.SmartBuilderService.RegisterPromptRowAction(new PluginPromptRowActionRegistration(
                SmartBuilderRowActionId,
                "Inspect Row",
                rowContext =>
                {
                    context.DialogService.ShowInfo(
                        "Sample Smart Builder Row Action",
                        BuildRowSummary(rowContext.Row));
                    return Task.CompletedTask;
                },
                SortOrder: 10,
                ToolTip: "Shows sample plugin details for this rendered prompt row."));

            context.SmartBuilderService.RegisterPromptValidator(new PluginPromptValidatorRegistration(
                SmartBuilderValidatorId,
                "Sample Prompt Validator",
                ValidateSmartBuilderPrompt,
                SortOrder: 10));

            context.SmartBuilderService.RegisterPromptTransform(new PluginPromptTransformRegistration(
                SmartBuilderTransformId,
                "Sample Demo Footer Transform",
                TransformSmartBuilderPrompt,
                SortOrder: 10));
        }
        catch (UnauthorizedAccessException exception)
        {
            context.Logger.LogWarning($"Smart Builder demo registration is not permitted for this plugin manifest: {exception.Message}");
        }
        catch (InvalidOperationException exception)
        {
            context.Logger.LogWarning($"Smart Builder demo registration is unavailable: {exception.Message}");
        }
    }


    private static void RegisterStoryOverviewExtensions(IPluginContext context)
    {
        try
        {
            context.UiRegistry.RegisterStoryOverviewPanel(new PluginStoryOverviewPanelRegistration(
                StoryOverviewPanelId,
                "Sample Story Overview Tools",
                overviewContext => CreateStoryOverviewDemoCard(context, overviewContext),
                Description: "Demonstrates host-mediated story title and synopsis changes.",
                SortOrder: 10));
        }
        catch (UnauthorizedAccessException exception)
        {
            context.Logger.LogWarning($"Story overview demo registration is not permitted for this plugin manifest: {exception.Message}");
        }
        catch (InvalidOperationException exception)
        {
            context.Logger.LogWarning($"Story overview demo registration is unavailable: {exception.Message}");
        }
    }


    private static void RegisterStoryListActions(IPluginContext context)
    {
        try
        {
            context.UiRegistry.RegisterStoryListItemButton(new PluginStoryListItemButtonRegistration(
                StoryListActionButtonId,
                "Sample",
                buttonContext =>
                {
                    PluginStoryDto story = buttonContext.Story;
                    context.DialogService.ShowInfo(
                        "Sample Story List Action",
                        $"The sample plugin received story-list context for '{story.Name}'.\n\n" +
                        $"Chapters: {story.Chapters.Count}\n" +
                        $"Characters: {story.Characters.Count}\n" +
                        $"Story info entries: {story.StoryInformation.Count}");
                    return Task.CompletedTask;
                },
                ToolTip: "Runs a sample plugin action for this story.",
                SortOrder: 10));
        }
        catch (UnauthorizedAccessException exception)
        {
            context.Logger.LogWarning($"Story-list action demo registration is not permitted for this plugin manifest: {exception.Message}");
        }
        catch (InvalidOperationException exception)
        {
            context.Logger.LogWarning($"Story-list action demo registration is unavailable: {exception.Message}");
        }
    }

    private static void RegisterNewStoryWizardExtensions(IPluginContext context)
    {
        try
        {
            context.UiRegistry.RegisterNewStoryWizardStep(new PluginNewStoryWizardStepRegistration(
                NewStoryWizardStepId,
                "Sample Plugin Draft Tweaks",
                wizardContext => CreateNewStoryWizardDemoCard(wizardContext),
                ValidateAsync: wizardContext =>
                {
                    PluginNewStoryWizardDraftDto draft = wizardContext.DraftEditor?.GetDraft() ?? wizardContext.Draft;
                    return Task.FromResult(string.IsNullOrWhiteSpace(draft.Title)
                        ? PluginWizardStepValidationResult.Failure("The sample plugin can read the draft and noticed the title is empty.")
                        : PluginWizardStepValidationResult.Success);
                },
                ApplyAsync: completionContext =>
                {
                    PluginNewStoryWizardDraftDto draft = completionContext.DraftEditor?.GetDraft() ?? completionContext.Draft;
                    completionContext.Contributions.AddWritingInstruction($"Sample plugin saw this new-story draft title during completion: {draft.Title}");
                    return Task.CompletedTask;
                },
                Description: "Demonstrates reading and changing built-in wizard fields before the story is created.",
                SortOrder: 10,
                ShowAsStep: false));
        }
        catch (UnauthorizedAccessException exception)
        {
            context.Logger.LogWarning($"New-story wizard demo registration is not permitted for this plugin manifest: {exception.Message}");
        }
        catch (InvalidOperationException exception)
        {
            context.Logger.LogWarning($"New-story wizard demo registration is unavailable: {exception.Message}");
        }
    }

    private static FrameworkElement CreateStoryOverviewDemoCard(IPluginContext context, PluginStoryOverviewPanelContext overviewContext)
    {
        TextBox titleBox = new() { Text = overviewContext.StoryOverview?.Title ?? overviewContext.CurrentStory?.Name ?? string.Empty, Margin = new Thickness(0, 8, 0, 0) };
        TextBox synopsisBox = new()
        {
            Text = overviewContext.StoryOverview?.Synopsis ?? overviewContext.CurrentStory?.BaseStoryInfo ?? string.Empty,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 80,
            Margin = new Thickness(0, 8, 0, 0)
        };
        Button applyButton = new() { Content = "Apply sample overview edit", Margin = new Thickness(0, 10, 0, 0), HorizontalAlignment = HorizontalAlignment.Left };
        applyButton.Click += (_, _) =>
        {
            if (overviewContext.StoryOverview == null)
            {
                context.DialogService.ShowInfo("Arclyra Sample Plugin", "No editable story overview is currently available.");
                return;
            }

            overviewContext.StoryOverview.Title = titleBox.Text;
            overviewContext.StoryOverview.Synopsis = synopsisBox.Text;
            context.DialogService.ShowInfo("Arclyra Sample Plugin", "Updated the story title and synopsis through the plugin story overview context.");
        };

        return CreateFormCard("Sample Story Overview Tools", "Plugins can read and update the built-in story overview title and premise/synopsis through a host-mediated context.", titleBox, synopsisBox, applyButton);
    }

    private static FrameworkElement CreateNewStoryWizardDemoCard(PluginNewStoryWizardStepContext wizardContext)
    {
        return wizardContext.BuiltInStepId switch
        {
            "title" => CreateWizardSampleButton(
                "Sample title helper",
                "Insert a polished fantasy working title into the built-in title field.",
                "Use sample title",
                wizardContext,
                editor => editor.Title = "The Clockwork Orchard"),
            "premise" => CreateWizardSampleButton(
                "Sample premise helper",
                "Insert a concise premise into the built-in synopsis field.",
                "Use sample premise",
                wizardContext,
                editor => editor.Premise = "When a runaway apprentice discovers that the royal orchard grows mechanical fruit that can rewind a single minute, she must protect the harvest from a charming thief before the kingdom repeats its worst day forever."),
            "world" => CreateWizardSampleButton(
                "Sample world details helper",
                "Insert line-by-line setting notes into the built-in world details field.",
                "Use sample world details",
                wizardContext,
                editor => editor.WorldDetails = "Gaslamp fantasy kingdom with brass automata and living gardens.\nThe clockwork orchard blooms only under moonlight.\nTime-fruit can rewind one minute, but repeated use steals memories."),
            "goals" => CreateWizardSampleButton(
                "Sample writing goals helper",
                "Insert line-by-line style guidance into the built-in writing goals field.",
                "Use sample writing goals",
                wizardContext,
                editor => editor.WritingGoals = "Keep the tone wondrous, warm, and lightly mysterious.\nUse vivid sensory details for machinery, moonlight, and gardens.\nBalance adventure pacing with character-driven emotional stakes."),
            "character" => CreateWizardSampleButton(
                "Sample character helper",
                "Insert a main character into the built-in character fields.",
                "Use sample character",
                wizardContext,
                editor =>
                {
                    editor.CharacterName = "Mira Vale";
                    editor.CharacterAge = "19";
                    editor.CharacterDescription = "A stubborn apprentice horologist who fixes broken machines by listening to their rhythms, but secretly fears she cannot repair the mistake that cost her mentor's trust.";
                }),
            _ => CreateWizardSampleSummaryCard(wizardContext)
        };
    }

    private static FrameworkElement CreateWizardSampleButton(
        string title,
        string body,
        string buttonText,
        PluginNewStoryWizardStepContext wizardContext,
        Action<IPluginNewStoryWizardDraftEditor> applySample)
    {
        Button applyButton = new() { Content = buttonText, Margin = new Thickness(0, 10, 0, 0), HorizontalAlignment = HorizontalAlignment.Left };
        applyButton.Click += (_, _) =>
        {
            if (wizardContext.DraftEditor == null)
                return;

            applySample(wizardContext.DraftEditor);
        };

        return CreateFormCard(title, body, applyButton);
    }

    private static FrameworkElement CreateWizardSampleSummaryCard(PluginNewStoryWizardStepContext wizardContext)
    {
        PluginNewStoryWizardDraftDto draft = wizardContext.DraftEditor?.GetDraft() ?? wizardContext.Draft;
        return CreateFormCard(
            "Sample New-Story Wizard Helpers",
            $"This plugin now appears on each built-in wizard step and can write directly to that step's fields. Current draft title: {(string.IsNullOrWhiteSpace(draft.Title) ? "not set" : draft.Title)}.");
    }

    private static FrameworkElement CreateFormCard(string title, string body, params UIElement[] controls)
    {
        StackPanel panel = new();
        panel.Children.Add(new TextBlock { Text = title, FontSize = 16, FontWeight = FontWeights.SemiBold, Foreground = Brushes.White, TextWrapping = TextWrapping.Wrap });
        panel.Children.Add(new TextBlock { Text = body, Margin = new Thickness(0, 8, 0, 0), Foreground = Brushes.Gainsboro, TextWrapping = TextWrapping.Wrap });
        foreach (UIElement control in controls)
            panel.Children.Add(control);

        return new Border
        {
            Padding = new Thickness(14),
            Background = new SolidColorBrush(Color.FromRgb(36, 40, 52)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(95, 103, 128)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Child = panel
        };
    }

    private static FrameworkElement CreateSmartBuilderDemoCard(string title, string body)
    {
        return new Border
        {
            Padding = new Thickness(14),
            Background = new SolidColorBrush(Color.FromRgb(36, 40, 52)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(95, 103, 128)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Child = new StackPanel
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = title,
                        FontSize = 16,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = Brushes.White,
                        TextWrapping = TextWrapping.Wrap
                    },
                    new TextBlock
                    {
                        Text = body,
                        Margin = new Thickness(0, 8, 0, 0),
                        Foreground = Brushes.Gainsboro,
                        TextWrapping = TextWrapping.Wrap
                    },
                    new TextBlock
                    {
                        Text = "Also registered: a toolbar action, row action, prompt validator, and demo footer transform.",
                        Margin = new Thickness(0, 10, 0, 0),
                        Foreground = Brushes.LightGray,
                        TextWrapping = TextWrapping.Wrap
                    }
                }
            }
        };
    }

    private static IReadOnlyList<PluginPromptValidationMessage> ValidateSmartBuilderPrompt(PluginPromptTransformContext context)
    {
        List<PluginPromptValidationMessage> messages = new();

        if (string.IsNullOrWhiteSpace(context.Prompt.RenderedText))
        {
            messages.Add(new PluginPromptValidationMessage(
                PluginPromptValidationSeverity.Warning,
                "The rendered Smart Builder prompt is empty.",
                "SAMPLE_EMPTY_PROMPT"));
        }

        if (context.Prompt.Rows.Count == 0)
        {
            messages.Add(new PluginPromptValidationMessage(
                PluginPromptValidationSeverity.Information,
                "The sample plugin did not find any rendered prompt rows yet.",
                "SAMPLE_NO_ROWS"));
        }

        if (context.Prompt.RenderedText.Length > 12000)
        {
            messages.Add(new PluginPromptValidationMessage(
                PluginPromptValidationSeverity.Information,
                "The sample plugin noticed a large prompt. Real plugins could use this hook to flag provider token-budget concerns.",
                "SAMPLE_LARGE_PROMPT"));
        }

        return messages;
    }

    private static PluginPromptTransformResult TransformSmartBuilderPrompt(PluginPromptTransformContext context)
    {
        const string footer = "\n\n---\nSample Plugin Smart Builder Demo: this footer was appended by a prompt transform. Remove or disable the sample plugin before production generation.";

        if (context.Prompt.RenderedText.Contains("Sample Plugin Smart Builder Demo:", StringComparison.Ordinal))
            return new PluginPromptTransformResult(context.Prompt);

        PluginRenderedPromptDetailsDto transformedPrompt = context.Prompt with
        {
            RenderedText = string.Concat(context.Prompt.RenderedText, footer)
        };

        return new PluginPromptTransformResult(
            transformedPrompt,
            new[]
            {
                new PluginPromptValidationMessage(
                    PluginPromptValidationSeverity.Information,
                    "The sample plugin appended a demo footer through the Smart Builder prompt transform hook.",
                    "SAMPLE_DEMO_FOOTER")
            });
    }

    private static string BuildPromptSummary(PluginRenderedPromptDetailsDto prompt)
    {
        return $"Story: {prompt.StoryName}\n" +
            $"Chapter: {(prompt.ChapterNumber?.ToString() ?? "not selected")}\n" +
            $"Rows: {prompt.Rows.Count}\n" +
            $"Characters: {prompt.CharacterCount}; items: {prompt.ItemCount}; story info: {prompt.StoryInformationCount}; custom entries: {prompt.CustomCount}\n" +
            $"Rendered characters: {prompt.RenderedText.Length}\n\n" +
            "This dialog is shown by a sample plugin Smart Builder toolbar action.";
    }

    private static string BuildRowSummary(PluginRenderedPromptRowDto row)
    {
        return $"Source: {row.Source}\n" +
            $"Entry type: {row.EntryType?.ToString() ?? "none"}\n" +
            $"Title: {row.RenderedTitle ?? row.PlaceholderTitle ?? "none"}\n" +
            $"Placeholder: {row.TemplatePlaceholderToken ?? row.PlaceholderName ?? "none"}\n" +
            $"Chapter plot rows: {row.ChapterPlotRows.Count}\n" +
            $"Rendered characters: {row.RenderedText.Length}\n\n" +
            "This dialog is shown by a sample plugin Smart Builder row action.";
    }

    private static void RegisterMenuItems(IPluginContext context)
    {
        context.UiRegistry.RegisterMenuItem(new PluginMenuItemRegistration(
            "Tools/Plugins/Sample Plugin",
            "Show Demo Capabilities",
            ShowCapabilitiesCommandId,
            SortOrder: 10));

        context.UiRegistry.RegisterMenuItem(new PluginMenuItemRegistration(
            "Tools/Plugins/Sample Plugin",
            "Write Demo Diagnostics",
            WriteDiagnosticsCommandId,
            SortOrder: 20));

        context.UiRegistry.RegisterMenuItem(new PluginMenuItemRegistration(
            "Tools/Plugins/Sample Plugin",
            "Register Demo AI Provider",
            RegisterDemoAiProviderCommandId,
            SortOrder: 30));

        context.UiRegistry.RegisterMenuItem(new PluginMenuItemRegistration(
            "Tools/Plugins/Sample Plugin",
            "Remove Demo AI Provider",
            RemoveDemoAiProviderCommandId,
            SortOrder: 40));
    }

    private static void RegisterSettingsPage(IPluginContext context)
    {
        context.UiRegistry.RegisterSettingsPage(new PluginSettingsPageRegistration(
            "com.arclyra.plugins.sample.settings",
            "Sample Plugin Settings (Demo Only)",
            () => new SampleSettingsControl(context),
            SortOrder: 100));
    }

    private static void RegisterPanel(IPluginContext context)
    {
        context.UiRegistry.RegisterPanel(new PluginPanelRegistration(
            "com.arclyra.plugins.sample.panel",
            "Sample Plugin Panel (Demo Only)",
            () => new SampleWorkspacePanelControl(context),
            DefaultDockLocation: "Right"));
    }

    private static void AddStorySummary(IPluginContext context, ICollection<string> lines)
    {
        try
        {
            IReadOnlyList<PluginStoryDto> stories = context.StoryDataService.GetStories();
            IReadOnlyList<string> globalInstructions = context.StoryDataService.GetGlobalInstructions();
            IReadOnlyList<PluginCharacterDto> globalCharacters = context.StoryDataService.GetGlobalCharacters();
            IReadOnlyList<PluginItemDto> globalItems = context.StoryDataService.GetGlobalItems();
            lines.Add($"• Stories: {stories.Count}; global instructions: {globalInstructions.Count}; global characters: {globalCharacters.Count}; global items: {globalItems.Count}.");
        }
        catch (UnauthorizedAccessException exception)
        {
            lines.Add($"• Story data: not permitted ({exception.Message}).");
        }
        catch (InvalidOperationException exception)
        {
            lines.Add($"• Story data: unavailable ({exception.Message}).");
        }
    }

    private static void AddPromptSummary(IPluginContext context, ICollection<string> lines)
    {
        try
        {
            IReadOnlyList<PluginChapterPromptTemplateDto> templates = context.PromptContextService.GetChapterPromptTemplates();
            lines.Add($"• Prompt templates: {templates.Count}.");
        }
        catch (UnauthorizedAccessException exception)
        {
            lines.Add($"• Prompt context: not permitted ({exception.Message}).");
        }
        catch (InvalidOperationException exception)
        {
            lines.Add($"• Prompt context: unavailable ({exception.Message}).");
        }
    }

    private static void AddSmartBuilderSummary(IPluginContext context, ICollection<string> lines)
    {
        try
        {
            _ = context.SmartBuilderService;
            lines.Add("• Smart Builder: toolbar, sidebar, below-prompt content, row action, validator, and prompt transform hooks registered.");
        }
        catch (UnauthorizedAccessException exception)
        {
            lines.Add($"• Smart Builder: not permitted ({exception.Message}).");
        }
        catch (InvalidOperationException exception)
        {
            lines.Add($"• Smart Builder: unavailable ({exception.Message}).");
        }
    }

    private static void AddAiProviderSummary(IPluginContext context, ICollection<string> lines)
    {
        try
        {
            IReadOnlyList<PluginAiConfiguration> configurations = context.AiProviderService.GetConfigurations();
            int pluginOwnedCount = configurations.Count(configuration => configuration.IsPluginOwned);
            lines.Add($"• AI providers: {configurations.Count}; plugin-owned providers: {pluginOwnedCount}.");
        }
        catch (UnauthorizedAccessException exception)
        {
            lines.Add($"• AI providers: not permitted ({exception.Message}).");
        }
        catch (InvalidOperationException exception)
        {
            lines.Add($"• AI providers: unavailable ({exception.Message}).");
        }
    }
}
