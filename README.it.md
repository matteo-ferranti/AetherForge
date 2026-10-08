<p align="center">
  <img src="src/AetherForge.App/Assets/Logos/aetherforge_logo.png" alt="AetherForge" width="460">
</p>

<h1 align="center">AetherForge Converter</h1>

<p align="center">
  Un'applicazione Windows per convertire ed elaborare immagini, audio, video e documenti.
</p>

<p align="center">
  <a href="https://aetherforgeconverter.pages.dev/it/">Sito web</a> ·
  <a href="https://github.com/matteo-ferranti/AetherForge/releases">Release</a> ·
  <a href="https://github.com/matteo-ferranti/AetherForge/issues">Segnala un problema</a> ·
  <a href="README.md">English</a>
</p>

---

## Panoramica

AetherForge riunisce la conversione dei file e strumenti pratici di modifica in un'unica applicazione desktop. Permette di elaborare singoli file o più file insieme, con un'interfaccia disponibile in italiano e inglese.

L'elaborazione avviene localmente sul computer. Non è necessario caricare i file su un servizio di conversione online. L'installer include il runtime .NET e i componenti utilizzati dall'applicazione per elaborare i file.

## Funzionalità

| Categoria | Strumenti disponibili |
| --- | --- |
| Immagini | Conversione, ridimensionamento, rotazione, capovolgimento, regolazione della qualità e rimozione dei metadati per i formati supportati. |
| Audio | Conversione, ritaglio, regolazione del bitrate per i formati supportati e rimozione dei metadati. |
| Video | Conversione, ritaglio, estrazione e rimozione dell'audio, creazione di GIF e rimozione dei metadati. |
| Documenti di testo | Conversione tra i formati di testo e Office supportati, estrazione del testo dai PDF ed esportazione in PDF. |
| Fogli di calcolo | Conversione tra i formati supportati, esportazione in PDF e in CSV con selezione del foglio. I CSV in ingresso possono essere esportati in PDF. |
| Presentazioni | Conversione tra PPT, PPTX e ODP ed esportazione in PDF. |
| E-book | Conversione di EPUB in TXT e PDF. |
| Strumenti PDF | Unione di file nell'ordine scelto ed estrazione di pagine o intervalli di pagine. |

L'elenco dei file offre inoltre trascinamento, scelta dell'operazione e del formato di output per ciascun file, stato dell'elaborazione e pulsanti per aprire i risultati completati o mostrarli nella cartella.

## Screenshot

### Finestra principale

Aggiungi i file, scegli le operazioni e i formati di output e controlla i risultati dell'elaborazione.

![Finestra principale di AetherForge](docs/screenshots/main-window.png)

### Ritaglio audio

Seleziona l'inizio e la fine del segmento audio da esportare.

![Finestra di ritaglio audio](docs/screenshots/audio-trim.png)

### Unione PDF

Aggiungi i file PDF e scegli il loro ordine prima di unirli.

![Finestra di unione PDF](docs/screenshots/pdf-merge.png)

## Esempi di formati

| Categoria | Esempi di formati in ingresso | Formati di conversione in uscita |
| --- | --- | --- |
| Immagini | JPG, PNG, WebP, GIF, BMP, TIFF, AVIF, HEIC, HEIF, ICO, SVG | JPG, PNG, WebP, BMP, TIFF, AVIF |
| Audio | MP3, WAV, FLAC, AAC, M4A, OGG, OPUS, WMA, AIFF, AC3 | MP3, WAV, FLAC, AAC, M4A, OGG, OPUS |
| Video | MP4, MKV, AVI, MOV, WebM, WMV, FLV, MPEG, TS | MP4, MKV, MOV, WebM, AVI |
| Documenti | PDF, TXT, DOC, DOCX, ODT, RTF, HTML, EPUB | I formati disponibili dipendono dal file in ingresso |
| Fogli di calcolo | XLS, XLSX, ODS, CSV | I formati disponibili dipendono dal file in ingresso |
| Presentazioni | PPT, PPTX, ODP | PDF e formati di presentazione supportati |

Questi elenchi non indicano che ogni formato possa essere convertito in tutti gli altri. L'applicazione mostra le operazioni e i formati disponibili per ciascun file in ingresso.

## Installazione

**Piattaforma di destinazione:** Windows 11 con processore Intel o AMD x64.

Per le versioni distribuite tramite installer:

1. Scarica l'installer dalla pagina [GitHub Releases](https://github.com/matteo-ferranti/AetherForge/releases).
2. Avvia `AetherForge-Setup-<versione>-win-x64.exe`.
3. Seleziona italiano o inglese e segui la procedura.
4. Apri AetherForge dal menu Start o dal collegamento desktop facoltativo.

L'installazione riguarda l'utente corrente e non richiede privilegi di amministratore. Non occorre installare separatamente .NET, FFmpeg, LibreOffice o qpdf.

Per disinstallare, usa **Impostazioni → App → App installate → AetherForge**.

L'installer attuale è destinato a Windows 11 x64. La compatibilità con Windows 10 e ARM non è stata verificata.

## Primi passi

1. Trascina i file nella finestra principale oppure usa **Seleziona file**.
2. Scegli un'operazione e un formato di output disponibile per ciascun file.
3. Imposta le eventuali opzioni richieste dall'operazione.
4. Avvia l'elaborazione e scegli dove salvare i risultati.
5. Controlla lo stato dei file, poi apri il risultato o la cartella che lo contiene.

L'unione PDF utilizza una finestra dedicata, in cui puoi aggiungere i file, modificarne l'ordine e scegliere la destinazione.

## Protezioni e limitazioni

AetherForge include controlli contro la sovrascrittura dei file in ingresso, scritture temporanee per i flussi supportati, limiti sulle risorse per immagini e archivi e restrizioni su alcune risorse esterne e sui contenuti attivi dei documenti.

Questi controlli non costituiscono una sandbox completa né garantiscono la sicurezza di qualsiasi file in ingresso.

- **PDF → TXT non include OCR:** le pagine scansionate devono contenere un livello di testo per consentirne l'estrazione.
- La conversione di documenti Office e presentazioni può modificare impaginazione, font o funzioni specifiche dell'applicazione di origine.
- Le operazioni e le impostazioni di qualità variano in base al formato.
- La rimozione dei metadati è disponibile per i formati supportati e non equivale all'anonimizzazione completa del contenuto.
- File danneggiati, protetti o troppo grandi possono essere rifiutati.

## Verifica del rilascio

L'applicazione pubblicata e l'installer hanno superato le prove di conversione, installazione e disinstallazione sul computer di sviluppo. La prova su un'installazione pulita di Windows è ancora da effettuare.

## Compilazione dal codice sorgente

Requisiti:

- Windows con **SDK .NET 10**.
- Componenti di elaborazione x64 compatibili nelle seguenti cartelle:

```text
tools/
├── ffmpeg/
│   ├── ffmpeg.exe
│   └── ffprobe.exe
├── libreoffice/
│   └── program/soffice.com
└── qpdf/
    └── bin/qpdf.exe
```

Sono necessarie le distribuzioni complete di LibreOffice e qpdf, con i relativi file di supporto, non soltanto gli eseguibili. Queste cartelle non sono incluse nel repository.

Dalla cartella principale del repository:

```powershell
dotnet restore .\src\AetherForge.App\AetherForge.App.csproj
dotnet build .\src\AetherForge.App\AetherForge.App.csproj -c Release
```

Per pubblicare una versione Windows x64 con runtime incluso:

```powershell
dotnet publish .\src\AetherForge.App\AetherForge.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -p:PublishTrimmed=false -o .\artifacts\release\win-x64
```

Mantieni insieme l'intera cartella pubblicata. Per creare l'installer, apri `AetherForge.iss` in **Inno Setup 7** e compilalo dopo aver pubblicato l'applicazione.

## Componenti e licenze

AetherForge utilizza:

- **Magick.NET / ImageMagick** per l'elaborazione delle immagini.
- **FFmpeg e FFprobe** per l'elaborazione di audio e video.
- **LibreOffice** per la conversione dei documenti Office.
- **PdfPig** per l'estrazione del testo dai PDF.
- **Open XML SDK** per le operazioni supportate sui documenti Office.
- **qpdf** per l'elaborazione dei PDF.
- **Noto Sans** per il testo nei documenti generati.

Il codice originale di AetherForge è distribuito con **[licenza MIT](LICENSE)**. I componenti di terze parti mantengono le proprie licenze e i rispettivi avvisi di copyright.

I testi delle licenze e gli avvisi sono disponibili in `src/AetherForge.App/Assets/Licenses` e nella finestra **Informazioni e licenze** dell'applicazione. La redistribuzione dei componenti inclusi richiede anche il rispetto delle rispettive licenze, compresi gli eventuali obblighi relativi ai sorgenti corrispondenti.

## Autore e attribuzione

Realizzata da **Matteo Ferranti**.

Se utilizzi AetherForge o il suo codice nel tuo progetto, una citazione visibile dell'autore e un collegamento al repository sono apprezzati. Si tratta di una richiesta, non di una condizione aggiuntiva della licenza MIT.

## Segnalazioni e contributi

Suggerimenti, segnalazioni riproducibili e contributi sono benvenuti tramite [GitHub Issues](https://github.com/matteo-ferranti/AetherForge/issues) e pull request.

Quando segnali un problema, indica la versione di Windows, la versione di AetherForge, i formati di ingresso e uscita, l'operazione e il messaggio di errore. Condividi un file di esempio privo di dati sensibili soltanto se necessario.


