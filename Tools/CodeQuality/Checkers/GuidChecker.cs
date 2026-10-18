using System.Text.RegularExpressions;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public static class GuidChecker
{
    private static readonly Regex GuidRegex =
        new(@"^guid:\s*([a-f0-9]{32})$", RegexOptions.Multiline);

    public static CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("GUID");

        Dictionary<string, string> guids = new();

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
                    string.Empty));

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

        return result;
    }
}