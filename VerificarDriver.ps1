# Script para verificar si el driver ya esta instalado
Write-Host "=== VERIFICACION DRIVER LECTOR DE HUELLAS ===" -ForegroundColor Cyan
Write-Host ""

# 1. Verificar dispositivo USB conectado
Write-Host "1. Buscando lectores conectados..." -ForegroundColor Yellow
$devices = Get-PnpDevice | Where-Object { $_.FriendlyName -like "*U.are.U*" -or $_.FriendlyName -like "*DigitalPersona*" -or $_.FriendlyName -like "*fingerprint*" }

if ($devices) {
    Write-Host "   [OK] Dispositivo(s) encontrado(s):" -ForegroundColor Green
    foreach ($dev in $devices) {
        $status = if ($dev.Status -eq "OK") { "[OK]" } else { "[ERROR] " + $dev.Status }
        Write-Host "     - $($dev.FriendlyName) [$status]" -ForegroundColor $(if ($dev.Status -eq "OK") { "Green" } else { "Red" })
    }
} else {
    Write-Host "   [X] No se encontro el lector" -ForegroundColor Red
    Write-Host "     Conecta el lector USB y espera unos segundos" -ForegroundColor Yellow
}

Write-Host ""

# 2. Verificar SDK instalado
Write-Host "2. Verificando SDK instalado..." -ForegroundColor Yellow

$uareuDllCandidates = @(
    "C:\Program Files\DigitalPersona\U.are.U SDK\DPUruNet.dll",
    "C:\Program Files (x86)\DigitalPersona\U.are.U SDK\DPUruNet.dll",
    "C:\Program Files\HID Global\U.are.U SDK\DPUruNet.dll",
    "C:\Program Files (x86)\HID Global\U.are.U SDK\DPUruNet.dll"
)

$oneTouchDllCandidates = @(
    "C:\Program Files\DigitalPersona\One Touch SDK\.NET\Bin\DPFPDevNET.dll",
    "C:\Program Files\DigitalPersona\One Touch SDK\.NET\Bin\DPFPEngNET.dll",
    "C:\Program Files\DigitalPersona\One Touch SDK\.NET\Bin\DPFPShrNET.dll",
    "C:\Program Files\DigitalPersona\One Touch SDK\.NET\Bin\DPFPVerNET.dll"
)

$sdkFound = $false
$activeSdk = $null

$uareuFound = $uareuDllCandidates | Where-Object { Test-Path $_ }
if ($uareuFound.Count -gt 0) {
    Write-Host "   [OK] SDK U.are.U detectado" -ForegroundColor Green
    foreach ($dll in $uareuFound) {
        Write-Host "     [OK] $dll" -ForegroundColor Green
    }
    $sdkFound = $true
    $activeSdk = "UAREU"
}

$oneTouchFound = $oneTouchDllCandidates | Where-Object { Test-Path $_ }
if ($oneTouchFound.Count -gt 0) {
    Write-Host "   [OK] SDK One Touch detectado" -ForegroundColor Green
    foreach ($dll in $oneTouchFound) {
        Write-Host "     [OK] $dll" -ForegroundColor Green
    }
    $sdkFound = $true
    if (-not $activeSdk) { $activeSdk = "ONETOUCH" }
}

if (-not $sdkFound) {
    Write-Host "   [X] SDK no encontrado" -ForegroundColor Red
    Write-Host "     Necesitas descargar e instalar el SDK" -ForegroundColor Yellow
}

Write-Host ""

# 3. Verificar drivers nativos
Write-Host "3. Verificando DLLs nativas del sistema..." -ForegroundColor Yellow
$nativeDlls = @("dpfpdd.dll", "dpfj.dll", "DPJasPer.dll")
$nativePaths = @(
    "$env:SystemRoot\System32",
    "$env:SystemRoot\SysWOW64",
    "C:\Program Files\DigitalPersona\Bin",
    "C:\Program Files (x86)\DigitalPersona\Bin"
)

foreach ($dll in $nativeDlls) {
    $found = $false
    foreach ($basePath in $nativePaths) {
        $fullPath = Join-Path $basePath $dll
        if (Test-Path $fullPath) {
            Write-Host "   [OK] $dll encontrado en $basePath" -ForegroundColor Green
            $found = $true
            break
        }
    }
    if (-not $found) {
        Write-Host "   [X] $dll no encontrado" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== RESUMEN ===" -ForegroundColor Cyan

if ($devices -and $sdkFound) {
    if ($activeSdk -eq "ONETOUCH") {
        Write-Host "[OK] Todo listo (One Touch SDK) - app compatible" -ForegroundColor Green
    } else {
        Write-Host "[OK] Todo listo (U.are.U SDK) - app compatible" -ForegroundColor Green
    }
} elseif ($devices -and -not $sdkFound) {
    Write-Host "[ADVERTENCIA] Driver OK pero falta SDK" -ForegroundColor Yellow
} elseif (-not $devices) {
    Write-Host "[ERROR] Conecta el lector USB primero" -ForegroundColor Red
} else {
    Write-Host "[ADVERTENCIA] Verifica instalacion completa" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Presiona Enter para cerrar..."
Read-Host
