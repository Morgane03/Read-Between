namespace CodeQuality.Models;

public class CheckResult
{
    public string Name { get; }

    public List<CheckError> Errors { get; } = new();

    public bool Success => Errors.Count == 0;

    public CheckResult(string name)
    {
        Name = name;
    }
}