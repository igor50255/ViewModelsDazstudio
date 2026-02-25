using System.Diagnostics;
using System.Windows;

namespace ViewModelsDazstudio.Services
{
    public class Restart
    {
        public static void RestartApplication()
        {
            var exe = Process.GetCurrentProcess().MainModule!.FileName!;

            var psi = new ProcessStartInfo(exe, "--restart")
            {
                UseShellExecute = true
            };

            Process.Start(psi);
            Application.Current.Shutdown();
        }
    }
}
