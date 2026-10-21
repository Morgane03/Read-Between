using System.Text.Json;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public sealed class JsonChecker : IChecker
{
    public CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("JSON");

        foreach (var file in FileScanner.GetFiles(rootDirectory, "*.json"))
        {
            try
            {
                var text = File.ReadAllText(file);

                if (string.IsNullOrWhiteSpace(text))
                {
                    result.Errors.Add(new CheckError(
                        file.Replace('\\', '/'),
                        0,
                        "Fichier JSON vide",
                        string.Empty));

                    continue;
                }

                JsonDocument.Parse(text);
            }
            catch (JsonException ex)
            {
                result.Errors.Add(new CheckError(
                    file.Replace('\\', '/'),
                    (int)ex.LineNumber + 1,
                    ex.Message,
                    string.Empty));
            }
            catch (Exception ex)
            {
                result.Errors.Add(new CheckError(
                    file.Replace('\\', '/'),
                    0,
                    ex.Message,
                    string.Empty));
            }
        }

        return result;
    }
}