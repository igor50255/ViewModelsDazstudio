using System.Configuration;
using System.Data;
using System.Windows;

namespace ViewModelsDazstudio;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (e.Args != null && e.Args.Contains("--restart"))
        {
            //MessageBox.Show("RESTART MODE");
        }
    }
}


