using System.Xml;
using CodeQuality.Models;
using CodeQuality.Utils;

namespace CodeQuality.Checkers;

public sealed class XmlChecker : IChecker
{
    public CheckResult Run(string rootDirectory)
    {
        var result = new CheckResult("XML");

        foreach (var file in FileScanner.GetFiles(rootDirectory, "*.xml"))
        {
            try
            {
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Ignore,
                    XmlResolver = null
                };

                using var reader = XmlReader.Create(file, settings);

                while (reader.Read())
                {
                    // Lecture complète du document.
                }
            }
            catch (XmlException ex)
            {
                result.Errors.Add(new CheckError(
                    file.Replace('\\', '/'),
                    ex.LineNumber,
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