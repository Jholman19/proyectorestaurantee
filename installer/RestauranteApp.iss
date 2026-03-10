; ============================================================
; INSTALADOR DE MI RESTAURANTE
; Sistema de Gestión de Restaurante con Control de Huellas
; Versión 1.0.0
; ============================================================

#define MyAppName "Mi Restaurante"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Mi Amigo el Pizzero"
#define MyAppExeName "RestauranteApp.exe"
#define MyAppURL "https://www.mirestaurante.com"
#define MyPublishDir "..\RestauranteApp\bin\Release\net8.0-windows\win-x64\publish"

[Setup]
; ============================================================
; CONFIGURACIÓN BÁSICA
; ============================================================
AppId={{E2B4B8D1-4A91-4C2E-A1F5-9D9F7A0A1234}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

; ============================================================
; DIRECTORIOS
; ============================================================
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=Output
OutputBaseFilename=RestauranteApp_Setup_v{#MyAppVersion}
; SetupIconFile=..\RestauranteApp\icon.ico

; ============================================================
; CONFIGURACIÓN DE COMPRESIÓN
; ============================================================
Compression=lzma2/ultra64
SolidCompression=yes
LZMAUseSeparateProcess=yes
LZMANumBlockThreads=2

; ============================================================
; APARIENCIA Y PRIVILEGIOS
; ============================================================
WizardStyle=modern
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\{#MyAppExeName}

; ============================================================
; INFORMACIÓN DE LA VERSIÓN
; ============================================================
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=Sistema de Gestión de Restaurante
VersionInfoCopyright=Copyright (C) 2026 {#MyAppPublisher}
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}

; ============================================================
; ARCHIVOS DE LICENCIA Y DOCUMENTACIÓN
; ============================================================
LicenseFile=..\README.md
InfoBeforeFile=..\GUIA_RAPIDA.md

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[CustomMessages]
spanish.WelcomeLabel2=Esto instalará [name/ver] en su equipo.%n%nSistema de Gestión de Restaurante con:%n• Control de asistencia con huella dactilar%n• Gestión de suscripciones y combos%n• Control automático de créditos%n• Reportes y estadísticas%n%nSe recomienda cerrar todas las aplicaciones antes de continuar.

[Tasks]
Name: "desktopicon"; Description: "Crear ícono en el Escritorio"; GroupDescription: "Accesos directos adicionales:"
Name: "quicklaunchicon"; Description: "Crear ícono en Inicio Rápido"; GroupDescription: "Accesos directos adicionales:"; Flags: unchecked

[Dirs]
Name: "{commonappdata}\{#MyAppName}"; Permissions: users-full
Name: "{commonappdata}\{#MyAppName}\Logs"; Permissions: users-full
Name: "{commonappdata}\{#MyAppName}\Database"; Permissions: users-full
Name: "{commonappdata}\{#MyAppName}\Backups"; Permissions: users-full

[Files]
; Archivos principales de la aplicación
Source: "{#MyPublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; Documentación
Source: "..\README.md"; DestDir: "{app}\Docs"; Flags: ignoreversion
Source: "..\GUIA_RAPIDA.md"; DestDir: "{app}\Docs"; Flags: ignoreversion
Source: "..\DOCUMENTACION.md"; DestDir: "{app}\Docs"; Flags: ignoreversion isreadme
Source: "..\INTEGRACION_LECTOR_HUELLAS.md"; DestDir: "{app}\Docs"; Flags: ignoreversion
Source: "..\VERIFICACION_CREDITOS.md"; DestDir: "{app}\Docs"; Flags: ignoreversion
Source: "..\GUIA_PRUEBAS_CREDITOS.md"; DestDir: "{app}\Docs"; Flags: ignoreversion
Source: "..\VerificarCreditos.sql"; DestDir: "{app}\Docs"; Flags: ignoreversion

[Icons]
; Acceso directo en el menú de inicio
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "Sistema de Gestión de Restaurante"
Name: "{group}\Documentación"; Filename: "{app}\Docs\DOCUMENTACION.md"; Comment: "Manual de usuario"
Name: "{group}\Guía Rápida"; Filename: "{app}\Docs\GUIA_RAPIDA.md"; Comment: "Guía de uso rápido"
Name: "{group}\Desinstalar {#MyAppName}"; Filename: "{uninstallexe}"

; Acceso directo en el escritorio
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; Comment: "Sistema de Gestión de Restaurante"

; Acceso directo en inicio rápido
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
; Ejecutar la aplicación al finalizar
Filename: "{app}\{#MyAppExeName}"; Description: "Abrir {#MyAppName}"; Flags: nowait postinstall skipifsilent

; Abrir la documentación
Filename: "{app}\Docs\GUIA_RAPIDA.md"; Description: "Ver Guía Rápida"; Flags: postinstall shellexec skipifsilent unchecked

[UninstallDelete]
Type: filesandordirs; Name: "{commonappdata}\{#MyAppName}\Logs"

[Code]
// ============================================================
// CÓDIGO PERSONALIZADO
// ============================================================

procedure InitializeWizard();
var
  WelcomePage: TWizardPage;
begin
  // Personalización adicional aquí si es necesario
end;

function InitializeSetup(): Boolean;
var
  ResultCode: Integer;
  ResultStr: String;
begin
  Result := True;
  
  // Verificar si ya existe una versión instalada
  if RegKeyExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{E2B4B8D1-4A91-4C2E-A1F5-9D9F7A0A1234}_is1') then
  begin
    if MsgBox('Ya existe una instalación de Mi Restaurante.'+#13#10+#13#10+
              '¿Desea desinstalar la versión anterior antes de continuar?', 
              mbConfirmation, MB_YESNO) = IDYES then
    begin
      // Ejecutar desinstalador silencioso
      RegQueryStringValue(HKEY_LOCAL_MACHINE, 
        'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{E2B4B8D1-4A91-4C2E-A1F5-9D9F7A0A1234}_is1',
        'UninstallString', ResultStr);
      if ResultStr <> '' then
      begin
        Exec(RemoveQuotes(ResultStr), '/SILENT', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      end;
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Crear base de datos inicial si no existe
    // La aplicación lo hará automáticamente la primera vez
  end;
end;

function InitializeUninstall(): Boolean;
var
  Response: Integer;
begin
  Result := True;
  
  Response := MsgBox('¿Desea conservar la base de datos y los archivos de configuración?'+#13#10+#13#10+
                     'Si selecciona NO, se eliminarán TODOS los datos del sistema.'+#13#10+
                     'Si selecciona SÍ, podrá reinstalar la aplicación sin perder datos.', 
                     mbConfirmation, MB_YESNO or MB_DEFBUTTON1);
  
  if Response = IDNO then
  begin
    // El usuario quiere eliminar todo
    DelTree(ExpandConstant('{commonappdata}\{#MyAppName}'), True, True, True);
  end;
end;