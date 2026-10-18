using System.Text.RegularExpressions;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public static class TodoChecker
{
    private static readonly Regex TodoRegex =
        new(@"\b(TODO|FIXME|HACK|TEMP)\b");

    private static readonly Regex CommentRegex =
        new(@"^\s*(//|///|/\*|\*)");

    public static CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("TODO");

        foreach (var line in FileScanner.ReadLines(rootDirectory, "*.cs"))
        {
            if (!CommentRegex.IsMatch(line.Text))
                continue;

            if (!TodoRegex.IsMatch(line.Text))
                continue;

            result.Errors.Add(new CheckError(
                line.File,
                line.Number,
                "TODO détecté",
                line.Text.Trim()));
        }

        return result;
    }
}