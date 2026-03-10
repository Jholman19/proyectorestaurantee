# RestauranteApp.Device

Implementaciones de servicios de hardware para el sistema RestauranteApp.

## 📁 Contenido

### Interfaces
- **`IFingerprintService`** - Interfaz para servicios de lectura de huellas dactilares

### Implementaciones

#### 1. FingerprintSimulator.cs ✅ ACTIVO POR DEFECTO
Simulador de lector de huellas para desarrollo y pruebas.
- No requiere hardware
- Permite simular enrollments y escaneos
- Útil para desarrollo sin el dispositivo físico

#### 2. HIDUareUService.cs 🔧 REQUIERE CONFIGURACIÓN
Implementación real para el lector **HID U.are.U 4500**.
- Requiere hardware conectado
- Requiere SDK de DigitalPersona instalado
- Para producción con lector físico

## 🔄 Cambiar entre Simulador y Dispositivo Real

### Usar Simulador (Por defecto)
En `RestauranteApp\App.xaml.cs`:
```csharp
services.AddSingleton<IFingerprintService, RestauranteApp.Device.FingerprintSimulator>();
```

### Usar Dispositivo Real
1. Instala el driver y SDK (ver INTEGRACION_LECTOR_HUELLAS.md)
2. Configura RestauranteApp.Device.csproj (descomenta referencias)
3. En `RestauranteApp\App.xaml.cs`:
```csharp
services.AddSingleton<IFingerprintService, RestauranteApp.Device.HIDUareUService>();
```

## 📖 Documentación Completa

Ver archivo raíz: **`INTEGRACION_LECTOR_HUELLAS.md`**

## 🛠️ Verificar Instalación del SDK

Ejecuta el script desde la raíz del proyecto:
```powershell
.\VerificarSDK-Huellero.ps1
```

Este script verifica:
- ✓ Driver del lector instalado
- ✓ SDK de DigitalPersona instalado
- ✓ DLLs disponibles
- ✓ Genera configuración para .csproj
