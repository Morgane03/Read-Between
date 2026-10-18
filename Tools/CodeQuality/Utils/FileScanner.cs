using CodeQuality.Models;

namespace CodeQuality.Utils;

public static class FileScanner
{
    private static readonly string[] IgnoredFolders =
    {
        "/Library/",
        "/Logs/",
        "/Temp/",
        "/obj/",
        "/bin/",
        "/Packages/",
        "/ProjectSettings/",
        "/UserSettings/",
        "/Tools/CodeQuality/",
        "/Assets/Tests/"
    };

    public static IEnumerable<string> GetFiles(
        string rootDirectory,
        string searchPattern)
    {
        var files = Directory.GetFiles(
            rootDirectory,
            searchPattern,
            SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var path = file.Replace('\\', '/');

            bool ignored = IgnoredFolders.Any(folder => path.Contains(folder));

            if (!ignored)
                yield return file;
        }
    }

    public static IEnumerable<SourceLine> ReadLines(
        string rootDirectory,
        string searchPattern)
    {
        foreach (var file in GetFiles(rootDirectory, searchPattern))
        {
            var path = file.Replace('\\', '/');

            var lines = File.ReadAllLines(file);

            for (int i = 0; i < lines.Length; i++)
            {
                yield return new SourceLine(
                    path,
                    i + 1,
                    lines[i]);
            }
        }
    }
}