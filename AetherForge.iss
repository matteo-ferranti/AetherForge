#define AppName "AetherForge"
#define AppVersion "1.0.0"
#define AppPublisher "Matteo Ferranti"
#define AppExeName "AetherForge.App.exe"

[Setup]
AppId=MatteoFerranti.AetherForge
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL=https://aetherforgeconverter.pages.dev/en/
AppSupportURL=https://github.com/matteo-ferranti/AetherForge
AppUpdatesURL=https://aetherforgeconverter.pages.dev/en/

DefaultDirName={localappdata}\Programs\AetherForge
PrivilegesRequired=lowest

ArchitecturesAllowed=x64compatible and not arm64
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0.22000

OutputDir=artifacts\installer
OutputBaseFilename=AetherForge-Setup-{#AppVersion}-win-x64

SetupIconFile=src\AetherForge.App\Assets\Icons\AetherForge.ico
UninstallDisplayIcon={app}\{#AppExeName}
LicenseFile=artifacts\release\win-x64\Assets\Licenses\AetherForge-LICENSE.txt

Compression=lzma2
SolidCompression=yes
WizardStyle=modern dynamic
LanguageDetectionMethod=none
UsePreviousLanguage=no

DisableProgramGroupPage=yes
CloseApplications=yes
RestartApplications=no
Uninstallable=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"

[CustomMessages]
english.CreateDesktopShortcut=Create a desktop shortcut
italian.CreateDesktopShortcut=Crea un collegamento sul desktop
english.LaunchApp=Launch AetherForge
italian.LaunchApp=Avvia AetherForge

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopShortcut}"; Flags: unchecked

[Files]
Source: "artifacts\release\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"; WorkingDir: "{app}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; WorkingDir: "{app}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchApp}"; Flags: nowait postinstall skipifsilent