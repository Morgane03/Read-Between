using System.Text.RegularExpressions;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public sealed class SinglePublicTypeChecker : IChecker
{
    private static readonly Regex PublicTypeRegex =
        new(@"^\s*public\s+(?:sealed\s+|abstract\s+)?(?:class|struct|interface|enum)\s+[A-Za-z_][A-Za-z0-9_]*",
            RegexOptions.Multiline);

    public CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("Single Public Type");

        foreach (var file in FileScanner.GetFiles(rootDirectory, "*.cs"))
        {
            var text = File.ReadAllText(file);

            var matches = PublicTypeRegex.Matches(text);

            if (matches.Count <= 1)
                continue;

            result.Errors.Add(new CheckError(
                file.Replace('\\', '/'),
                1,
                "Le fichier contient plusieurs types publics.",
                string.Empty));
        }

        return result;
    }
}