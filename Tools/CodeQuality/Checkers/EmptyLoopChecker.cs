using System.Text.RegularExpressions;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public sealed class EmptyLoopChecker : IChecker
{
    private static readonly Regex EmptyLoopRegex =
        new(@"(for|foreach|while)\s*\([^\)]*\)\s*\{\s*\}",
            RegexOptions.Singleline);

    public CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("Empty Loop");

        foreach (var file in FileScanner.GetFiles(rootDirectory, "*.cs"))
        {
            var text = File.ReadAllText(file);

            foreach (Match match in EmptyLoopRegex.Matches(text))
            {
                var line =
                    text[..match.Index].Count(c => c == '\n') + 1;

                result.Errors.Add(new CheckError(
                    file.Replace('\\', '/'),
                    line,
                    "Boucle vide.",
                    match.Value));
            }
        }

        return result;
    }
}