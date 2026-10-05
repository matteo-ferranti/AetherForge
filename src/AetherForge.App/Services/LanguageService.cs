using System.Windows;

namespace AetherForge.App.Services;

public static class LanguageService
{
    private static readonly Dictionary<string, string> English = new()
    {
        ["DragFilesText"] = "Drag your files here",
        ["OrText"] = "or",
        ["SelectFilesText"] = "Select files",

        ["NameText"] = "Name",
        ["CategoryText"] = "Category",
        ["InputText"] = "Input",
        ["OperationText"] = "Operation",
        ["OutputText"] = "Output",
        ["SizeText"] = "Size",

        ["RemoveSelectedText"] = "Remove selected",
        ["ClearListText"] = "Clear list",
        ["ConvertText"] = "Convert",

        ["OperationConvert"] = "Convert",
        ["OperationExtractAudio"] = "Extract audio",
        ["OperationCreateGif"] = "Create GIF",
        ["OperationRemoveAudio"] = "Remove audio",
        ["OperationRemoveMetadata"] = "Remove metadata",
        ["OperationResize"] = "Resize",
        ["OperationChangeQuality"] = "Change quality",

        ["ResizeWidthText"] = "Width",
        ["ResizeHeightText"] = "Height",
        ["QualityText"] = "Quality %",
        ["AudioBitrateText"] = "Bitrate (kbps)",

        ["ConversionCompletedTitle"] = "Conversion completed",
        ["ConversionCompletedMessage"] = "The conversion was completed successfully.",
        ["ConversionErrorTitle"] = "Conversion error",
        ["ConversionErrorMessage"] = "An error occurred during conversion.",
        ["NoConvertibleImagesMessage"] = "No image files are ready to be converted.",

        ["StatusText"] = "Status",
        ["StatusPending"] = "Pending",
        ["StatusCompleted"] = "Completed",
        ["StatusError"] = "Error",

        ["BatchFolderDialogTitle"] = "Batch output",
        ["BatchFolderNameText"] = "Choose a name for the output folder:",
        ["SelectBatchFolderText"] = "Choose where to create the output folder",
        ["OkText"] = "OK",
        ["CancelText"] = "Cancel",

        ["BatchCompletedWithErrorsMessage"] =
    "Operation completed.\n\nCompleted: {0}\nErrors: {1}",

        ["NoConvertibleFilesMessage"] = "There are no files ready for conversion.",

    };

    private static readonly Dictionary<string, string> Italian = new()
    {
        ["DragFilesText"] = "Trascina qui i tuoi file",
        ["OrText"] = "oppure",
        ["SelectFilesText"] = "Seleziona file",

        ["NameText"] = "Nome",
        ["CategoryText"] = "Categoria",
        ["InputText"] = "Input",
        ["OperationText"] = "Operazione",
        ["OutputText"] = "Output",
        ["SizeText"] = "Dimensione",

        ["RemoveSelectedText"] = "Rimuovi selezionati",
        ["ClearListText"] = "Svuota lista",
        ["ConvertText"] = "Converti",

        ["OperationConvert"] = "Converti",
        ["OperationExtractAudio"] = "Estrai audio",
        ["OperationCreateGif"] = "Crea GIF",
        ["OperationRemoveAudio"] = "Rimuovi audio",
        ["OperationRemoveMetadata"] = "Rimuovi metadati",
        ["OperationResize"] = "Ridimensiona",
        ["OperationChangeQuality"] = "Cambia qualità",

        ["ResizeWidthText"] = "Larghezza",
        ["ResizeHeightText"] = "Altezza",
        ["QualityText"] = "Qualità %",
        ["AudioBitrateText"] = "Bitrate (kbps)",

        ["ConversionCompletedTitle"] = "Conversione completata",
        ["ConversionCompletedMessage"] = "La conversione è stata completata con successo.",
        ["ConversionErrorTitle"] = "Errore di conversione",
        ["ConversionErrorMessage"] = "Si è verificato un errore durante la conversione.",
        ["NoConvertibleImagesMessage"] = "Nessun file immagine è pronto per essere convertito.",

        ["StatusText"] = "Stato",
        ["StatusPending"] = "In attesa",
        ["StatusCompleted"] = "Completato",
        ["StatusError"] = "Errore",

        ["BatchFolderDialogTitle"] = "Output multiplo",
        ["BatchFolderNameText"] = "Scegli un nome per la cartella di output:",
        ["SelectBatchFolderText"] = "Scegli dove creare la cartella di output",
        ["OkText"] = "OK",
        ["CancelText"] = "Annulla",

        ["BatchCompletedWithErrorsMessage"] =
    "Operazione completata.\n\nCompletati: {0}\nErrori: {1}",

        ["NoConvertibleFilesMessage"] = "Non ci sono file pronti per la conversione.",

    };

    public static string CurrentLanguage { get; private set; } = "en";

    public static void SetLanguage(string language)
    {
        Dictionary<string, string> strings =
            language == "it"
                ? Italian
                : English;

        CurrentLanguage =
            language == "it"
                ? "it"
                : "en";

        foreach (KeyValuePair<string, string> item in strings)
        {
            Application.Current.Resources[item.Key] = item.Value;
        }
    }

    public static string Get(string key)
    {
        if (Application.Current.Resources[key] is string value)
        {
            return value;
        }

        return key;
    }
}