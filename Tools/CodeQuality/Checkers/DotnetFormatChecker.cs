using System.Diagnostics;
using System.Text.RegularExpressions;
using CodeQuality.Models;

namespace CodeQuality.Checkers;

public sealed class DotnetFormatChecker : IChecker
{
    private static readonly Regex DiagnosticRegex =
        new(@"^(.*)\((\d+),(\d+)\):\s*(warning|error)\s+([A-Z0-9]+):\s*(.*)$");

    public CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("Roslyn");

        var solution = Directory
            .EnumerateFiles(rootDirectory, "*.sln", SearchOption.TopDirectoryOnly)
            .FirstOrDefault();

        if (solution is null)
        {
            result.Errors.Add(new CheckError(
                "",
                0,
                "Aucune solution .sln trouvée.",
                ""));

            return result;
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"format analyzers \"{solution}\" --verify-no-changes --severity warn --verbosity diagnostic",
                WorkingDirectory = rootDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        output += Environment.NewLine;
        output += process.StandardError.ReadToEnd();

        process.WaitForExit();

        foreach (var line in output.Split(Environment.NewLine))
        {
            var match = DiagnosticRegex.Match(line);

            if (!match.Success)
                continue;

            result.Errors.Add(new CheckError(
                match.Groups[1].Value.Replace('\\', '/'),
                int.Parse(match.Groups[2].Value),
                $"{match.Groups[5].Value} : {match.Groups[6].Value}",
                ""));
        }

        return result;
    }
}