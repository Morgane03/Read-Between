using System.IO;
using NUnit.Framework;

public class NoDebugLogTests
{
    [Test]
    public void No_Debug_Log_Should_Be_Present()
    {
        if (!file.StartsWith("Assets"))
            continue;
        else if (file.StartsWith("Assets/Tests/"))
            continue;

        string[] files = Directory.GetFiles(
            "Assets",
            "*.cs",
            SearchOption.AllDirectories);

        foreach (string file in files)
        {
            if (file.Contains("/Tests/"))
                continue;

            string code = File.ReadAllText(file);

            Assert.IsFalse(
                code.Contains("Debug.Log("),
                $"Debug.Log trouvé dans {file}");

            Assert.IsFalse(
                code.Contains("Debug.LogWarning("),
                $"Debug.LogWarning trouvé dans {file}");

            Assert.IsFalse(
                code.Contains("Debug.LogError("),
                $"Debug.LogError trouvé dans {file}");
        }
    }
}