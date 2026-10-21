using System.Text.RegularExpressions;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public sealed class UnityReferenceChecker : IChecker
{
    private static readonly Regex GuidRegex =
        new(@"guid:\s*([a-f0-9]{32})");

    private static readonly string[] UnityExtensions =
    {
        ".unity",
        ".prefab",
        ".asset",
        ".mat",
        ".anim",
        ".controller",
        ".overrideController",
        ".playable"
    };

    public CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("Unity References");

        HashSet<string> existingGuids = new();

        // Récupération de tous les GUID des .meta
        foreach (var meta in FileScanner.GetFiles(rootDirectory, "*.meta"))
        {
            var content = File.ReadAllText(meta);

            var match = Regex.Match(
                content,
                @"^guid:\s*([a-f0-9]{32}),\s*type:\s*(\d)$",
                RegexOptions.Multiline);

            var type = match.Groups[2].Value;

            if (type == "3")
                continue;

            if (match.Success)
                existingGuids.Add(match.Groups[1].Value);
        }

        // Analyse des fichiers Unity
        foreach (var file in FileScanner.GetAllFiles(rootDirectory))
        {
            var extension = Path.GetExtension(file);

            if (!UnityExtensions.Contains(extension))
                continue;

            var lines = File.ReadAllLines(file);

            for (int i = 0; i < lines.Length; i++)
            {
                foreach (Match match in GuidRegex.Matches(lines[i]))
                {
                    var guid = match.Groups[1].Value;

                    if (existingGuids.Contains(guid))
                        continue;

                    result.Errors.Add(new CheckError(
                        file.Replace('\\', '/'),
                        i + 1,
                        "GUID référencé introuvable",
                        lines[i].Trim()));
                }
            }
        }

        return result;
    }
}