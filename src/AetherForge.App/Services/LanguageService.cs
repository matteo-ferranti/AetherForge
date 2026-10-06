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

        ["UnsupportedDocumentPdfConversion"] =
        "Supported inputs: DOCX, DOC, ODT, RTF. Output must be PDF.",

        ["UnsupportedOfficeConversion"] =
            "Text documents support PDF, DOCX, ODT, RTF and TXT output. " +
            "XLS, XLSX and ODS spreadsheets support PDF, XLS, XLSX, ODS and CSV output. " +
            "PPT, PPTX and ODP presentations support PDF, PPT, PPTX and ODP output. " +
            "CSV input currently supports PDF output only. " +
            "Input and output formats must differ.",

        ["OfficeConversionFailed"] =
        "LibreOffice did not produce the converted document.",

        ["InvalidOfficeOutput"] =
        "The converted document is empty or invalid.",

        ["UnsupportedDocumentConversion"] =
        "Unsupported document conversion: {0} → {1}.",

        ["InvalidCsvImport"] =
        "The CSV is empty, malformed, has inconsistent columns, or its separator cannot be determined unambiguously.",

        ["InvalidCsvSheet"] =
        "Select a valid worksheet to export to CSV.",

        ["CsvSheetDialogTitle"] = "Export to CSV",
        ["CsvSheetFileLabel"] = "File: {0}",
        ["CsvSheetSelectionLabel"] = "Choose the worksheet to export:",
        ["CsvExportInfo"] =
            "Only the selected worksheet will be exported. " +
            "CSV contains displayed values, including formula results, " +
            "but does not preserve formulas, formatting or images.",
        ["CsvExportButton"] = "Export",

        ["UnsupportedEpubConversion"] =
            "This conversion requires EPUB input and TXT output.",
        ["InvalidEpubFile"] =
            "The EPUB structure or one of its chapters is invalid or unsupported.",
        ["EpubNoText"] =
            "No extractable text was found in the EPUB.",

        ["UnsupportedEpubPdfConversion"] =
            "This conversion requires EPUB input and PDF output.",

        ["UnsupportedDocumentMetadata"] =
            "Metadata removal currently supports PDF, DOCX, XLSX, PPTX, ODT, ODS and ODP. " +
            "The output must use the same format as the input.",
        ["DocumentMetadataSamePath"] =
            "Choose an output path different from the original document.",
        ["SignedDocumentMetadata"] =
            "Metadata removal is not supported for digitally signed documents.",
        ["InvalidOpenDocumentMetadata"] =
            "The OpenDocument file is invalid or encrypted and cannot be processed.",

        ["QpdfNotFound"] =
            "The component required to process PDFs is missing. Reinstall AetherForge.",

        ["PdfMetadataError"] =
            "PDF metadata could not be removed. The file may be invalid, encrypted, or could not be processed.",

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

        ["UnsupportedDocumentPdfConversion"] =
        "Formati di ingresso supportati: DOCX, DOC, ODT, RTF. Il formato di uscita deve essere PDF.",

        ["UnsupportedOfficeConversion"] =
            "I documenti di testo supportano gli output PDF, DOCX, ODT, RTF e TXT. " +
            "I fogli di calcolo XLS, XLSX e ODS supportano gli output PDF, XLS, XLSX, ODS e CSV. " +
            "Le presentazioni PPT, PPTX e ODP supportano gli output PDF, PPT, PPTX e ODP. " +
            "I file CSV in ingresso supportano attualmente solo l'output PDF. " +
            "Il formato di ingresso e quello di uscita devono essere diversi.",

        ["OfficeConversionFailed"] =
        "LibreOffice non ha prodotto il documento convertito.",

        ["InvalidOfficeOutput"] =
        "Il documento convertito è vuoto o non valido.",

        ["UnsupportedDocumentConversion"] =
        "Conversione documento non supportata: {0} → {1}.",

        ["InvalidCsvSheet"] =
        "Seleziona un foglio valido da esportare in CSV.",

        ["CsvSheetDialogTitle"] = "Esporta in CSV",
        ["CsvSheetFileLabel"] = "File: {0}",
        ["CsvSheetSelectionLabel"] = "Scegli il foglio da esportare:",
        ["CsvExportInfo"] =
            "Verrà esportato soltanto il foglio selezionato. " +
            "Il CSV contiene i valori visualizzati, inclusi i risultati delle formule, " +
            "ma non conserva formule, formattazione o immagini.",
        ["CsvExportButton"] = "Esporta",

        ["UnsupportedEpubConversion"] =
            "Questa conversione richiede un file EPUB in ingresso e TXT in uscita.",
        ["InvalidEpubFile"] =
            "La struttura dell'EPUB o uno dei suoi capitoli non è valido o non è supportato.",
        ["EpubNoText"] =
            "Nell'EPUB non è stato trovato testo estraibile.",

        ["UnsupportedEpubPdfConversion"] =
            "Questa conversione richiede un file EPUB in ingresso e PDF in uscita.",

        ["UnsupportedDocumentMetadata"] =
            "La rimozione dei metadati supporta attualmente PDF, DOCX, XLSX, PPTX, ODT, ODS e ODP. " +
            "Il formato di uscita deve essere uguale a quello di ingresso.",
        ["DocumentMetadataSamePath"] =
            "Scegli un percorso di uscita diverso da quello del documento originale.",
        ["SignedDocumentMetadata"] =
            "La rimozione dei metadati non è supportata per i documenti con firma digitale.",
        ["InvalidOpenDocumentMetadata"] =
            "Il file OpenDocument non è valido oppure è cifrato e non può essere elaborato.",

        ["QpdfNotFound"] =
            "Manca il componente necessario per elaborare i PDF. Reinstalla AetherForge.",

        ["PdfMetadataError"] =
            "Impossibile rimuovere i metadati dal PDF. Il file potrebbe non essere valido, essere cifrato oppure non essere elaborabile.",

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