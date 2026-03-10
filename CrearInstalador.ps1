# ============================================================
# Script de CrearInstalador.ps1
# Automatiza la publicación y creación del instalador
# ============================================================

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " CREADOR DE INSTALADOR - MI RESTAURANTE" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

# Ruta base del proyecto
$ProjectRoot = Split-Path -Parent $PSScriptRoot
Set-Location $ProjectRoot

Write-Host "📁 Directorio del proyecto: $ProjectRoot" -ForegroundColor Yellow
Write-Host ""

# Paso 1: Limpiar compilaciones anteriores
Write-Host "🧹 Paso 1: Limpiando compilaciones anteriores..." -ForegroundColor Green
try {
    dotnet clean --configuration Release
    Write-Host "   ✅ Limpieza completada" -ForegroundColor Green
} catch {
    Write-Host "   ⚠️ Error al limpiar: $_" -ForegroundColor Yellow
}
Write-Host ""

# Paso 2: Restaurar paquetes
Write-Host "📦 Paso 2: Restaurando paquetes NuGet..." -ForegroundColor Green
try {
    dotnet restore
    Write-Host "   ✅ Paquetes restaurados" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Error al restaurar paquetes: $_" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Paso 3: Compilar en Release
Write-Host "🔨 Paso 3: Compilando en modo Release..." -ForegroundColor Green
try {
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        throw "Error en la compilación"
    }
    Write-Host "   ✅ Compilación exitosa" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Error al compilar: $_" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Paso 4: Publicar aplicación
Write-Host "📤 Paso 4: Publicando aplicación (self-contained)..." -ForegroundColor Green
try {
    dotnet publish RestauranteApp\RestauranteApp.csproj `
        -c Release `
        -r win-x64 `
        --self-contained true `
        -p:PublishSingleFile=false `
        -p:PublishReadyToRun=true
    
    if ($LASTEXITCODE -ne 0) {
        throw "Error en la publicación"
    }
    Write-Host "   ✅ Publicación completada" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Error al publicar: $_" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Paso 5: Verificar Inno Setup
Write-Host "🔍 Paso 5: Verificando Inno Setup..." -ForegroundColor Green
$InnoSetupPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"

if (-not (Test-Path $InnoSetupPath)) {
    Write-Host "   ❌ Inno Setup no encontrado en: $InnoSetupPath" -ForegroundColor Red
    Write-Host "   📥 Descargue e instale desde: https://jrsoftware.org/isdl.php" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Presione cualquier tecla para continuar sin crear instalador..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}
Write-Host "   ✅ Inno Setup encontrado" -ForegroundColor Green
Write-Host ""

# Paso 6: Compilar instalador
Write-Host "📦 Paso 6: Compilando instalador con Inno Setup..." -ForegroundColor Green
try {
    Set-Location "$ProjectRoot\installer"
    
    & $InnoSetupPath "RestauranteApp_Simple.iss" 2>&1 | Out-Null
    
    if ($LASTEXITCODE -ne 0) {
        throw "Error al compilar instalador"
    }
    Write-Host "   ✅ Instalador compilado exitosamente" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Error al compilar instalador: $_" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Paso 7: Verificar resultado
Write-Host "✅ Paso 7: Verificando instalador generado..." -ForegroundColor Green
$InstallerPath = "$ProjectRoot\installer\Output\MiRestaurante_Setup_v1.0.0.exe"

if (Test-Path $InstallerPath) {
    $FileInfo = Get-Item $InstallerPath
    $SizeMB = [math]::Round($FileInfo.Length / 1MB, 2)
    
    Write-Host "   ✅ Instalador creado correctamente" -ForegroundColor Green
    Write-Host ""
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host " INSTALADOR GENERADO EXITOSAMENTE" -ForegroundColor Green
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "📁 Ubicación:" -ForegroundColor Yellow
    Write-Host "   $InstallerPath" -ForegroundColor White
    Write-Host ""
    Write-Host "📊 Tamaño: $SizeMB MB" -ForegroundColor Yellow
    Write-Host "📅 Fecha: $($FileInfo.LastWriteTime)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host ""
    
    # Opción de abrir carpeta
    $Response = Read-Host "¿Desea abrir la carpeta del instalador? (S/N)"
    if ($Response -eq "S" -or $Response -eq "s") {
        explorer.exe "$ProjectRoot\installer\Output"
    }
    
    Write-Host ""
    Write-Host "🎉 ¡Proceso completado! El instalador está listo para distribuir." -ForegroundColor Green
    
} else {
    Write-Host "   ❌ No se encontró el instalador en la ubicación esperada" -ForegroundColor Red
    Write-Host "   Revise los errores anteriores" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Presione cualquier tecla para salir..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
