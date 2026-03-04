using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace ViewModelsDazstudio.Services
{
    public class Optimization
    {
        // удалить все папки preview с содержимым внутри 
        public static void Delete()
        {
            //string rootPath = @"C:\RootDirectory"; // корневая директория
            string rootPath = Properties.Settings.Default.rootFolderPath; // корневая директория

            var previewDirs = Directory.EnumerateDirectories(
                rootPath,
                "preview",
                SearchOption.AllDirectories
            );

            foreach (var dir in previewDirs)
            {
                try
                {
                    Directory.Delete(dir, true); // true = удалить с содержимым
                    Console.WriteLine($"Удалено: {dir}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при удалении {dir}: {ex.Message}");
                }
            }
        }

        // --------------------------------------------------------------------------------------------


        // запуск конвертации в jpg
        public static void Convert(int hieght)
        {
           string rootPath = Properties.Settings.Default.rootFolderPath; // корневая директория

            var previewDirs = Directory.EnumerateDirectories(
                rootPath,
                "preview",
                SearchOption.AllDirectories
            );

            foreach (var previewDir in previewDirs)
            {
                try
                {
                    ConvertPreviewImage(previewDir, hieght);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка в {previewDir}: {ex.Message}");
                }
            }
        }

        // конвертация в jpg и уменьшение картинки до heigt = 250
        static void ConvertPreviewImage(string previewDir, int heightImg)
        {
            string[] imageExts = { ".jpg", ".jpeg", ".png", ".webp" };

            var imagePath = Directory.EnumerateFiles(previewDir)
                .FirstOrDefault(f =>
                    imageExts.Contains(
                        Path.GetExtension(f),
                        StringComparer.OrdinalIgnoreCase));

            if (imagePath == null)
                return;

            string ext = Path.GetExtension(imagePath).ToLowerInvariant();

            bool isJpg = ext == ".jpg" || ext == ".jpeg";

            string outPath = isJpg
                ? imagePath
                : Path.Combine(
                    previewDir,
                    Path.GetFileNameWithoutExtension(imagePath) + ".jpg"
                );

            using var image = Image.Load(imagePath);

            // уменьшаем только если высота больше 250 (heightImg)
            if (image.Height > heightImg)
            {
                int newWidth = image.Width * heightImg / image.Height;

                image.Mutate(x =>
                    x.Resize(newWidth, heightImg)
                );
            }

            image.Save(outPath, new JpegEncoder { Quality = 90 });

            // если был не JPG — можно удалить оригинал
             if (!isJpg)
                File.Delete(imagePath);

            Console.WriteLine($"Обработано: {outPath}");
        }

        // -----------------------------------------------------------------------------------------------


        // запуск восстановления превью (копирование первой картинки из родительской папки в preview\0.*)
        public static void Refresh()
        {
            string rootPath = Properties.Settings.Default.rootFolderPath; // корневая директория

            var previewDirs = Directory.EnumerateDirectories(
                rootPath,
                "preview",
                SearchOption.AllDirectories
            );

            foreach (var previewDir in previewDirs)
            {
                try
                {
                    RefreshPreview(previewDir);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка в {previewDir}: {ex.Message}");
                }
            }
        }

        // восстановление первоначального превью (копирование первой картинки из родительской папки в preview\0.*)
        static void RefreshPreview(string previewDir)
        {
            // 1) Очистить preview (удалить файлы; папку не удаляем)
            foreach (var file in Directory.EnumerateFiles(previewDir, "*", SearchOption.TopDirectoryOnly))
            {
                File.Delete(file);
            }

            // (опционально) если внутри preview могут быть подпапки и их тоже надо чистить:
            foreach (var dir in Directory.EnumerateDirectories(previewDir, "*", SearchOption.TopDirectoryOnly))
            {
                Directory.Delete(dir, true);
            }

            // 2) Найти "соседнюю" картинку (в родительской папке preview)
            var parentDir = Directory.GetParent(previewDir)?.FullName;
            if (string.IsNullOrWhiteSpace(parentDir) || !Directory.Exists(parentDir))
                return;

            string[] exts = { ".jpg", ".jpeg", ".png", ".webp" };

            // Берём первую картинку из родительской папки (НЕ рекурсивно)
            var sourceImage = Directory.EnumerateFiles(parentDir, "*.*", SearchOption.TopDirectoryOnly)
                .Where(p => exts.Contains(Path.GetExtension(p), StringComparer.OrdinalIgnoreCase))
                .OrderBy(p => p, StringComparer.OrdinalIgnoreCase) // чтобы "первая" была стабильной
                .FirstOrDefault();

            if (sourceImage == null)
            {
                Console.WriteLine($"Нет картинок рядом с preview: {parentDir}");
                return;
            }

            // 3) Копируем в preview как 0.<ext>
            string ext = Path.GetExtension(sourceImage); // с точкой
            string destPath = Path.Combine(previewDir, "0" + ext.ToLowerInvariant());

            File.Copy(sourceImage, destPath, overwrite: true);

            Console.WriteLine($"Скопировано: {sourceImage} -> {destPath}");
        }
    }
}


