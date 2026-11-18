using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public sealed class LargeFileChecker : IChecker
{
    private const long MaxFileSize = 100 * 1024 * 1024; // 100 Mo

    public CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("Large Files");

        foreach (var file in FileScanner.GetAllFiles(rootDirectory))
        {
            var info = new FileInfo(file);

            if (!info.Exists)
                continue;

            if (info.Length <= MaxFileSize)
                continue;

            result.Errors.Add(new CheckError(
                file.Replace('\\', '/'),
                0,
                $"Fichier trop volumineux ({FormatSize(info.Length)} > {FormatSize(MaxFileSize)})",
                string.Empty));
        }

        return result;
    }

    private static string FormatSize(long bytes)
    {
        string[] units = ["o", "Ko", "Mo", "Go", "To"];

        double size = bytes;
        int unit = 0;

        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        return $"{size:0.##} {units[unit]}";
    }
}