using System.Text.RegularExpressions;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public sealed class UnityReferenceChecker : IChecker
{
    private static readonly Regex GuidRegex =
        new(@"guid:\s*([a-f0-9]{32})");

    private static readonly Regex TypeRegex =
        new(@"type:\s*(\d+)");

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

        // Récupère tous les GUID des assets du projet.
        foreach (var meta in FileScanner.GetFiles(rootDirectory, "*.meta"))
        {
            var content = File.ReadAllText(meta);

            var match = Regex.Match(
                content,
                @"^guid:\s*([a-f0-9]{32})$",
                RegexOptions.Multiline);

            if (match.Success)
                existingGuids.Add(match.Groups[1].Value);
        }

        foreach (var file in FileScanner.GetAllFiles(rootDirectory))
        {
            var normalized = file.Replace('\\', '/');

            // On ne vérifie que les assets du projet.
            if (!normalized.StartsWith("./Assets/") &&
                !normalized.StartsWith("Assets/"))
                continue;

            var extension = Path.GetExtension(file);

            if (!UnityExtensions.Contains(extension))
                continue;

            var lines = File.ReadAllLines(file);

            for (int i = 0; i < lines.Length; i++)
            {
                foreach (Match guidMatch in GuidRegex.Matches(lines[i]))
                {
                    var guid = guidMatch.Groups[1].Value;

                    // GUID internes Unity.
                    if (guid == "0000000000000000e000000000000000" ||
                        guid == "0000000000000000f000000000000000")
                        continue;

                    var typeMatch = TypeRegex.Match(lines[i]);

                    // On ne vérifie que les références vers des assets.
                    if (!typeMatch.Success ||
                        typeMatch.Groups[1].Value != "2")
                        continue;

                    if (existingGuids.Contains(guid))
                        continue;

                    result.Errors.Add(new CheckError(
                        normalized,
                        i + 1,
                        "GUID référencé introuvable",
                        lines[i].Trim()));
                }
            }
        }

        return result;
    }
}