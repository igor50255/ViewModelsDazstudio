using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelsDazstudio.Services
{
    public static class GalleryMapper
    {
        public static List<object> BuildImageDtos(
            List<string> previewFiles,   // из PreviewFixer.BuildList(...)
            string hostGallery,
            string folder,
            long ver)
        {
            return previewFiles
                .Select(fullPath =>
                {
                    // Нормализуем разделители и режем на секции
                    var parts = fullPath
                        .Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                               StringSplitOptions.RemoveEmptyEntries);

                    // Последние 3 секции: <Character>\preview\0.jpg
                    if (parts.Length < 3)
                        throw new InvalidOperationException($"Путь слишком короткий: {fullPath}");

                    var last3 = parts.Skip(parts.Length - 3).ToArray();

                    // Третья секция с конца — имя папки персонажа/набора
                    var name = last3[0];

                    // urlName — это Name, но URL-encoded
                    var urlName = Uri.EscapeDataString(name);

                    // urlNameFile — 3 последние секции, но кодируем КАЖДУЮ секцию отдельно,
                    // а затем соединяем через "/" (чтобы слэши не кодировались)
                    var urlNameFile = string.Join("/", last3.Select(Uri.EscapeDataString));

                    return new
                    {
                        Name = name,
                        PathFolder = $"https://{hostGallery}/{folder}/{urlName}",
                        PathFile = $"https://{hostGallery}/{folder}/{urlNameFile}?v={ver}"
                    };
                })
                .Cast<object>()
                .ToList();
        }
    }
}
