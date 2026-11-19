using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using CodeQuality.Models;
using System.Collections.Immutable;

namespace CodeQuality.Checkers;

public sealed class RoslynChecker : IChecker
{
    private static bool _registered;

    public CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("Roslyn");

        if (!_registered)
        {
            MSBuildLocator.RegisterDefaults();
            _registered = true;
        }

        var csproj = Directory
            .EnumerateFiles(rootDirectory, "*.csproj", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (csproj is null)
        {
            result.Errors.Add(new CheckError(
                "",
                0,
                "Aucun .csproj trouvé.",
                ""));

            return result;
        }

        using var workspace = MSBuildWorkspace.Create();

        var project = workspace.OpenProjectAsync(csproj).Result;

        var compilation = project.GetCompilationAsync().Result;

        if (compilation is null)
            return result;

        var analyzers = project.AnalyzerReferences
            .SelectMany(r => r.GetAnalyzers(LanguageNames.CSharp))
            .ToImmutableArray();

        var compilationWithAnalyzers =
            compilation.WithAnalyzers(analyzers);

        var diagnostics = compilationWithAnalyzers
            .GetAllDiagnosticsAsync()
            .Result
            .OrderBy(d => d.Location.SourceTree?.FilePath)
            .ThenBy(d => d.Location.GetLineSpan().StartLinePosition.Line);

        foreach (var diagnostic in diagnostics)
        {
            if (diagnostic.Location == Location.None)
                continue;

            var span = diagnostic.Location.GetLineSpan();

            result.Errors.Add(new CheckError(
                span.Path.Replace('\\', '/'),
                span.StartLinePosition.Line + 1,
                $"{diagnostic.Id} : {diagnostic.GetMessage()}",
                ""));
        }

        return result;
    }
}