#define MyAppName "RestauranteApp"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "RestauranteApp"
#define MyAppExeName "RestauranteApp.exe"
#define MyPublishDir "C:\Users\jholm\RestauranteApp\RestauranteApp\bin\Release\net8.0-windows\win-x64\publish"

[Setup]
AppId={{E2B4B8D1-4A91-4C2E-A1F5-9D9F7A0A1234}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=C:\Users\jholm\RestauranteApp\installer\Output
OutputBaseFilename=RestauranteAppSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "Crear ícono en el Escritorio"; GroupDescription: "Accesos directos:"; Flags: unchecked

[Files]
Source: "{#MyPublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Abrir {#MyAppName}"; Flags: nowait postinstall skipifsilent