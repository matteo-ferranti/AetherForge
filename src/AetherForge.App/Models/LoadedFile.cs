using System.Collections.ObjectModel;

namespace AetherForge.App.Models;

public sealed class LoadedFile
{
    public required string FullPath { get; init; }

    public required string Name { get; init; }

    public required string Category { get; init; }

    public required string Format { get; init; }

    public required string Size { get; init; }

    public ConversionStatus Status { get; set; } =
    ConversionStatus.Pending;

    public string StatusText { get; set; } = string.Empty;

    public ObservableCollection<OperationOption> AvailableOperations { get; init; } = [];

    public OperationOption? SelectedOperation { get; set; }

    public int? ResizeWidth { get; set; }

    public int? ResizeHeight { get; set; }

    public int? QualityPercent { get; set; }

    public int? AudioBitrateKbps { get; set; } = 192;

    public int[] AvailableAudioBitrates { get; } =
    [
        64, 96, 128, 160, 192, 256, 320
    ];

    public ObservableCollection<string> AvailableOutputFormats { get; init; } = [];

    public string? SelectedOutputFormat { get; set; }
}