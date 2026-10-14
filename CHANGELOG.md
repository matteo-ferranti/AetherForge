# Changelog

[Italiano](CHANGELOG.it.md) · [README](README.md)

Version history is listed from newest to oldest.

## [1.1.0]

### Added

- Automatic OCR for PDF → TXT, using English and Italian recognition models locally. Pages with extractable text retain normal text extraction; pages without it use OCR, including in mixed PDFs.
- PDF → PNG/JPG: export one page or a consecutive page range. Multiple exported pages are saved in a dedicated folder.
- PDF → DOCX through the bundled LibreOffice PDF import filter, retaining text, images, and visual layout where possible. Imported page margins are normalized to avoid shifting the content in Word.
- A processing overlay with page counts and progress for PDF → TXT, overall batch progress, and an activity indicator for operations without internal progress reporting.
- Images → PDF: export individual images separately or combine multiple images in a single PDF, with controls to reorder and remove images before processing.
- Arrow menus beside operation and output selectors to apply a choice to all compatible pending files.
- A Ko-fi support button with its cup icon in the Information tab.
- A manual update check in the Information tab and an automatic startup check, enabled by default and switchable off with a saved preference. New stable versions with an uploaded Windows x64 installer prompt to open the official GitHub release page; offline startup failures remain silent.

### Changed

- Remaining synchronous image operations run off the UI thread to keep the processing overlay responsive.
- Larger default main and About windows, constrained to the available screen area.
- Output format selections are preserved when still supported after refreshing the available formats. Supported conversions can keep the input format while saving to a different file.
- The website button now includes the FlameSworn icon and opens the new AetherForge website in the selected language.
- The main window footer shows the version and a clickable FlameSworn Studio logo, opening the studio website in English or Italian.
- Application and installer version updated to 1.1.0.
- English and Italian documentation updated for OCR, image/PDF conversion, bulk selection, and the new website.

### Distribution

- PDF rendering and OCR libraries, English/Italian recognition models, and the required x64 Visual C++ runtime DLLs are included in build/publish output.
- Third-party license texts and notices added to the application's license list for the new components.

### Known limitations

- PDF → DOCX is a best-effort visual conversion: text can become positioned boxes and tables may not become native Word tables. Scanned pages can remain images; automatic OCR applies to PDF → TXT.
- Progress is reported per page for PDF → TXT and per operation for batches. Other conversions show activity without an estimated internal percentage.

## [1.0.0] — First public release

- Image conversion, resizing, rotation, flipping, quality adjustment, and metadata removal for supported formats.
- Audio conversion, trimming, bitrate adjustment, and metadata removal.
- Video conversion, trimming, audio extraction/removal, GIF creation, and metadata removal.
- Document, spreadsheet, presentation, CSV, and EPUB conversion workflows, including PDF export and spreadsheet worksheet selection for CSV export.
- PDF → TXT extraction for PDFs containing a text layer; OCR was not included in this release.
- PDF merging with drag and drop and ordering controls, plus page extraction.
- Single-file and batch processing, English/Italian interface, drag and drop, processing statuses, and shortcuts to open results or show them in their folder.
- Input file overwrite protection, temporary output staging, resource limits for images and archives, and restrictions on external resources and active document content.
- Information and license tabs, MIT license for AetherForge's original code, and bundled third-party notices.
- A self-contained Windows 11 x64 installer with English and Italian setup, per-user installation, and an uninstaller.
- English/Italian README files, application screenshots, and a separate FFmpeg source/build-script archive.

[1.1.0]: https://github.com/matteo-ferranti/AetherForge/releases/tag/v1.1.0
[1.0.0]: https://github.com/matteo-ferranti/AetherForge/releases/tag/v1.0.0
