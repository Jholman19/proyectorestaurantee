# Script de verificación para SDK de HID U.are.U 4500
# Verifica la instalación del SDK y los archivos necesarios

Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "  VERIFICACIÓN DE SDK HID U.are.U 4500" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

$sdkPath = "C:\Program Files\DigitalPersona\U.are.U SDK"
$sdkPathx86 = "C:\Program Files (x86)\DigitalPersona\U.are.U SDK"
$driverInstalled = $false
$sdkFound = $false

# Verificar driver del lector en Administrador de Dispositivos
Write-Host "[1/4] Verificando driver del dispositivo..." -ForegroundColor Yellow
try {
    $devices = Get-PnpDevice | Where-Object { $_.FriendlyName -like "*U.are.U*" }
    if ($devices) {
        Write-Host "  ✓ Driver encontrado: " -ForegroundColor Green -NoNewline
        foreach ($device in $devices) {
            Write-Host "$($device.FriendlyName)" -ForegroundColor White
            Write-Host "    Estado: $($device.Status)" -ForegroundColor Gray
        }
        $driverInstalled = $true
    } else {
        Write-Host "  ✗ No se encontró el driver HID U.are.U" -ForegroundColor Red
        Write-Host "    Descarga el driver desde: https://www.hidglobal.com/drivers" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  ⚠ No se pudo verificar (requiere permisos)" -ForegroundColor Yellow
}
Write-Host ""

# Verificar instalación del SDK
Write-Host "[2/4] Verificando SDK instalado..." -ForegroundColor Yellow
if (Test-Path $sdkPath) {
    Write-Host "  ✓ SDK encontrado en: $sdkPath" -ForegroundColor Green
    $sdkFound = $true
    $finalPath = $sdkPath
} elseif (Test-Path $sdkPathx86) {
    Write-Host "  ✓ SDK encontrado en: $sdkPathx86" -ForegroundColor Green
    $sdkFound = $true
    $finalPath = $sdkPathx86
} else {
    Write-Host "  ✗ SDK no encontrado" -ForegroundColor Red
    Write-Host "    Instala el SDK desde el instalador del driver" -ForegroundColor Yellow
}
Write-Host ""

# Verificar archivos DLL necesarios
if ($sdkFound) {
    Write-Host "[3/4] Verificando archivos DLL..." -ForegroundColor Yellow
    
    $requiredDlls = @(
        "DPUruNet.dll",
        "dpfpdd.dll",
        "dpfj.dll"
    )
    
    $allFound = $true
    foreach ($dll in $requiredDlls) {
        $dllPath = Join-Path $finalPath $dll
        if (Test-Path $dllPath) {
            Write-Host "  ✓ $dll" -ForegroundColor Green
        } else {
            Write-Host "  ✗ $dll (NO ENCONTRADO)" -ForegroundColor Red
            $allFound = $false
        }
    }
    
    Write-Host ""
    
    # Sugerir configuración del .csproj
    if ($allFound) {
        Write-Host "[4/4] Configuración del proyecto..." -ForegroundColor Yellow
        Write-Host "  Las DLLs están disponibles. Actualiza RestauranteApp.Device.csproj:" -ForegroundColor Green
        Write-Host ""
        Write-Host "  <ItemGroup>" -ForegroundColor Cyan
        Write-Host "    <Reference Include=`"DPUruNet`">" -ForegroundColor Cyan
        Write-Host "      <HintPath>$finalPath\DPUruNet.dll</HintPath>" -ForegroundColor Cyan
        Write-Host "      <Private>True</Private>" -ForegroundColor Cyan
        Write-Host "    </Reference>" -ForegroundColor Cyan
        Write-Host "  </ItemGroup>" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "  <ItemGroup>" -ForegroundColor Cyan
        Write-Host "    <None Include=`"$finalPath\dpfpdd.dll`">" -ForegroundColor Cyan
        Write-Host "      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>" -ForegroundColor Cyan
        Write-Host "    </None>" -ForegroundColor Cyan
        Write-Host "    <None Include=`"$finalPath\dpfj.dll`">" -ForegroundColor Cyan
        Write-Host "      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>" -ForegroundColor Cyan
        Write-Host "    </None>" -ForegroundColor Cyan
        Write-Host "  </ItemGroup>" -ForegroundColor Cyan
        Write-Host ""
    } else {
        Write-Host "  ⚠ Faltan archivos DLL necesarios" -ForegroundColor Yellow
    }
} else {
    Write-Host "[3/4] OMITIDO - SDK no encontrado" -ForegroundColor Gray
    Write-Host "[4/4] OMITIDO - SDK no encontrado" -ForegroundColor Gray
}

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "  RESUMEN" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan

if ($driverInstalled -and $sdkFound -and $allFound) {
    Write-Host "  ✓ LISTO PARA INTEGRAR" -ForegroundColor Green
    Write-Host ""
    Write-Host "  Próximos pasos:" -ForegroundColor White
    Write-Host "  1. Actualiza RestauranteApp.Device.csproj con las rutas mostradas arriba" -ForegroundColor White
    Write-Host "  2. En App.xaml.cs, cambia FingerprintSimulator por HIDUareUService" -ForegroundColor White
    Write-Host "  3. Compila el proyecto: dotnet build -c Release" -ForegroundColor White
    Write-Host "  4. Prueba el enrollment desde la ventana de Suscripciones" -ForegroundColor White
} else {
    Write-Host "  ⚠ CONFIGURACIÓN INCOMPLETA" -ForegroundColor Yellow
    Write-Host ""
    if (-not $driverInstalled) {
        Write-Host "  • Instala el driver del dispositivo" -ForegroundColor Yellow
    }
    if (-not $sdkFound) {
        Write-Host "  • Instala el SDK de DigitalPersona U.are.U" -ForegroundColor Yellow
    }
    Write-Host ""
    Write-Host "  Consulta: INTEGRACION_LECTOR_HUELLAS.md" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

# Ofrecer copiar configuración al portapapeles
if ($sdkFound -and $allFound) {
    $response = Read-Host "¿Copiar configuración del .csproj al portapapeles? (S/N)"
    if ($response -eq "S" -or $response -eq "s") {
        $config = @"
  <ItemGroup>
    <Reference Include="DPUruNet">
      <HintPath>$finalPath\DPUruNet.dll</HintPath>
      <Private>True</Private>
    </Reference>
  </ItemGroup>

  <ItemGroup>
    <None Include="$finalPath\dpfpdd.dll">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Include="$finalPath\dpfj.dll">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
"@
        Set-Clipboard -Value $config
        Write-Host "✓ Configuración copiada al portapapeles" -ForegroundColor Green
        Write-Host "  Pégala en RestauranteApp.Device\RestauranteApp.Device.csproj" -ForegroundColor White
    }
}
