using System.Collections.ObjectModel;

using System.ComponentModel;

namespace AetherForge.App.Models;

public sealed class LoadedFile : INotifyPropertyChanged
{
    public required string FullPath { get; init; }

    public string? OutputPath { get; set; }

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

    public event PropertyChangedEventHandler? PropertyChanged;

    private string? _selectedOutputFormat;

    public string? SelectedOutputFormat
    {
        get => _selectedOutputFormat;
        set
        {
            if (_selectedOutputFormat == value)
                return;

            _selectedOutputFormat = value;

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(SelectedOutputFormat)));
        }
    }
}