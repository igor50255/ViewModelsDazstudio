using System.IO;

namespace ViewModelsDazstudio.Services
{
    public static class PreviewFixer
    {
        private static readonly HashSet<string> ImageExts = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".gif", ".tif", ".tiff", ".heic", ".avif"
    };

        public static List<string> BuildList(string rootDir, string criteria, string defaultImagePath = @"D:\default.jpg")
        {
            if (!Directory.Exists(rootDir))
                throw new DirectoryNotFoundException($"Root directory not found: {rootDir}");

            if (!File.Exists(defaultImagePath))
                throw new FileNotFoundException($"Default image not found: {defaultImagePath}", defaultImagePath);

            var result = new List<string>();

            // фильтруем директории согласно поиска: All, SASE, Other, A, IJ, LM и т.п.
            var filteredFolders = Directory.EnumerateDirectories(rootDir).Where(folder => Filter.FilterFolder(folder, criteria));

            foreach (var folder in filteredFolders)
            {
                // 1) preview folder must exist
                var previewDir = Path.Combine(folder, "preview");
                Directory.CreateDirectory(previewDir);

                // 2) ориентир ТОЛЬКО на preview\0.*
                var previewZero = Directory.EnumerateFiles(previewDir, "0.*", SearchOption.TopDirectoryOnly)
                                           .FirstOrDefault();

                if (!string.IsNullOrEmpty(previewZero))
                {
                    result.Add(previewZero);
                    continue;
                }

                // 3) если нет — ищем первую картинку в ОСНОВНОЙ папке (не в preview)
                var sourceImage = Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                                           .Where(f => ImageExts.Contains(Path.GetExtension(f)))
                                           .FirstOrDefault();

                if (string.IsNullOrEmpty(sourceImage))
                    sourceImage = defaultImagePath;

                var ext = Path.GetExtension(sourceImage);           // расширение источника
                var dest = Path.Combine(previewDir, "0" + ext);     // preview\0.<ext>

                // копируем (в preview нет 0.* по условию, но на всякий overwrite:true)
                File.Copy(sourceImage, dest, overwrite: true);

                result.Add(dest);
            }

            return result;
        }
    }
}
