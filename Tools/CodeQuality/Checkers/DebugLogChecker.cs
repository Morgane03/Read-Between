using System.Text.RegularExpressions;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public static class DebugLogChecker
{
    private static readonly Regex DebugRegex =
        new(@"Debug\.(Log|LogWarning|LogError)\s*\(");

    private static readonly Regex CommentRegex =
        new(@"^\s*(//|///|/\*|\*)");

    public static CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("Debug.Log");

        foreach (var line in FileScanner.ReadLines(rootDirectory, "*.cs"))
        {
            if (CommentRegex.IsMatch(line.Text))
                continue;

            if (!DebugRegex.IsMatch(line.Text))
                continue;

            result.Errors.Add(new CheckError(
                line.File,
                line.Number,
                "Debug.Log détecté",
                line.Text.Trim()));
        }

        return result;
    }
}