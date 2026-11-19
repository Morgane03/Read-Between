using System.Text.RegularExpressions;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public sealed class EmptyIfChecker : IChecker
{
    private static readonly Regex EmptyIfRegex =
        new(@"if\s*\([^\)]*\)\s*\{\s*\}",
            RegexOptions.Singleline);

    public CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("Empty If");

        foreach (var file in FileScanner.GetFiles(rootDirectory, "*.cs"))
        {
            var text = File.ReadAllText(file);

            foreach (Match match in EmptyIfRegex.Matches(text))
            {
                var line =
                    text[..match.Index].Count(c => c == '\n') + 1;

                result.Errors.Add(new CheckError(
                    file.Replace('\\', '/'),
                    line,
                    "Bloc if vide.",
                    match.Value));
            }
        }

        return result;
    }
}