; ============================================================
; INSTALADOR DE MI RESTAURANTE - VERSION SIMPLIFICADA
; Sistema de Gestión de Restaurante
; Versión 1.0.0
; ============================================================

#define MyAppName "Mi Restaurante"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Mi Amigo el Pizzero"
#define MyAppExeName "RestauranteApp.exe"
#define MyPublishDir "..\RestauranteApp\bin\Release\net8.0-windows\win-x64\publish"

[Setup]
AppId={{E2B4B8D1-4A91-4C2E-A1F5-9D9F7A0A1234}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}

; Directorios
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=Output
OutputBaseFilename=MiRestaurante_Setup_v{#MyAppVersion}

; Compresión
Compression=lzma2
SolidCompression=yes

; Apariencia
WizardStyle=modern
PrivilegesRequired=admin
DisableProgramGroupPage=yes

; Información de versión
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=Sistema de Gestión de Restaurante

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[CustomMessages]
spanish.WelcomeLabel2=Esto instalará [name/ver] en su equipo.%n%nSistema de Gestión de Restaurante con:%n• Control de asistencia con huella dactilar%n• Gestión de suscripciones y combos%n• Control automático de créditos%n• Reportes y estadísticas%n%nSe recomienda cerrar todas las aplicaciones antes de continuar.

[Tasks]
Name: "desktopicon"; Description: "Crear ícono en el Escritorio"; GroupDescription: "Accesos directos:"

[Dirs]
Name: "{localappdata}\{#MyAppName}"; Permissions: users-full
Name: "{localappdata}\{#MyAppName}\Logs"; Permissions: users-full
Name: "{localappdata}\{#MyAppName}\Database"; Permissions: users-full

[Files]
; Archivos principales
Source: "{#MyPublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; Documentación
Source: "..\README.md"; DestDir: "{app}\Docs"; Flags: ignoreversion
Source: "..\GUIA_RAPIDA.md"; DestDir: "{app}\Docs"; Flags: ignoreversion
Source: "..\VERIFICACION_CREDITOS.md"; DestDir: "{app}\Docs"; Flags: ignoreversion
Source: "..\GUIA_PRUEBAS_CREDITOS.md"; DestDir: "{app}\Docs"; Flags: ignoreversion

[Icons]
; Menú de inicio
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "Sistema de Gestión de Restaurante"
Name: "{group}\Documentación"; Filename: "{app}\Docs"; Comment: "Ver documentación"
Name: "{group}\Desinstalar"; Filename: "{uninstallexe}"

; Escritorio
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; Comment: "Sistema de Gestión de Restaurante"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Abrir {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\{#MyAppName}\Logs"
