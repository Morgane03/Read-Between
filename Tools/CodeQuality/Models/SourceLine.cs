namespace CodeQuality.Models;

public record SourceLine(
    string File,
    int Number,
    string Text);