using System.IO;

namespace ViewModelsDazstudio.Services
{
    public class CreateTreeFolder
    {
        public static void Create(string defaultRoot)
        {
            string[] roots =
            {
                "Genesis 9",
                "Genesis 8 - 8.1",
                "Genesis 3",
                "Genesis 2",
                "Victoria 4",
                "Other"
    };

            string[] subFolders =
            {
                "Female",
                "Male",
                "Clothes",
                "Base",
                "Hair",
                "Poses",
                "Morf",
                "Props",
                "Animals",
                "Locations",
                "Other"
    };

            // (опционально) создаём сам defaultRoot, если его ещё нет
            Directory.CreateDirectory(defaultRoot);

            foreach (var root in roots)
            {
                var rootPath = Path.Combine(defaultRoot, root);
                Directory.CreateDirectory(rootPath);

                foreach (var sub in subFolders)
                {
                    Directory.CreateDirectory(Path.Combine(rootPath, sub));
                }
            }
        }
    }
}
