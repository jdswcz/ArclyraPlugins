using System.Windows;
using System.Windows.Controls;
using Arclyra.PluginSdk;

namespace Arclyra.SamplePlugin;

public partial class SampleSettingsControl : UserControl
{
    private readonly IPluginContext _context;

    public SampleSettingsControl(IPluginContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        InitializeComponent();
        _context = context;
        HostDetailsText.Text = $"Host version: {_context.HostVersion}\nPlugin directory: {_context.PluginDirectory}";
        RefreshStatus();
    }

    private void RegisterDemoProvider_Click(object sender, RoutedEventArgs e)
    {
        string message = SamplePlugin.RegisterDemoAiProvider(_context);
        _context.DialogService.ShowInfo("Arclyra Sample Plugin", message);
        RefreshStatus();
    }

    private void RemoveDemoProvider_Click(object sender, RoutedEventArgs e)
    {
        string message = SamplePlugin.RemoveDemoAiProvider(_context);
        _context.DialogService.ShowInfo("Arclyra Sample Plugin", message);
        RefreshStatus();
    }

    private void RefreshStatus()
    {
        AiProviderStatusText.Text = SamplePlugin.GetDemoProviderStatus(_context);
        DataSnapshotText.Text = SamplePlugin.GetReadOnlyDataSummary(_context);
    }
}
