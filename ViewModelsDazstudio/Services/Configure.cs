using Microsoft.Win32;
using System.Windows;

namespace ViewModelsDazstudio.Services
{
    public class Configure
    {
        public static string SetRootContent(string defaultRoot, Window window)
        {
            string folder = string.Empty;

            // Если путь уже установлен, пропускаем выбор папки
            if (Properties.Settings.Default.rootFolder == false)
            {
                try
                {
                    CreateTreeFolder.Create(defaultRoot);
                }
                catch
                {
                    string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    defaultRoot = System.IO.Path.Combine(userProfile, "ContentDazStudio");
                    CreateTreeFolder.Create(defaultRoot);
                }

                var dlg = new OpenFolderDialog
                {
                    Title = "Выберите папку для размещения контента",
                    InitialDirectory = defaultRoot,  // стартовая папка
                    Multiselect = false,
                };

                bool? result = dlg.ShowDialog(window); // или ShowDialog(ownerWindow)

                if (result == true)
                {
                    folder = dlg.FolderName; // полный путь к выбранной папке

                    CreateTreeFolder.Create(defaultRoot);

                    Properties.Settings.Default.rootFolder = true;
                    Properties.Settings.Default.rootFolderPath = folder;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                folder = Properties.Settings.Default.rootFolderPath;
                CreateTreeFolder.Create(folder);
            }
            
            //MessageBox.Show(folder);

            return folder;
        }
    }
}
