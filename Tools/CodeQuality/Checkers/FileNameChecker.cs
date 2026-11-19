using System.Text.RegularExpressions;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public sealed class FileNameChecker : IChecker
{
    private static readonly Regex TypeRegex =
        new(@"^\s*public\s+(?:sealed\s+|abstract\s+)?(?:class|struct|interface|enum)\s+([A-Za-z_][A-Za-z0-9_]*)",
            RegexOptions.Multiline);

    public CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("File Name");

        foreach (var file in FileScanner.GetFiles(rootDirectory, "*.cs"))
        {
            var text = File.ReadAllText(file);

            var match = TypeRegex.Match(text);

            if (!match.Success)
                continue;

            var expected = Path.GetFileNameWithoutExtension(file);
            var actual = match.Groups[1].Value;

            if (expected == actual)
                continue;

            result.Errors.Add(new CheckError(
                file.Replace('\\', '/'),
                1,
                $"Le nom du fichier devrait être '{actual}.cs'.",
                actual));
        }

        return result;
    }
}