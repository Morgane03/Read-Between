namespace CodeQuality.Models;

public class CheckError
{
    public string File { get; }

    public int Line { get; }

    public string Message { get; }

    public string Code { get; }

    public CheckError(
        string file,
        int line,
        string message,
        string code)
    {
        File = file;
        Line = line;
        Message = message;
        Code = code;
    }
}