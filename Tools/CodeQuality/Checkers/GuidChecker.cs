using System.Text.RegularExpressions;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public static class GuidChecker
{
    private static readonly Regex GuidRegex =
        new(@"^guid:\s*([a-f0-9]{32})$", RegexOptions.Multiline);

    private static readonly HashSet<string> IgnoredExtensions =
    [
        ".meta",
        ".cs",
        ".asmdef",
        ".csproj",
        ".sln",
        ".gitignore",
        ".md",
        ".txt",
        ".yml",
        ".yaml"
    ];

    public static CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("GUID");

        Dictionary<string, string> guids = new();

        // Vérification des GUID
        foreach (var file in FileScanner.GetFiles(rootDirectory, "*.meta"))
        {
            var path = file.Replace('\\', '/');

            var content = File.ReadAllText(file);

            var match = GuidRegex.Match(content);

            if (!match.Success)
            {
                result.Errors.Add(new CheckError(
                    path,
                    0,
                    "GUID manquant",
                    ""));

                continue;
            }

            var guid = match.Groups[1].Value;

            if (guids.TryGetValue(guid, out var existingFile))
            {
                result.Errors.Add(new CheckError(
                    path,
                    0,
                    $"GUID dupliqué (déjà présent dans {existingFile})",
                    guid));
            }
            else
            {
                guids.Add(guid, path);
            }
        }

        // Assets sans .meta
        foreach (var file in FileScanner.GetAllFiles(rootDirectory))
        {
            var path = file.Replace('\\', '/');
            var extension = Path.GetExtension(path);

            if (IgnoredExtensions.Contains(extension))
                continue;

            if (File.Exists(file + ".meta"))
                continue;

            result.Errors.Add(new CheckError(
                path,
                0,
                "Asset sans fichier .meta",
                ""));
        }

        // Meta orphelins
        foreach (var meta in FileScanner.GetFiles(rootDirectory, "*.meta"))
        {
            var asset = meta[..^5];

            if (File.Exists(asset) || Directory.Exists(asset))
                continue;

            result.Errors.Add(new CheckError(
                meta.Replace('\\', '/'),
                0,
                "Fichier .meta orphelin",
                ""));
        }

        return result;
    }
}