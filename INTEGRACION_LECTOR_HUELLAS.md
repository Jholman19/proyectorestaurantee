# Integración del Lector de Huellas HID U.are.U 4500

## 📋 Requisitos del Sistema

### Hardware
- **Lector de Huellas**: HID U.are.U 4500 USB-A (88003-001-S04)
- **Puerto**: USB 2.0 o superior
- **Sistema Operativo**: Windows 10/11 (64-bit)

### Software Requerido

#### 1. Driver del Dispositivo HID U.are.U 4500
Descarga e instala el driver desde:
- **Sitio Oficial**: [HID Global - U.are.U SDK](https://www.hidglobal.com/drivers)
- **Archivo**: U.are.U SDK Windows (última versión)

**Pasos de Instalación del Driver:**
1. Conecta el lector HID U.are.U 4500 al puerto USB
2. Ejecuta el instalador del driver como Administrador
3. Sigue las instrucciones del asistente
4. Reinicia el equipo si se solicita
5. Verifica en "Administrador de Dispositivos" que aparezca: `HID Global U.are.U 4500 Reader`

#### 2. SDK de DigitalPersona U.are.U (.NET)

El SDK se instala automáticamente con el driver, pero necesitas las DLLs para el proyecto:

**Ubicación típica del SDK:**
```
C:\Program Files\DigitalPersona\U.are.U SDK\
```

**Archivos DLL necesarios:**
- `DPUruNet.dll` - Biblioteca principal
- `dpfpdd.dll` - Driver de bajo nivel
- `dpfj.dll` - Biblioteca de coincidencia

---

## 🔧 Configuración del Proyecto

### Paso 1: Añadir Referencias al SDK

Debes añadir las referencias manualmente al proyecto `RestauranteApp.Device.csproj`:

```xml
<ItemGroup>
  <!-- SDK de DigitalPersona U.are.U -->
  <Reference Include="DPUruNet">
    <HintPath>C:\Program Files\DigitalPersona\U.are.U SDK\DPUruNet.dll</HintPath>
    <Private>True</Private>
  </Reference>
</ItemGroup>

<ItemGroup>
  <!-- Copiar DLLs nativas al directorio de salida -->
  <None Include="C:\Program Files\DigitalPersona\U.are.U SDK\dpfpdd.dll">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
  <None Include="C:\Program Files\DigitalPersona\U.are.U SDK\dpfj.dll">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

**⚠ NOTA:** Ajusta las rutas (`HintPath`) según la ubicación real del SDK en tu sistema.

### Paso 2: Activar el Servicio Real en App.xaml.cs

Abre `RestauranteApp\App.xaml.cs` y reemplaza la línea del simulador:

**Cambiar de:**
```csharp
services.AddSingleton<IFingerprintService, RestauranteApp.Device.FingerprintSimulator>();
```

**A:**
```csharp
services.AddSingleton<IFingerprintService, RestauranteApp.Device.HIDUareUService>();
```

### Paso 3: Compilar el Proyecto

```powershell
cd RestauranteApp
dotnet build -c Release
```

Si hay errores de compilación relacionados con `DPUruNet`, verifica:
- Que el SDK esté instalado
- Que las rutas en el .csproj sean correctas
- Que tengas permisos de lectura en la carpeta del SDK

---

## 🚀 Uso del Sistema con el Lector

### 1. Enrollment (Registrar Huella)

1. Abre la ventana de **Suscripciones**
2. Selecciona un **Cliente**
3. Haz clic en **"Registrar Huella"**
4. Sigue las instrucciones en pantalla:
   - Coloca el dedo 4 veces en el lector
   - Espera el mensaje de confirmación
5. La huella quedará asociada al cliente en la base de datos

### 2. Escucha Automática (Modo Asistencia)

1. En la ventana de **Suscripciones**, haz clic en **"Iniciar Escucha"**
2. El sistema queda en modo de escucha continua
3. Cuando un cliente coloca su dedo:
   - El sistema identifica automáticamente al cliente
   - Verifica el horario actual (Desayuno/Almuerzo/Cena)
   - Registra el consumo si está dentro del horario
   - Muestra notificación de éxito o error
4. Para detener: haz clic en **"Detener Escucha"**

### 3. Registro Manual

Si el lector falla o necesitas registrar manualmente:
1. Selecciona el cliente
2. Usa los botones **Desayuno / Almuerzo / Cena**
3. El sistema registra sin verificar horario (modo manual)

---

## 🔍 Solución de Problemas

### Problema: "No se encontró ningún lector de huellas"

**Soluciones:**
- Verifica que el lector esté conectado al USB
- Abre "Administrador de Dispositivos" y busca "HID Global U.are.U"
- Reinstala el driver si aparece con símbolo de advertencia
- Prueba en otro puerto USB

### Problema: "Error al inicializar el lector"

**Soluciones:**
- Cierra otras aplicaciones que puedan estar usando el lector
- Ejecuta la aplicación como Administrador
- Verifica que las DLLs nativas (dpfpdd.dll, dpfj.dll) estén en la carpeta de salida

### Problema: La huella no se reconoce

**Soluciones:**
- Re-enrolla la huella (el enrollment requiere 4 capturas de calidad)
- Limpia el sensor del lector con un paño suave
- Asegúrate de usar el mismo dedo con el que se enrolló
- Ajusta el `THRESHOLD` en HIDUareUService.cs (línea ~260)

### Problema: DLLs faltantes al ejecutar

**Soluciones:**
- Copia manualmente dpfpdd.dll y dpfj.dll a la carpeta `bin\Release\net8.0-windows\`
- Verifica que el .csproj tenga configurado `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>`

---

## 📊 Configuración Avanzada

### Ajustar Umbral de Coincidencia

En `HIDUareUService.cs`, línea ~260:

```csharp
const int THRESHOLD = 100000; // Valores más bajos = más estricto
```

- **Valores sugeridos:**
  - `80000` - Muy estricto (puede rechazar huellas válidas)
  - `100000` - Recomendado (balance entre seguridad y usabilidad)
  - `150000` - Más permisivo (mayor tasa de falsos positivos)

### Cambiar Número de Capturas para Enrollment

En `HIDUareUService.cs`, línea ~80:

```csharp
for (int i = 0; i < 4; i++) // Cambiar 4 por el número deseado
```

- **Mínimo:** 2 capturas (menos seguro)
- **Recomendado:** 4 capturas
- **Máximo:** 6 capturas (mejor calidad, más tiempo)

---

## 📝 Notas Adicionales

### Licencias y SDK
- El SDK de DigitalPersona/HID Global es gratuito para desarrollo
- Verifica los términos de licencia para uso comercial en: https://www.hidglobal.com

### Compatibilidad
- Esta implementación funciona con los modelos:
  - U.are.U 4500
  - U.are.U 5100
  - U.are.U 5160
- Otros modelos pueden requerir ajustes en el código

### Almacenamiento de Plantillas
- Las plantillas se guardan en la tabla `Clientes` (columna `HuellaTemplate`)
- Formato: Binario (byte[])
- Se cargan en memoria al iniciar la escucha
- Para mejor rendimiento con muchos clientes, considera indexar

---

## 📞 Soporte

Para problemas con:
- **Hardware/Drivers**: Contacta a HID Global Support
- **SDK**: Consulta la documentación en `C:\Program Files\DigitalPersona\U.are.U SDK\Documentation\`
- **Integración**: Revisa los logs en `%LocalAppData%\RestauranteApp\app.log`

---

## ✅ Checklist de Instalación

- [ ] Driver HID U.are.U instalado
- [ ] Lector visible en Administrador de Dispositivos
- [ ] SDK instalado en `C:\Program Files\DigitalPersona`
- [ ] Referencias agregadas al .csproj
- [ ] DLLs copiándose al directorio de salida
- [ ] App.xaml.cs actualizado con HIDUareUService
- [ ] Proyecto compilando sin errores
- [ ] Lector detectado al iniciar la aplicación
- [ ] Enrollment funcionando (4 capturas)
- [ ] Reconocimiento funcionando en modo escucha
