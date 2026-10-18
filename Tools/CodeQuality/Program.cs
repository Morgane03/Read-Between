using CodeQuality.Checkers;
using CodeQuality.Models;

Console.WriteLine("=================================");
Console.WriteLine(" Read-Between Code Quality");
Console.WriteLine("=================================");
Console.WriteLine();

var results = new List<CheckResult>
{
    DebugLogChecker.Run("."),
    TodoChecker.Run("."),
    JsonChecker.Run("."),
    GuidChecker.Run("."),
};

bool hasErrors = false;

foreach (var result in results)
{
    if (result.Success)
    {
        Console.WriteLine($"✅ {result.Name}");
    }
    else
    {
        hasErrors = true;

        Console.WriteLine($"❌ {result.Name}");

        foreach (var error in result.Errors)
        {
            Console.WriteLine($"Fichier : {error.File}");
            Console.WriteLine($"Ligne   : {error.Line}");
            Console.WriteLine($"Message : {error.Message}");
            Console.WriteLine($"Code    : {error.Code}");
            Console.WriteLine();
        }
    }

    Console.WriteLine("---------------------------------");
}

if (hasErrors)
{
    Environment.Exit(1);
}

Console.WriteLine("Tous les contrôles sont validés.");