namespace AetherForge.App.Models;

public sealed class OperationOption
{
    public required ConversionOperation Operation { get; init; }

    public required string DisplayName { get; init; }

    public override string ToString()
    {
        return DisplayName;
    }
}