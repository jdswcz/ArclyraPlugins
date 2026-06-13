using System.Windows;
using System.Windows.Controls;
using Arclyra.PluginSdk;

namespace Arclyra.SamplePlugin;

public partial class SampleWorkspacePanelControl : UserControl
{
    private readonly IPluginContext _context;

    public SampleWorkspacePanelControl(IPluginContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        InitializeComponent();
        _context = context;
        RefreshSnapshot();
    }

    private void RefreshSnapshot_Click(object sender, RoutedEventArgs e)
    {
        RefreshSnapshot();
        _context.DialogService.ShowInfo("Arclyra Sample Plugin", "The sample workspace snapshot was refreshed.");
    }

    private void RefreshSnapshot()
    {
        SnapshotText.Text = SamplePlugin.GetReadOnlyDataSummary(_context);
    }
}
