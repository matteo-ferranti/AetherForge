namespace AetherForge.App.Models;

public sealed class FormatDefinition
{
    public required string Name { get; init; }

    public required string Extension { get; init; }

    public required string Category { get; init; }

    public bool SupportsTransparency { get; init; }

    public bool SupportsMetadata { get; init; }

    public List<ConversionOperation> SupportedOperations { get; init; } = [];
}