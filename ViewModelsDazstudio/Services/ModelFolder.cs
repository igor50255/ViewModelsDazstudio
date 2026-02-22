using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ViewModelsDazstudio.Services
{
    public class ModelFolder
    {
        // открытие папки с выбранной моделью в проводнике Windows
        public static void Open(string url, string rootContent, string hostGallery)
        {
            if (!url.StartsWith($"https://{hostGallery}/", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show($"Неожиданный URL: {url}");
                return;
            }
            var uri = new Uri(url);

            var rel = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'))
                         .Replace('/', Path.DirectorySeparatorChar);

            var rootPath = Path.GetFullPath(rootContent);
            var fullPath = Path.GetFullPath(Path.Combine(rootPath, rel));

            if (!fullPath.StartsWith(rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(fullPath, rootPath, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Неверный путь (выход за пределы root).");
                return;
            }

            if (Directory.Exists(fullPath))
            {
                Process.Start(new ProcessStartInfo { FileName = fullPath, UseShellExecute = true });
            }
            else
            {
                MessageBox.Show($"Папка не найдена: {fullPath}");
            }
        }
    }
}
