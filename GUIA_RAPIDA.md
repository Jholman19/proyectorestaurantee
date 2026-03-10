# 📌 Guía Rápida - RestauranteApp

## ⚡ Inicio Rápido

### Instalar y Ejecutar
```powershell
# Compilar
dotnet build -c Release

# Ejecutar
cd RestauranteApp\bin\Release\net8.0-windows
.\RestauranteApp.exe
```

### Operaciones Básicas

#### 1. Crear Combo
```
Menú → Combos → Nuevo Combo
- Nombre: "Completo"
- Marcar: Desayuno + Almuerzo + Cena
- Guardar
```

#### 2. Registrar Cliente
```
Menú → Clientes → Nuevo Cliente
- Nombre: "Juan Pérez"
- Guardar
```

#### 3. Crear Suscripción
```
Menú → Suscripciones
- Seleccionar Cliente
- Seleccionar Combo
- Duración: 30 días
- Crear Suscripción
```

#### 4. Registrar Consumo
```
Método 1 - Manual:
  - Seleccionar cliente
  - Clic en botón: Desayuno/Almuerzo/Cena

Método 2 - Huella:
  - Iniciar Escucha
  - Cliente coloca dedo
  - Sistema registra automáticamente
```

---

## 🕐 Horarios

| Comida | Inicio | Fin |
|--------|--------|-----|
| Desayuno | 07:00 | 09:30 |
| Almuerzo | 11:00 | 16:00 |
| Cena | 18:00 | 21:00 |

---

## 🔧 Configuración del Lector HID U.are.U 4500

### Paso 1: Instalar SDK
```
https://www.hidglobal.com/drivers
→ U.are.U SDK for Windows
```

### Paso 2: Verificar
```powershell
.\VerificarSDK-Huellero.ps1
```

### Paso 3: Activar en Código
**RestauranteApp.Device\RestauranteApp.Device.csproj**
```xml
<!-- Descomentar -->
<DefineConstants>USE_HID_SDK</DefineConstants>
```

**RestauranteApp\App.xaml.cs** (línea ~111)
```csharp
// Cambiar de:
services.AddSingleton<IFingerprintService, FingerprintSimulator>();
// A:
services.AddSingleton<IFingerprintService, HIDUareUService>();
```

### Paso 4: Compilar
```powershell
dotnet build -c Release
```

---

## 📂 Archivos Importantes

```
📁 Base de Datos:
   C:\Users\[Usuario]\AppData\Local\RestauranteApp\restaurante.db

📁 Logs:
   C:\Users\[Usuario]\AppData\Local\RestauranteApp\app.log

📁 SDK Lector:
   C:\Program Files\DigitalPersona\U.are.U SDK\
```

---

## 🆘 Problemas Comunes

### "No se encontró el lector"
✅ Verificar en Administrador de Dispositivos
✅ Reinstalar driver
✅ Probar otro puerto USB

### "Error al compilar HIDUareUService"
✅ Comentar `<DefineConstants>USE_HID_SDK</DefineConstants>`
✅ Mantener simulador activo: `FingerprintSimulator`

### "Aplicación no inicia"
✅ Revisar `app.log`
✅ Verificar .NET 8 Runtime instalado

---

## 📖 Documentación Completa

- **README.md** - Documentación principal completa
- **INTEGRACION_LECTOR_HUELLAS.md** - Guía del lector de huellas
- **RestauranteApp.Device/README.md** - Información de hardware

---

## ✅ Checklist Pre-Producción

- [ ] Compilar en Release
- [ ] Probar crear cliente
- [ ] Probar crear combo
- [ ] Probar crear suscripción
- [ ] Probar registro manual
- [ ] Probar renovación
- [ ] (Opcional) Instalar SDK lector
- [ ] (Opcional) Probar enrollment
- [ ] (Opcional) Probar modo escucha
- [ ] Crear respaldo BD
- [ ] Generar instalador

---

**Versión**: 1.0  
**Última Actualización**: Marzo 2026
