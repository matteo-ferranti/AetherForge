<p align="center">
  <img src="src/AetherForge.App/Assets/Logos/aetherforge_logo.png" alt="AetherForge" width="460">
</p>

<h1 align="center">AetherForge Converter</h1>

<p align="center">
  Un'applicazione Windows per convertire ed elaborare immagini, audio, video e documenti.
</p>

<p align="center">
  <a href="https://flameswornstudio.com/aetherforgeconverter/it/">Sito web</a> ·
  <a href="https://github.com/matteo-ferranti/AetherForge/releases">Release</a> ·
  <a href="CHANGELOG.it.md">Changelog</a> ·
  <a href="https://ko-fi.com/flameswornstudio">Sostieni il progetto</a> ·
  <a href="https://github.com/matteo-ferranti/AetherForge/issues">Segnala un problema</a> ·
  <a href="README.md">English</a>
</p>

---

## Panoramica

AetherForge riunisce la conversione dei file e strumenti pratici di modifica in un'unica applicazione desktop. Permette di elaborare singoli file o più file insieme, con un'interfaccia disponibile in italiano e inglese.

L'elaborazione avviene localmente sul computer. Non è necessario caricare i file su un servizio di conversione online. L'installer include il runtime .NET e i componenti utilizzati dall'applicazione per elaborare i file.

## Ultimo aggiornamento — 1.1.0

**La versione 1.1.0** aggiunge OCR, conversioni immagini/PDF, PDF → DOCX, avanzamento dell'elaborazione e controllo aggiornamenti.

- OCR automatico in italiano e inglese per PDF → TXT sulle pagine prive di testo estraibile, anche nei PDF misti.
- Immagini → PDF, con esportazione separata o unione in un unico PDF e controlli per ordinare e rimuovere le immagini.
- PDF → PNG/JPG, con esportazione di una pagina o di un intervallo consecutivo in una cartella dedicata.
- PDF → DOCX tramite l'importazione PDF di LibreOffice già incluso, per conservare testo, immagini e impaginazione quando possibile.
- Pannello di elaborazione in sovraimpressione con progresso per pagina per PDF → TXT, avanzamento complessivo del batch e indicatore di attività per le altre conversioni.
- Menu con freccetta per applicare un'operazione o un formato di output a tutti i file compatibili in attesa.
- Nuova identità FlameSworn Studio, collegamenti al sito in base alla lingua e pulsante per sostenere il progetto su Ko-fi.
- Controllo aggiornamenti nelle Informazioni e all'avvio, con preferenza salvata per disattivare i controlli automatici.

Lo **[storico completo delle versioni](CHANGELOG.it.md)** è disponibile nel changelog.

## Funzionalità

| Categoria | Strumenti disponibili |
| --- | --- |
| Immagini | Conversione, ridimensionamento, rotazione, capovolgimento, regolazione della qualità, rimozione dei metadati per i formati supportati ed esportazione in PDF separati o in un unico documento ordinato. |
| Audio | Conversione, ritaglio, regolazione del bitrate per i formati supportati e rimozione dei metadati. |
| Video | Conversione, ritaglio, estrazione e rimozione dell'audio, creazione di GIF e rimozione dei metadati. |
| Documenti di testo | Conversione tra i formati di testo e Office supportati, estrazione del testo dai PDF con OCR automatico italiano/inglese sulle pagine prive di testo estraibile ed esportazione in PDF. |
| Fogli di calcolo | Conversione tra i formati supportati, esportazione in PDF e in CSV con selezione del foglio. I CSV in ingresso possono essere esportati in PDF. |
| Presentazioni | Conversione tra PPT, PPTX e ODP ed esportazione in PDF. |
| E-book | Conversione di EPUB in TXT e PDF. |
| Strumenti PDF | Unione di file nell'ordine scelto, estrazione di pagine o intervalli, esportazione in PNG/JPG e conversione PDF → DOCX con testo, immagini e impaginazione. |

L'elenco dei file offre inoltre trascinamento, scelta dell'operazione e del formato di output per ciascun file, stato dell'elaborazione e pulsanti per aprire i risultati completati o mostrarli nella cartella. Le freccette accanto ai selettori di operazione e formato permettono di applicare una scelta a tutti i file compatibili in attesa.

## Screenshot

Gli screenshot mostrano la versione 1.0.0. La 1.1.0 aggiorna l'interfaccia e aggiunge gli strumenti elencati sopra.

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
| Immagini | JPG, PNG, WebP, GIF, BMP, TIFF, AVIF, HEIC, HEIF, ICO, SVG | PDF, JPG, PNG, WebP, BMP, TIFF, AVIF |
| PDF | PDF | TXT (con OCR automatico quando necessario), DOCX, PNG, JPG |
| Audio | MP3, WAV, FLAC, AAC, M4A, OGG, OPUS, WMA, AIFF, AC3 | MP3, WAV, FLAC, AAC, M4A, OGG, OPUS |
| Video | MP4, MKV, AVI, MOV, WebM, WMV, FLV, MPEG, TS | MP4, MKV, MOV, WebM, AVI |
| Documenti | PDF, TXT, DOC, DOCX, ODT, RTF, HTML, EPUB | I formati disponibili dipendono dal file in ingresso |
| Fogli di calcolo | XLS, XLSX, ODS, CSV | I formati disponibili dipendono dal file in ingresso |
| Presentazioni | PPT, PPTX, ODP | PDF e formati di presentazione supportati |

Questi elenchi non indicano che ogni formato possa essere convertito in tutti gli altri. L'applicazione mostra le operazioni e i formati disponibili per ciascun file in ingresso.

## Installazione

**Piattaforma di destinazione:** Windows 11 con processore Intel o AMD x64.

[**Scarica AetherForge 1.1.0 — Installer Windows x64**](https://github.com/matteo-ferranti/AetherForge/releases/download/v1.1.0/AetherForge-Setup-1.1.0-win-x64.exe)

1. Scarica l'installer tramite il collegamento sopra.
2. Avvia `AetherForge-Setup-1.1.0-win-x64.exe`.
3. Seleziona italiano o inglese e segui la procedura. L'inglese è selezionato per impostazione predefinita.
4. Apri AetherForge dal menu Start o dal collegamento desktop facoltativo.

L'installazione riguarda l'utente corrente e non richiede privilegi di amministratore. Non occorre installare separatamente .NET, FFmpeg, LibreOffice o qpdf.

Per disinstallare, usa **Impostazioni → App → App installate → AetherForge**.

L'installer attuale è destinato a Windows 11 x64. La compatibilità con Windows 10 e ARM non è stata verificata.

I [sorgenti e gli script di compilazione di FFmpeg](https://github.com/matteo-ferranti/AetherForge/releases/download/v1.0.0/AetherForge-FFmpeg-Sources-20261009.zip) sono disponibili separatamente e non servono per utilizzare l'applicazione.

## Primi passi

### Aggiornamenti

Dalla versione 1.1.0, usa **Controlla aggiornamenti** nella scheda Informazioni oppure lascia attivo **Controlla aggiornamenti all'avvio**. La scelta viene ricordata tra gli avvii. Il controllo contatta GitHub e richiede internet; l'elaborazione dei file resta locale e funziona offline. Se una nuova release stabile contiene l'installer Windows x64, l'app chiede se aprire la pagina ufficiale. Download e installazione restano sotto il tuo controllo.

Esegui il nuovo installer nella cartella dell'installazione esistente per aggiornare; non occorre disinstallare prima. La 1.0.0 non ha il controllo aggiornamenti, quindi il primo aggiornamento va scaricato manualmente. All'avvio, l'assenza di connessione non produce avvisi; il controllo manuale segnala gli errori di connessione.

### Elaborazione dei file

1. Trascina i file nella finestra principale oppure usa **Seleziona file**.
2. Scegli un'operazione e un formato di output disponibile per ciascun file.
   Usa la freccetta accanto al selettore per applicare la scelta a tutti i file compatibili in attesa.
3. Imposta le eventuali opzioni richieste dall'operazione.
4. Avvia l'elaborazione e scegli dove salvare i risultati.
5. Controlla lo stato dei file, poi apri il risultato o la cartella che lo contiene.

L'unione PDF utilizza una finestra dedicata, in cui puoi aggiungere i file, modificarne l'ordine e scegliere la destinazione.

Quando converti più immagini in PDF, scegli se esportarle separatamente o unirle in un unico documento, quindi imposta l'ordine. Per PDF → PNG/JPG, seleziona una pagina o un intervallo consecutivo; più pagine vengono esportate in una cartella dedicata. PDF → TXT utilizza automaticamente l'OCR sulle pagine prive di testo estraibile.

Per PDF → DOCX, scegli DOCX come formato di uscita. Il convertitore importa i contenuti delle pagine in Word, comprese le immagini e il testo modificabile quando il PDF contiene un livello di testo. Il pannello di elaborazione mostra il file corrente e il progresso complessivo. PDF → TXT indica anche le pagine completate; le operazioni prive di una percentuale interna mostrano un indicatore di attività fino al termine.

## Protezioni e limitazioni

AetherForge include controlli contro la sovrascrittura dei file in ingresso, scritture temporanee per i flussi supportati, limiti sulle risorse per immagini e archivi e restrizioni su alcune risorse esterne e sui contenuti attivi dei documenti.

Questi controlli non costituiscono una sandbox completa né garantiscono la sicurezza di qualsiasi file in ingresso.

- L'OCR riconosce localmente testi in italiano e inglese. Il risultato dipende dalla qualità della scansione; non garantisce un riconoscimento accurato della scrittura a mano né la conservazione di tabelle e impaginazione.
- L'OCR automatico si attiva sulle pagine prive di testo estraibile. Un'immagine contenente testo su una pagina che ha anche un livello di testo estraibile non viene riconosciuta separatamente.
- Immagini → PDF esporta il primo fotogramma delle GIF animate e la prima pagina dei TIFF multipagina.
- PDF → DOCX cerca di conservare l'aspetto delle pagine. Il testo può diventare caselle posizionate e le tabelle possono essere rappresentate da linee e caselle di testo anziché da tabelle native di Word. Le pagine scansionate possono restare immagini nel DOCX; l'OCR automatico attualmente riguarda PDF → TXT.
- La conversione di documenti Office e presentazioni può modificare impaginazione, font o funzioni specifiche dell'applicazione di origine.
- Le operazioni e le impostazioni di qualità variano in base al formato.
- La rimozione dei metadati è disponibile per i formati supportati e non equivale all'anonimizzazione completa del contenuto.
- File danneggiati, protetti o troppo grandi possono essere rifiutati.

## Verifica del rilascio

**1.0.0:** l'applicazione pubblicata e l'installer hanno superato le prove di conversione, installazione e disinstallazione sul computer di sviluppo. L'applicazione è stata provata con successo anche su un'installazione pulita di Windows 11.

**1.1.0:** OCR, conversioni immagini/PDF e applicazione delle scelte a più file hanno superato le prove funzionali sul computer di sviluppo. L'OCR è stato verificato anche in una compilazione Release con runtime incluso. PDF → DOCX e avanzamento dell'elaborazione hanno superato ulteriori controlli di sviluppo. Il risultato di PDF → DOCX resta dipendente dal documento di origine. Restano da verificare il nuovo installer e i nuovi componenti su un'installazione pulita di Windows 11; la prova della 1.0.0 su un sistema pulito non copre queste aggiunte.

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
├── qpdf/
│   └── bin/qpdf.exe
└── vcruntime/
    ├── msvcp140.dll
    ├── vcruntime140.dll
    └── vcruntime140_1.dll
```

Sono necessarie le distribuzioni complete di LibreOffice e qpdf, con i relativi file di supporto, non soltanto gli eseguibili. Queste cartelle non sono incluse nel repository.

NuGet ripristina PDFtoImage, PDFium, SkiaSharp e TesseractOCR, comprese le rispettive librerie native. I modelli OCR italiano e inglese si trovano in `src/AetherForge.App/Assets/OCR/tessdata` e vengono copiati nelle cartelle di compilazione e pubblicazione. Mantieni entrambi i file `eng.traineddata` e `ita.traineddata` insieme all'applicazione. Le DLL x64 del runtime Visual C++ in `tools/vcruntime` vengono copiate accanto all'eseguibile; la loro redistribuzione è soggetta ai [termini di licenza Microsoft](https://learn.microsoft.com/en-us/cpp/windows/redistributing-visual-cpp-files).

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
- **PDFtoImage / PDFium / SkiaSharp** per il rendering delle pagine PDF in immagini.
- **TesseractOCR / Tesseract / Leptonica** con i modelli **tessdata_fast** italiano e inglese per l'OCR.
- **Microsoft Visual C++ Runtime** per i componenti OCR nativi.
- **Open XML SDK** per le operazioni supportate sui documenti Office.
- **qpdf** per l'elaborazione dei PDF.
- **Noto Sans** per il testo nei documenti generati.

Il codice originale di AetherForge è distribuito con **[licenza MIT](LICENSE)**. I componenti di terze parti mantengono le proprie licenze e i rispettivi avvisi di copyright.

I testi delle licenze e gli avvisi sono disponibili in `src/AetherForge.App/Assets/Licenses` e nella finestra **Informazioni e licenze** dell'applicazione. La redistribuzione dei componenti inclusi richiede anche il rispetto delle rispettive licenze, compresi gli eventuali obblighi relativi ai sorgenti corrispondenti.

## Autore e attribuzione

Realizzata da **Matteo Ferranti**, con il nome **[FlameSworn Studio](https://flameswornstudio.com/it/)**.

Se utilizzi AetherForge o il suo codice nel tuo progetto, una citazione visibile dell'autore e un collegamento al repository sono apprezzati. Si tratta di una richiesta, non di una condizione aggiuntiva della licenza MIT.

## Segnalazioni e contributi

Se vuoi sostenere lo sviluppo, visita **[FlameSworn Studio su Ko-fi](https://ko-fi.com/flameswornstudio)**.

Suggerimenti, segnalazioni riproducibili e contributi sono benvenuti tramite [GitHub Issues](https://github.com/matteo-ferranti/AetherForge/issues) e pull request.

Quando segnali un problema, indica la versione di Windows, la versione di AetherForge, i formati di ingresso e uscita, l'operazione e il messaggio di errore. Condividi un file di esempio privo di dati sensibili soltanto se necessario.


