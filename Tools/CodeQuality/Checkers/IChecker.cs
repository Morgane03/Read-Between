using CodeQuality.Models;

namespace CodeQuality.Checkers;

public interface IChecker
{
    CheckResult Run(string rootDirectory);
}