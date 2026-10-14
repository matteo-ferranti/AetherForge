# Changelog

[English](CHANGELOG.md) · [README italiano](README.it.md)

Lo storico è ordinato dalla versione più recente alla più vecchia. Le voci non ancora pubblicate descrivono la versione in sviluppo, non l'installer disponibile per il download.

## [1.1.0] — Non ancora pubblicata

### Aggiunto

- OCR automatico per PDF → TXT, con modelli italiano e inglese utilizzati localmente. Le pagine con testo estraibile mantengono l'estrazione normale; quelle prive di testo estraibile utilizzano l'OCR, anche nei PDF misti.
- PDF → PNG/JPG: esportazione di una pagina o di un intervallo consecutivo. Più pagine esportate vengono salvate in una cartella dedicata.
- Immagini → PDF: esportazione delle immagini separatamente o unione in un unico PDF, con controlli per ordinare e rimuovere le immagini prima dell'elaborazione.
- Menu con freccetta accanto ai selettori di operazione e formato per applicare una scelta a tutti i file compatibili in attesa.
- Pulsante Ko-fi con l'icona della tazza nella scheda Informazioni.

### Modificato

- Il formato di output selezionato viene conservato, quando ancora supportato, durante l'aggiornamento dei formati disponibili. Le conversioni supportate possono mantenere il formato di ingresso salvando in un file diverso.
- Il pulsante del sito include l'icona FlameSworn e apre il nuovo sito di AetherForge nella lingua selezionata.
- Il piè di pagina della finestra principale mostra la versione e il logo FlameSworn Studio cliccabile, che apre il sito dello studio in italiano o inglese.
- Versione dell'applicazione e dell'installer aggiornata a 1.1.0.
- Documentazione italiana e inglese aggiornata per OCR, conversioni immagini/PDF, applicazione delle scelte a più file e nuovo sito.

### Distribuzione

- Librerie per rendering PDF e OCR, modelli di riconoscimento italiano/inglese e DLL x64 del runtime Visual C++ richieste inclusi nelle cartelle di compilazione/pubblicazione.
- Testi delle licenze e attribuzioni dei nuovi componenti aggiunti all'elenco delle licenze nell'app.

## [1.0.0] — Prima release pubblica

- Conversione, ridimensionamento, rotazione, capovolgimento, regolazione della qualità e rimozione dei metadati delle immagini per i formati supportati.
- Conversione audio, ritaglio, regolazione del bitrate e rimozione dei metadati.
- Conversione video, ritaglio, estrazione/rimozione dell'audio, creazione di GIF e rimozione dei metadati.
- Conversione di documenti, fogli di calcolo, presentazioni, CSV ed EPUB, con esportazione in PDF e selezione del foglio per l'esportazione in CSV.
- Estrazione PDF → TXT per i PDF contenenti un livello di testo; questa versione non includeva l'OCR.
- Unione PDF con trascinamento e controlli sull'ordine, oltre all'estrazione delle pagine.
- Elaborazione singola e batch, interfaccia italiano/inglese, trascinamento, stati dell'elaborazione e pulsanti per aprire i risultati o mostrarli nella cartella.
- Protezione dei file in ingresso dalla sovrascrittura, scritture temporanee, limiti sulle risorse per immagini e archivi e restrizioni sulle risorse esterne e sui contenuti attivi dei documenti.
- Schede Informazioni e licenze, licenza MIT per il codice originale di AetherForge e attribuzioni dei componenti inclusi.
- Installer Windows 11 x64 con runtime incluso, procedura italiano/inglese, installazione per utente e programma di disinstallazione.
- README italiano/inglese, screenshot dell'app e archivio separato con sorgenti e script di compilazione di FFmpeg.

[1.0.0]: https://github.com/matteo-ferranti/AetherForge/releases/tag/v1.0.0
