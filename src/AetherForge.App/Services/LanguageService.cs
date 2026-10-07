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

        ["OperationRotate"] = "Rotate",
        ["OperationFlip"] = "Flip",
        ["ImageTransformFileLabel"] = "File: {0}",
        ["RotationAngleLabel"] = "Rotation angle (clockwise):",
        ["FlipDirectionLabel"] = "Flip direction:",
        ["FlipHorizontalLabel"] = "Horizontal — swap left and right",
        ["FlipVerticalLabel"] = "Vertical — swap top and bottom",
        ["ImageTransformContinue"] = "Continue",

        ["OperationExtractPages"] = "Extract pages",
        ["PdfProcessingError"] =
            "The PDF could not be processed. It may be invalid or require a password.",
        ["PdfPageRangeInvalid"] =
            "Choose a valid page range within the document. The last page must be greater than or equal to the first page.",

        ["PdfPagesFileLabel"] = "File: {0}",
        ["PdfPageCountLabel"] = "Total pages: {0}",
        ["PdfFirstPageLabel"] = "First page:",
        ["PdfLastPageLabel"] = "Last page:",

        ["PdfMergeMinimumFiles"] =
            "Select at least two PDF files to merge.",

        ["PdfMergeSamePath"] =
            "Choose an output path different from all the original PDF files.",

        ["PdfMergeTitle"] = "Merge PDFs",
        ["PdfMergeInfo"] =
            "Add or drag at least two PDF files into this window. All pages will be merged in the order shown, from top to bottom.",
        ["PdfMergeAdd"] = "Add PDFs",
        ["PdfMergeRemove"] = "Remove",
        ["PdfMergeMoveUp"] = "Move up",
        ["PdfMergeMoveDown"] = "Move down",
        ["PdfMergeFileFilter"] = "PDF files (*.pdf)|*.pdf",
        ["PdfMergeSaveTitle"] = "Save merged PDF",
        ["PdfMergeDefaultFileName"] = "merged.pdf",
        ["PdfMergeCompleted"] =
            "PDF files merged successfully.\n\nSaved file:\n{0}",
        ["PdfMergeFailed"] =
            "The PDF files could not be merged. Check that the input files are accessible and that you can save to the selected folder.",

        ["OperationTrim"] = "Trim",

        ["MediaToolsMissing"] =
            "A component required to process audio and video is missing. Reinstall AetherForge.",

        ["MediaDurationUnavailable"] =
            "The file duration could not be determined.",

        ["MediaTrimSamePath"] =
            "Choose an output path different from the original file.",

        ["MediaTrimUnsupportedOutput"] =
            "The selected output format is not supported for trimming.",

        ["MediaTrimInvalidRange"] =
            "Choose a valid time range within the file. The end time must be greater than the start time.",

        ["MediaTrimFailed"] =
            "The selected segment could not be saved.",

        ["MediaTrimFileLabel"] = "File: {0}",
        ["MediaTrimDurationLabel"] = "Total duration: {0} seconds",
        ["MediaTrimStartLabel"] = "Start time (seconds):",
        ["MediaTrimEndLabel"] = "End time (seconds):",
        ["MediaTrimTimeInfo"] =
            "Enter times in seconds. For example, 90 means 1 minute and 30 seconds. You can use a dot or comma for decimals.",

        ["OpenOutputText"] = "Open file",
        ["ShowOutputFolderText"] = "Show in folder",

        ["OutputFileUnavailable"] =
            "The output file is no longer available. It may have been moved or deleted.",

        ["OpenOutputFailed"] =
            "The file could not be opened. Check that an application is installed to open this file type.",

        ["OpenOutputFolderFailed"] =
            "The file could not be shown in its folder.",

        ["SaveOutputTitle"] = "Save output file",

        ["OutputFormatFilter"] =
            "{0} files (*.{1})|*.{1}",

        ["OutputExtensionMismatch"] =
            "The output file must use the {0} extension.",

        ["OutputMatchesOriginal"] =
            "An original input file cannot be overwritten. Choose a different file name or folder.",

        ["CannotVerifyOutputSafety"] =
            "The output file could not be checked safely. Choose a new file name that does not already exist.",

        ["InvalidImageOutput"] =
            "The generated image is missing or empty. The output file was not replaced.",

        ["InvalidMediaOutput"] =
            "The output file could not be generated correctly. The destination file was not replaced.",

        ["ArchiveSafetyLimitExceeded"] =
            "The document exceeds the safety limits for archive size, expanded content, or number of entries.",

        ["DocumentXmlLimitExceeded"] =
            "A document section exceeds the safety limit for XML content.",

        ["ImageSafetyLimitExceeded"] =
            "The image exceeds the safety limits for dimensions, complexity or processing resources.",

        ["ErrorDetailsLimited"] =
            "Only the first 10 errors are shown.",

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

        ["OperationRotate"] = "Ruota",
        ["OperationFlip"] = "Capovolgi",
        ["ImageTransformFileLabel"] = "File: {0}",
        ["RotationAngleLabel"] = "Angolo di rotazione (senso orario):",
        ["FlipDirectionLabel"] = "Direzione del ribaltamento:",
        ["FlipHorizontalLabel"] = "Orizzontale — scambia sinistra e destra",
        ["FlipVerticalLabel"] = "Verticale — scambia alto e basso",
        ["ImageTransformContinue"] = "Continua",

        ["OperationExtractPages"] = "Estrai pagine",
        ["PdfProcessingError"] =
            "Impossibile elaborare il PDF. Il file potrebbe non essere valido oppure richiedere una password.",
        ["PdfPageRangeInvalid"] =
            "Scegli un intervallo di pagine valido all'interno del documento. L'ultima pagina deve essere maggiore o uguale alla prima.",

        ["PdfPagesFileLabel"] = "File: {0}",
        ["PdfPageCountLabel"] = "Pagine totali: {0}",
        ["PdfFirstPageLabel"] = "Prima pagina:",
        ["PdfLastPageLabel"] = "Ultima pagina:",

        ["PdfMergeMinimumFiles"] =
            "Seleziona almeno due file PDF da unire.",

        ["PdfMergeSamePath"] =
            "Scegli un percorso di uscita diverso da quello di tutti i PDF originali.",

        ["PdfMergeTitle"] = "Unisci PDF",
        ["PdfMergeInfo"] =
            "Aggiungi o trascina almeno due file PDF in questa finestra. Tutte le pagine verranno unite nell'ordine mostrato, dall'alto verso il basso.",
        ["PdfMergeAdd"] = "Aggiungi PDF",
        ["PdfMergeRemove"] = "Rimuovi",
        ["PdfMergeMoveUp"] = "Sposta su",
        ["PdfMergeMoveDown"] = "Sposta giù",
        ["PdfMergeFileFilter"] = "File PDF (*.pdf)|*.pdf",
        ["PdfMergeSaveTitle"] = "Salva PDF unito",
        ["PdfMergeDefaultFileName"] = "unito.pdf",
        ["PdfMergeCompleted"] =
            "PDF uniti correttamente.\n\nFile salvato:\n{0}",
        ["PdfMergeFailed"] =
            "Impossibile unire i PDF. Verifica che i file di ingresso siano accessibili e che sia possibile salvare nella cartella selezionata.",

            ["OperationTrim"] = "Taglia",

        ["MediaToolsMissing"] =
            "Manca un componente necessario per elaborare audio e video. Reinstalla AetherForge.",

        ["MediaDurationUnavailable"] =
            "Impossibile determinare la durata del file.",

        ["MediaTrimSamePath"] =
            "Scegli un percorso di uscita diverso da quello del file originale.",

        ["MediaTrimUnsupportedOutput"] =
            "Il formato di uscita selezionato non è supportato per il taglio.",

        ["MediaTrimInvalidRange"] =
            "Scegli un intervallo di tempo valido all'interno del file. Il tempo finale deve essere maggiore di quello iniziale.",

        ["MediaTrimFailed"] =
            "Impossibile salvare il segmento selezionato.",

        ["MediaTrimFileLabel"] = "File: {0}",
        ["MediaTrimDurationLabel"] = "Durata totale: {0} secondi",
        ["MediaTrimStartLabel"] = "Tempo iniziale (secondi):",
        ["MediaTrimEndLabel"] = "Tempo finale (secondi):",
        ["MediaTrimTimeInfo"] =
            "Inserisci i tempi in secondi. Ad esempio, 90 indica 1 minuto e 30 secondi. Puoi usare il punto o la virgola per i decimali.",

        ["OpenOutputText"] = "Apri file",
        ["ShowOutputFolderText"] = "Mostra nella cartella",

        ["OutputFileUnavailable"] =
            "Il file di uscita non è più disponibile. Potrebbe essere stato spostato o eliminato.",

        ["OpenOutputFailed"] =
            "Impossibile aprire il file. Verifica che sia installata un'applicazione adatta a questo tipo di file.",

        ["OpenOutputFolderFailed"] =
            "Impossibile mostrare il file nella sua cartella.",

        ["SaveOutputTitle"] = "Salva file di uscita",

        ["OutputFormatFilter"] =
            "File {0} (*.{1})|*.{1}",

        ["OutputExtensionMismatch"] =
            "Il file di uscita deve utilizzare l'estensione {0}.",

        ["OutputMatchesOriginal"] =
            "Non è possibile sovrascrivere un file originale di ingresso. Scegli un nome o una cartella diversi.",

        ["CannotVerifyOutputSafety"] =
            "Impossibile verificare in sicurezza il file di uscita. Scegli un nuovo nome di file che non esista già.",

        ["InvalidImageOutput"] =
            "L'immagine generata è mancante o vuota. Il file di uscita non è stato sostituito.",

        ["InvalidMediaOutput"] =
            "Impossibile generare correttamente il file di uscita. Il file di destinazione non è stato sostituito.",

        ["ArchiveSafetyLimitExceeded"] =
            "Il documento supera i limiti di sicurezza per le dimensioni dell'archivio, il contenuto decompresso o il numero di elementi.",

        ["DocumentXmlLimitExceeded"] =
            "Una sezione del documento supera il limite di sicurezza per il contenuto XML.",

        ["ImageSafetyLimitExceeded"] =
            "L'immagine supera i limiti di sicurezza per dimensioni, complessità o risorse di elaborazione.",

        ["ErrorDetailsLimited"] =
            "Sono mostrati soltanto i primi 10 errori.",

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


    public static string GetUserFriendlyError(
    Exception exception,
    string fallbackKey = "ConversionErrorMessage")
{
    string[] knownErrorKeys =
    [
        "OutputMatchesOriginal",
        "CannotVerifyOutputSafety",
        "InvalidImageOutput",
        "InvalidMediaOutput",
        "ArchiveSafetyLimitExceeded",
        "DocumentXmlLimitExceeded",
        "DocumentMetadataSamePath",
        "SignedDocumentMetadata",
        "UnsupportedDocumentMetadata",
        "InvalidOpenDocumentMetadata",
        "PdfMetadataError",
        "PdfProcessingError",
        "PdfPageRangeInvalid",
        "PdfMergeMinimumFiles",
        "PdfMergeSamePath",
        "PdfMergeFailed",
        "QpdfNotFound",
        "InvalidCsvImport",
        "InvalidCsvSheet",
        "InvalidEpubFile",
        "EpubNoText",
        "UnsupportedEpubConversion",
        "UnsupportedEpubPdfConversion",
        "UnsupportedDocumentPdfConversion",
        "UnsupportedOfficeConversion",
        "InvalidOfficeOutput",
        "OfficeConversionFailed",
        "MediaToolsMissing",
        "MediaDurationUnavailable",
        "MediaTrimSamePath",
        "MediaTrimUnsupportedOutput",
        "MediaTrimInvalidRange",
        "MediaTrimFailed"
    ];

    Exception? current = exception;

    for (int depth = 0; current is not null && depth < 8; depth++)
    {
        if (current is ImageMagick.MagickResourceLimitErrorException ||
    (current is ImageMagick.MagickImageErrorException &&
     current.Message.StartsWith(
         "width or height exceeds limit",
         StringComparison.OrdinalIgnoreCase)))
{
    return Get("ImageSafetyLimitExceeded");
}

        foreach (string key in knownErrorKeys)
        {
            bool matchesEnglish =
                English.TryGetValue(key, out string? english) &&
                current.Message == english;

            bool matchesItalian =
                Italian.TryGetValue(key, out string? italian) &&
                current.Message == italian;

            if (matchesEnglish || matchesItalian)
            {
                return Get(key);
            }
        }

        current = current.InnerException;
    }

    return Get(fallbackKey);
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