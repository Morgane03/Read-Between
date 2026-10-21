using CodeQuality.Checkers;
using CodeQuality.Models;

Console.WriteLine("=================================");
Console.WriteLine(" Read-Between Code Quality");
Console.WriteLine("=================================");
Console.WriteLine();

IChecker[] checkers =
[
    new DebugLogChecker(),
    new TodoChecker(),
    new JsonChecker(),
    new GuidChecker(),
    new UnityReferenceChecker()
];

bool hasErrors = false;

foreach (var checker in checkers)
{
    var result = checker.Run(".");

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

Environment.Exit(hasErrors ? 1 : 0);