namespace AetherForge.App.Models;

public sealed class LoadedFile
{
    public required string FullPath { get; init; }

    public required string Name { get; init; }

    public required string Category { get; init; }

    public required string Format { get; init; }

    public required string Size { get; init; }
}