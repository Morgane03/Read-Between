using System.Text.RegularExpressions;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public sealed class NamespaceChecker : IChecker
{
    private static readonly Regex NamespaceRegex =
        new(@"^\s*namespace\s+[A-Za-z0-9_.]+", RegexOptions.Multiline);

    public CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("Namespace");

        foreach (var file in FileScanner.GetFiles(rootDirectory, "*.cs"))
        {
            var text = File.ReadAllText(file);

            if (NamespaceRegex.IsMatch(text))
                continue;

            result.Errors.Add(new CheckError(
                file.Replace('\\', '/'),
                1,
                "Le fichier ne contient aucun namespace.",
                string.Empty));
        }

        return result;
    }
}