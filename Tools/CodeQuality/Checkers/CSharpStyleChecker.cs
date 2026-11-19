using System.Text.RegularExpressions;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public sealed class CSharpStyleChecker : IChecker
{
    private static readonly Regex PrivateFieldRegex =
        new(@"^\s*private\s+(?:readonly\s+)?[\w<>\[\],?]+\s+([A-Za-z_][A-Za-z0-9_]*)");

    private static readonly Regex PublicFieldRegex =
        new(@"^\s*public\s+(?:static\s+)?(?:readonly\s+)?[\w<>\[\],?]+\s+([A-Za-z_][A-Za-z0-9_]*)\s*(?:=|;)");

    private static readonly Regex ConstRegex =
        new(@"^\s*public\s+const\s+[\w<>\[\],?]+\s+([A-Za-z_][A-Za-z0-9_]*)");

    private static readonly Regex PropertyRegex =
        new(@"^\s*(?:public|protected|internal)\s+[\w<>\[\],?]+\s+([A-Za-z_][A-Za-z0-9_]*)\s*\{");

    private static readonly Regex MethodRegex =
        new(@"^\s*(?:public|protected|internal)\s+(?:async\s+)?[\w<>\[\],?]+\s+([A-Za-z_][A-Za-z0-9_]*)\s*\(");

    private static readonly Regex ClassRegex =
        new(@"^\s*(?:public|internal)?\s*(?:sealed\s+|abstract\s+)?class\s+([A-Za-z_][A-Za-z0-9_]*)");

    private static readonly Regex InterfaceRegex =
        new(@"^\s*(?:public|internal)?\s*interface\s+([A-Za-z_][A-Za-z0-9_]*)");

    public CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("C# Style");

        foreach (var file in FileScanner.GetFiles(rootDirectory, "*.cs"))
        {
            var lines = File.ReadAllLines(file);

            for (int i = 0; i < lines.Length; i++)
            {
                CheckRegex(result, file, i, lines[i], PrivateFieldRegex,
                    IsCamelCase,
                    "Le champ privé doit être en camelCase");

                CheckRegex(result, file, i, lines[i], PublicFieldRegex,
                    IsPascalCase,
                    "Le champ public doit être en PascalCase");

                CheckRegex(result, file, i, lines[i], ConstRegex,
                    IsPascalCase,
                    "La constante doit être en PascalCase");

                CheckRegex(result, file, i, lines[i], PropertyRegex,
                    IsPascalCase,
                    "La propriété doit être en PascalCase");

                CheckRegex(result, file, i, lines[i], MethodRegex,
                    IsPascalCase,
                    "La méthode doit être en PascalCase");

                CheckRegex(result, file, i, lines[i], ClassRegex,
                    IsPascalCase,
                    "La classe doit être en PascalCase");

                CheckInterface(result, file, i, lines[i]);
            }
        }

        return result;
    }

    private static void CheckRegex(
        CheckResult result,
        string file,
        int line,
        string text,
        Regex regex,
        Func<string, bool> validator,
        string message)
    {
        var match = regex.Match(text);

        if (!match.Success)
            return;

        var name = match.Groups[1].Value;

        if (validator(name))
            return;

        result.Errors.Add(new CheckError(
            file.Replace('\\', '/'),
            line + 1,
            message,
            text.Trim()));
    }

    private static void CheckInterface(
        CheckResult result,
        string file,
        int line,
        string text)
    {
        var match = InterfaceRegex.Match(text);

        if (!match.Success)
            return;

        var name = match.Groups[1].Value;

        if (name.StartsWith("I") &&
            name.Length > 1 &&
            char.IsUpper(name[1]))
            return;

        result.Errors.Add(new CheckError(
            file.Replace('\\', '/'),
            line + 1,
            "Une interface doit commencer par I",
            text.Trim()));
    }

    private static bool IsCamelCase(string value)
    {
        return value.Length > 0 && char.IsLower(value[0]);
    }

    private static bool IsPascalCase(string value)
    {
        return value.Length > 0 && char.IsUpper(value[0]);
    }
}