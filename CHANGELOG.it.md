# Changelog

[English](CHANGELOG.md) · [README italiano](README.it.md)

Lo storico è ordinato dalla versione più recente alla più vecchia.

## [1.1.0]

### Aggiunto

- OCR automatico per PDF → TXT, con modelli italiano e inglese utilizzati localmente. Le pagine con testo estraibile mantengono l'estrazione normale; quelle prive di testo estraibile utilizzano l'OCR, anche nei PDF misti.
- PDF → PNG/JPG: esportazione di una pagina o di un intervallo consecutivo. Più pagine esportate vengono salvate in una cartella dedicata.
- PDF → DOCX tramite il filtro di importazione PDF di LibreOffice già incluso, conservando testo, immagini e aspetto delle pagine quando possibile. I margini importati vengono normalizzati per evitare lo spostamento dei contenuti in Word.
- Pannello di elaborazione in sovraimpressione con conteggio pagine e progresso per PDF → TXT, avanzamento complessivo del batch e indicatore di attività per le operazioni prive di una percentuale interna.
- Immagini → PDF: esportazione delle immagini separatamente o unione in un unico PDF, con controlli per ordinare e rimuovere le immagini prima dell'elaborazione.
- Menu con freccetta accanto ai selettori di operazione e formato per applicare una scelta a tutti i file compatibili in attesa.
- Pulsante Ko-fi con l'icona della tazza nella scheda Informazioni.
- Controllo aggiornamenti manuale nelle Informazioni e automatico all'avvio, attivo per impostazione predefinita e disattivabile con preferenza salvata. Le nuove versioni stabili con installer Windows x64 caricato propongono di aprire la pagina ufficiale GitHub; gli errori di connessione all'avvio restano silenziosi.

### Modificato

- Le operazioni immagini ancora sincrone vengono eseguite fuori dal thread dell'interfaccia per mantenere reattivo il pannello di elaborazione.
- Dimensioni iniziali più ampie per la finestra principale e le Informazioni, limitate allo spazio disponibile sullo schermo.
- Il formato di output selezionato viene conservato, quando ancora supportato, durante l'aggiornamento dei formati disponibili. Le conversioni supportate possono mantenere il formato di ingresso salvando in un file diverso.
- Il pulsante del sito include l'icona FlameSworn e apre il nuovo sito di AetherForge nella lingua selezionata.
- Il piè di pagina della finestra principale mostra la versione e il logo FlameSworn Studio cliccabile, che apre il sito dello studio in italiano o inglese.
- Versione dell'applicazione e dell'installer aggiornata a 1.1.0.
- Documentazione italiana e inglese aggiornata per OCR, conversioni immagini/PDF, applicazione delle scelte a più file e nuovo sito.

### Distribuzione

- Librerie per rendering PDF e OCR, modelli di riconoscimento italiano/inglese e DLL x64 del runtime Visual C++ richieste inclusi nelle cartelle di compilazione/pubblicazione.
- Testi delle licenze e attribuzioni dei nuovi componenti aggiunti all'elenco delle licenze nell'app.

### Limitazioni note

- PDF → DOCX cerca di conservare l'aspetto delle pagine: il testo può diventare caselle posizionate e le tabelle possono non essere tabelle native di Word. Le scansioni possono restare immagini; l'OCR automatico riguarda PDF → TXT.
- Il progresso viene indicato per pagina per PDF → TXT e per operazione nei batch. Le altre conversioni mostrano l'attività senza stimare una percentuale interna.

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

[1.1.0]: https://github.com/matteo-ferranti/AetherForge/releases/tag/v1.1.0
[1.0.0]: https://github.com/matteo-ferranti/AetherForge/releases/tag/v1.0.0
