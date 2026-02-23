using System.IO;

namespace ViewModelsDazstudio.Services
{
    public class Filter
    {
        public static bool FilterFolder(string folderPath, string criteria)
        {
            var name = Path.GetFileName(folderPath);

            if (criteria == "All")
                return true;

            if (criteria == "SASE")
                return name.StartsWith("SASE", StringComparison.OrdinalIgnoreCase);

            if (criteria == "Other")
            {
                if (name.StartsWith("SASE", StringComparison.OrdinalIgnoreCase))
                    return false;

                return !char.IsLetter(name[0]);
            }

            // набор букв (A, IJ, LM ...)
            if (criteria.Length > 1)
            {
                return criteria
                    .Any(c => name.StartsWith(c.ToString(), StringComparison.OrdinalIgnoreCase));
            }

            // одиночная буква + исключение SASE
            return name.StartsWith(criteria, StringComparison.OrdinalIgnoreCase) &&
                   !(criteria == "S" &&
                     name.StartsWith("SASE", StringComparison.OrdinalIgnoreCase));
        }
    }
}
