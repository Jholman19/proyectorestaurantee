# 🍽️ RestauranteApp - Sistema de Gestión de Comidas

Sistema completo de gestión de clientes, suscripciones y control de consumo de alimentos con soporte para lector de huellas dactilares.

---

## 📋 Tabla de Contenidos

- [Descripción General](#-descripción-general)
- [Características Principales](#-características-principales)
- [Requisitos del Sistema](#-requisitos-del-sistema)
- [Instalación y Configuración](#-instalación-y-configuración)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Uso del Sistema](#-uso-del-sistema)
- [Integración del Lector de Huellas](#-integración-del-lector-de-huellas)
- [Base de Datos](#-base-de-datos)
- [Horarios Configurados](#-horarios-configurados)
- [Guía de Desarrollo](#-guía-de-desarrollo)
- [Solución de Problemas](#-solución-de-problemas)

---

## 🎯 Descripción General

**RestauranteApp** es un sistema de escritorio desarrollado en **WPF (.NET 8)** para la gestión integral de un restaurante o comedor institucional. Permite:

- Registro y gestión de clientes
- Creación y administración de combos de comida
- Sistema de suscripciones con créditos por tipo de comida
- Control de asistencia y consumo diario
- Integración con lector biométrico de huellas (HID U.are.U 4500)
- Registro manual y automático de consumos
- Avisos de ausencia para preservar créditos
- Procesamiento automático de pérdidas por inasistencia

---

## ✨ Características Principales

### 1. Gestión de Clientes
- Alta, baja y modificación de clientes
- Almacenamiento de plantillas de huellas dactilares
- Búsqueda y filtrado de clientes
- Vista detallada de historial de consumo

### 2. Combos de Comida
- Creación de combos personalizados
- Configuración de tipos de comida incluidos:
  - ☕ Desayuno
  - 🍽️ Almuerzo  
  - 🌙 Cena
- Activación/desactivación de combos
- Precios y descripciones

### 3. Sistema de Suscripciones
- Creación de suscripciones con duración configurable (1-365 días)
- Asignación automática de créditos por tipo de comida
- Proceso de renovación simplificado
- Seguimiento de créditos restantes por tipo de comida
- Visualización de estado de suscripción (activa/vencida)
- Procesamiento automático de vencimientos

### 4. Control de Consumo
- **Registro Manual**: Desde interfaz gráfica con validación de horario
- **Registro Automático**: Mediante lector de huellas dactilares
- **Máximo 2 consumos** por tipo de comida al día
- Verificación de créditos disponibles
- Historial completo de consumos

### 5. Sistema de Avisos
- Marcar ausencias anticipadas
- Preservación de créditos cuando se avisa
- Registro de ausencias por tipo de comida
- Prevención de descuento automático

### 6. Pérdidas Automáticas
- Descuento automático de créditos por inasistencia sin aviso
- Aplica solo para desayuno y cena
- Procesamiento al finalizar horario establecido
- Se ejecuta automáticamente al iniciar la aplicación

### 7. Integración Biométrica
- Soporte para lector **HID U.are.U 4500**
- Enrollment (registro) de huellas con 4 capturas
- Reconocimiento automático en modo escucha
- Simulador para desarrollo sin hardware

---

## 💻 Requisitos del Sistema

### Hardware Mínimo
- **Procesador**: Intel Core i3 o equivalente
- **RAM**: 4 GB
- **Disco**: 500 MB libres
- **Display**: 1280x720 o superior
- **Puerto USB**: Para lector de huellas (opcional)

### Hardware Recomendado
- **Lector de Huellas**: HID U.are.U 4500 USB-A (88003-001-S04)
  - Conexión: USB 2.0+
  - Resolución: 512 DPI
  - Certificación: FBI PIV-071006

### Software
- **Sistema Operativo**: Windows 10/11 (64-bit)
- **.NET Runtime**: 8.0 o superior
- **Base de Datos**: SQLite (incluida)

### Software Adicional (Para Lector de Huellas)
- **Driver HID U.are.U**: Instalador del fabricante
- **SDK DigitalPersona**: U.are.U SDK para Windows
- Descargar desde: https://www.hidglobal.com/drivers

---

## 🚀 Instalación y Configuración

### Opción 1: Instalador (Recomendado)

1. **Descargar el instalador**:
   ```
   installer/Output/RestauranteAppSetup.exe
   ```

2. **Ejecutar como Administrador**

3. **Seguir el asistente de instalación**:
   - Aceptar licencia
   - Elegir directorio de instalación
   - Crear acceso directo en escritorio
   - Finalizar

4. **Iniciar la aplicación** desde el acceso directo

### Opción 2: Compilar desde Código Fuente

#### Prerrequisitos
- **.NET 8 SDK** instalado
- Visual Studio 2022 o VS Code
- Git (opcional)

#### Pasos

1. **Clonar o descargar el repositorio**:
   ```powershell
   git clone <repository-url>
   cd RestauranteApp
   ```

2. **Restaurar dependencias**:
   ```powershell
   dotnet restore
   ```

3. **Compilar el proyecto**:
   ```powershell
   dotnet build -c Release
   ```

4. **Ejecutar la aplicación**:
   ```powershell
   cd RestauranteApp/bin/Release/net8.0-windows
   .\RestauranteApp.exe
   ```

### Configuración Inicial

#### 1. Primera Ejecución
- La base de datos se crea automáticamente en:
  ```
  C:\Users\[Usuario]\AppData\Local\RestauranteApp\restaurante.db
  ```
- Las migraciones se aplican automáticamente
- Los logs se guardan en:
  ```
  C:\Users\[Usuario]\AppData\Local\RestauranteApp\app.log
  ```

#### 2. Crear Combos
1. Desde el menú principal, clic en **"Combos"**
2. Hacer clic en **"Nuevo Combo"**
3. Completar información:
   - Nombre del combo
   - Precio (opcional)
   - Seleccionar tipos de comida incluidos
4. Guardar

#### 3. Registrar Clientes
1. Desde el menú principal, clic en **"Clientes"**
2. Hacer clic en **"Nuevo Cliente"**
3. Ingresar datos:
   - Nombre completo
   - Información de contacto (opcional)
4. Guardar

#### 4. Crear Suscripciones
1. Desde el menú principal, clic en **"Suscripciones"**
2. Seleccionar un cliente
3. Seleccionar un combo
4. Establecer fecha de inicio y duración (días)
5. Confirmar creación
6. El sistema asigna automáticamente los créditos correspondientes

---

## 📁 Estructura del Proyecto

```
RestauranteApp/
│
├── RestauranteApp/                    # Proyecto principal (WPF)
│   ├── App.xaml / App.xaml.cs        # Configuración de la aplicación
│   ├── MainWindow.xaml               # Ventana principal
│   ├── ClienteForm.xaml              # Formulario de clientes
│   ├── ComboForm.xaml                # Formulario de combos
│   ├── CombosWindow.xaml             # Gestión de combos
│   ├── SuscripcionesWindow.xaml      # Gestión de suscripciones y consumos
│   ├── Models/                       # Modelos de vista
│   └── Resources/                    # Recursos y estilos
│
├── RestauranteApp.Core/               # Lógica de negocio
│   ├── ConsumoService.cs             # Servicio de consumos
│   ├── SuscripcionesService.cs       # Servicio de suscripciones
│   ├── AsistenciaRulesService.cs     # Reglas de horarios
│   └── IFingerprintService.cs        # Interfaz del lector de huellas
│
├── RestauranteApp.Data/               # Capa de datos
│   ├── AppDbContext.cs               # Contexto de Entity Framework
│   ├── AppDbContextFactory.cs        # Factory para migraciones
│   ├── Entities/                     # Entidades del modelo
│   │   ├── Cliente.cs
│   │   ├── Combo.cs
│   │   ├── Suscripcion.cs
│   │   ├── Consumo.cs
│   │   └── Aviso.cs
│   └── Migrations/                   # Migraciones de base de datos
│
├── RestauranteApp.Device/            # Servicios de hardware
│   ├── FingerprintSimulator.cs      # Simulador (desarrollo)
│   ├── HIDUareUService.cs           # Implementación real del lector
│   └── README.md
│
├── installer/                        # Instalador Inno Setup
│   ├── RestauranteApp.iss
│   └── Output/
│
├── INTEGRACION_LECTOR_HUELLAS.md    # Guía de integración del lector
├── VerificarSDK-Huellero.ps1        # Script de verificación del SDK
└── README.md                         # Este archivo
```

---

## 📖 Uso del Sistema

### Ventana Principal

Al iniciar la aplicación, se presenta el menú principal con las siguientes opciones:

- **👥 Clientes**: Gestión de clientes
- **🍱 Combos**: Administración de combos de comida
- **📋 Suscripciones**: Control de suscripciones y registro de consumo

### Módulo de Clientes

#### Crear Cliente
1. Clic en **"Nuevo Cliente"**
2. Completar formulario:
   - Nombre (obligatorio)
   - Información adicional (opcional)
3. Guardar

#### Editar Cliente
1. Seleccionar cliente en la lista
2. Clic en **"Editar"**
3. Modificar información
4. Guardar cambios

#### Eliminar Cliente
1. Seleccionar cliente
2. Clic en **"Eliminar"**
3. Confirmar eliminación

⚠️ **Nota**: No se pueden eliminar clientes con suscripciones activas

### Módulo de Combos

#### Crear Combo
1. Clic en **"Nuevo Combo"**
2. Ingresar información:
   - **Nombre**: Identificador del combo
   - **Precio**: Costo del combo (opcional)
   - **Tipos de comida**: Marcar checkboxes
     - ☑️ Desayuno
     - ☑️ Almuerzo
     - ☑️ Cena
3. Estado: **Activo** (por defecto)
4. Guardar

#### Editar Combo
1. Seleccionar combo en la lista
2. Clic en **"Editar"**
3. Modificar campos necesarios
4. Guardar

#### Desactivar Combo
1. Editar el combo
2. Desmarcar **"Activo"**
3. Guardar

ℹ️ Los combos inactivos no aparecen al crear nuevas suscripciones

### Módulo de Suscripciones

Esta es la ventana más completa del sistema. Desde aquí se gestionan:
- Creación de suscripciones
- Registro de consumos (manual o biométrico)
- Avisos de ausencia
- Renovaciones
- Visualización de créditos y consumos

#### Crear Suscripción

1. **Seleccionar Cliente** del ComboBox superior
2. **Seleccionar Combo** disponible
3. **Fecha de Inicio**: Por defecto es hoy, puede ser futura
4. **Duración**: Ingresar número de días (1-365)
5. Clic en **"Crear Suscripción"**
6. Revisar el resumen mostrado:
   - Fechas de inicio y fin
   - Créditos a asignar por tipo de comida
   - Total de créditos
7. Confirmar

**Ejemplo**:
- Cliente: Juan Pérez
- Combo: "Completo" (Desayuno + Almuerzo + Cena)
- Inicio: 01/03/2026
- Duración: 30 días
- **Resultado**: 
  - 30 créditos de desayuno
  - 30 créditos de almuerzo
  - 30 créditos de cena

#### Ver Información de Suscripción

Al seleccionar un cliente, se muestra automáticamente:
- Combo activo
- Tipos de comida incluidos
- Fechas de inicio y fin
- Duración total
- Estado (Activo/Inactivo)

En la sección de **Créditos Restantes**:
```
CRÉDITOS POR COMIDA:
Desayuno:  25 restantes
Almuerzo:  28 restantes
Cena:      27 restantes
```

#### Registrar Consumo Manual

**Escenario**: La persona viene a comer y el operador registra manualmente.

1. **Seleccionar el cliente** de la lista
2. **Verificar** que tenga suscripción activa
3. Hacer clic en el botón correspondiente:
   - **🌅 Desayuno**
   - **☀️ Almuerzo**
   - **🌙 Cena**
4. El sistema muestra una notificación:
   - ✅ **"DENTRO DE HORARIO"** si está en el horario correcto
   - ⚠️ **"FUERA DE HORARIO"** si está fuera (aun así registra)
5. Se descuenta 1 crédito automáticamente
6. La tabla de consumos se actualiza

**Validaciones automáticas**:
- Verifica que el combo incluya ese tipo de comida
- Verifica que queden créditos disponibles
- Verifica que no se hayan alcanzado los 2 consumos máximos del día
- Muestra mensaje de error si alguna validación falla

#### Registrar Consumo con Lector de Huellas

**Escenario**: Sistema automatizado sin intervención del operador.

**Paso 1 - Enrollment (Registro inicial de huella)**:

1. Seleccionar el cliente en la interfaz
2. Clic en **"Registrar Huella"**
3. Seguir instrucciones en pantalla:
   - Colocar el dedo **4 veces** en el lector
   - Esperar entre cada captura
4. Mensaje: **"✓ Huella registrada y guardada"**
5. La plantilla se almacena en la base de datos

**Paso 2 - Modo Escucha (Operación normal)**:

1. Clic en **"Iniciar Escucha"**
2. El sistema entra en modo de detección continua
3. Estado cambia a: **"Escuchando huella... Acerca la huella al lector"**
4. Cuando una persona coloca su dedo:
   - El sistema identifica automáticamente al cliente
   - Determina el tipo de comida según la hora actual
   - Registra el consumo
   - Muestra notificación: **"✓ Consumido: cliente [ID] - [Tipo]"**
5. Para detener: **"Detener Escucha"**

**Horarios automáticos**:
- 07:00-09:30 → Registra Desayuno
- 11:00-16:00 → Registra Almuerzo
- 18:00-21:00 → Registra Cena
- Fuera de horario → Muestra advertencia pero no registra

#### Marcar Ausencia (Avisos)

**Escenario**: El cliente sabe que no vendrá mañana y quiere preservar su crédito.

1. Seleccionar el cliente
2. **Seleccionar la fecha** en el DatePicker (por defecto: hoy)
3. Clic en el botón de ausencia correspondiente:
   - **"Ausencia Desayuno"**
   - **"Ausencia Almuerzo"**
   - **"Ausencia Cena"**
4. Confirmar la acción
5. Mensaje: **"✓ Ausencia registrada"**

**Efecto**:
- No se descuenta crédito cuando pase el horario
- El sistema registra el aviso en la tabla `Avisos`
- Útil para vacaciones, días libres, etc.

#### Renovar Suscripción

**Escenario**: La suscripción está por vencer o ya venció, y el cliente desea continuar.

1. Seleccionar el cliente con suscripción a renovar
2. En la sección **"Renovación"**:
   - Ingresar duración en días (1-365)
   - Por defecto: 30 días
3. Clic en **"Renovar"**
4. Revisar el resumen:
   - Combo que se renovará (el mismo anterior)
   - Duración nueva
   - Créditos que se asignarán
5. Confirmar

**Efecto**:
- La suscripción anterior se marca como inactiva
- Se crea una nueva suscripción con fecha de inicio = hoy
- Los créditos se reinician al máximo según duración
- El cliente vuelve a tener acceso completo

#### Procesar Vencimientos

Clic en **"Procesar Vencimientos"** para:
- Desactivar suscripciones que hayan alcanzado su fecha de fin
- Ejecuta el proceso para todas las suscripciones
- Se ejecuta automáticamente al iniciar la aplicación

#### Ver Consumos del Día

En la parte inferior de la ventana:

1. **Selector de Fecha**: Permite elegir qué día consultar (por defecto: hoy)
2. **Tabla de Consumos**: Muestra todos los consumos del cliente en ese día
   - Tipo de comida
   - Fecha
3. **Resumen**: Estadísticas del día
   ```
   CONSUMOS HOY (03/03/2026):
   Desayunos: 1 | Almuerzos: 1 | Cenas: 0
   ```

---

## 🔐 Integración del Lector de Huellas

### Dispositivo Soportado

**HID U.are.U 4500 USB-A Fingerprint Reader**
- Modelo: 88003-001-S04
- Conexión: USB 2.0+
- Resolución: 512 DPI
- Tecnología: Óptica
- Certificación: FBI PIV-071006
- Dimensiones: 6.5cm x 3.6cm x 1.6cm
- Material: Plástico ABS resistente al polvo

### Instalación del Hardware

#### 1. Instalar Driver y SDK

**Descarga**:
```
https://www.hidglobal.com/drivers
```

Buscar: **"U.are.U SDK for Windows"**

**Instalación**:
1. Ejecutar el instalador como Administrador
2. Aceptar términos de licencia
3. Elegir instalación completa (Driver + SDK)
4. Reiniciar si se solicita

**Verificar instalación**:
1. Abrir **Administrador de Dispositivos** (devmgmt.msc)
2. Buscar en **Dispositivos biométricos**:
   - Debe aparecer: `HID Global U.are.U 4500 Reader`
3. Estado debe ser: **"Este dispositivo funciona correctamente"**

#### 2. Verificar SDK Instalado

Ejecutar el script de verificación incluido:

```powershell
cd C:\Users\jholm\RestauranteApp
.\VerificarSDK-Huellero.ps1
```

**El script verifica**:
- ✅ Driver del lector instalado
- ✅ SDK en `C:\Program Files\DigitalPersona\U.are.U SDK`
- ✅ Archivos DLL necesarios:
  - `DPUruNet.dll` (Biblioteca .NET)
  - `dpfpdd.dll` (Driver nativo)
  - `dpfj.dll` (Matching/comparación)
- ✅ Genera configuración para el .csproj

**Si todo está OK**: El script muestra las líneas para copiar al archivo de proyecto.

#### 3. Configurar el Proyecto

**Editar**: `RestauranteApp.Device\RestauranteApp.Device.csproj`

Buscar la sección comentada y descomentarla:

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
  
  <!-- Descomentar para habilitar el lector real -->
  <DefineConstants>USE_HID_SDK</DefineConstants>
</PropertyGroup>

<!-- Descomentar este bloque -->
<ItemGroup>
  <Reference Include="DPUruNet">
    <HintPath>C:\Program Files\DigitalPersona\U.are.U SDK\DPUruNet.dll</HintPath>
    <Private>True</Private>
  </Reference>
</ItemGroup>

<ItemGroup>
  <None Include="C:\Program Files\DigitalPersona\U.are.U SDK\dpfpdd.dll">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
  <None Include="C:\Program Files\DigitalPersona\U.are.U SDK\dpfj.dll">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

⚠️ **Ajustar las rutas** si el SDK está en otra ubicación.

#### 4. Activar el Servicio Real

**Editar**: `RestauranteApp\App.xaml.cs`

**Línea ~111**, cambiar de:
```csharp
services.AddSingleton<IFingerprintService, RestauranteApp.Device.FingerprintSimulator>();
```

**A:**
```csharp
services.AddSingleton<IFingerprintService, RestauranteApp.Device.HIDUareUService>();
```

#### 5. Compilar y Probar

```powershell
dotnet clean
dotnet build -c Release
cd RestauranteApp/bin/Release/net8.0-windows
.\RestauranteApp.exe
```

### Uso del Lector

#### Registrar Huella (Enrollment)

1. Abrir **Ventana de Suscripciones**
2. Seleccionar el cliente
3. Clic en **"Registrar Huella"**
4. Seguir las instrucciones:
   - **Captura 1/4**: Coloca el dedo en el lector
   - Esperar mensaje de confirmación
   - **Captura 2/4**: Coloca el mismo dedo nuevamente
   - Repetir para capturas 3 y 4
5. Mensaje final: **"✓ Huella registrada y guardada en la base de datos"**

**Tips para buen enrollment**:
- Usar siempre el mismo dedo (recomendado: índice derecho)
- Limpiar el sensor antes de empezar
- Colocar el dedo completamente sobre el sensor
- Presionar con firmeza pero sin exceso
- Si falla una captura, reintentar con más presión o diferente ángulo

#### Modo Escucha (Identificación Automática)

1. Clic en **"Iniciar Escucha"**
2. El botón cambia a **"Detener Escucha"**
3. Estado: **"Escuchando huella... Acerca la huella al lector"**
4. El sistema detecta automáticamente cualquier huella
5. Al reconocer un cliente:
   - Determina el tipo de comida según horario
   - Registra el consumo
   - Muestra notificación
   - Descuenta el crédito

**Estados posibles**:
- ✅ **"✓ Consumido: cliente [ID] - [Tipo]"**: Registro exitoso
- ⚠️ **"Huella detectada pero no estamos en horario de comida"**: Fuera de horario
- ❌ **"Error: No quedan créditos disponibles"**: Sin créditos
- ❌ **"Error: Ya alcanzó el máximo de 2 consumos"**: Límite alcanzado

### Resolución de Problemas - Lector

#### Error: "No se encontró ningún lector de huellas"

**Causas posibles**:
- Lector no conectado
- Driver no instalado correctamente
- USB sin alimentación suficiente

**Soluciones**:
1. Verificar conexión física del USB
2. Abrir **Administrador de Dispositivos**:
   - Buscar `HID Global U.are.U`
   - Si tiene símbolo de advertencia: reinstalar driver
3. Probar en otro puerto USB (preferiblemente USB 3.0)
4. Reiniciar el equipo

#### Error: "Error al inicializar el lector"

**Causas posibles**:
- Otra aplicación está usando el lector
- Permisos insuficientes
- DLLs nativas no encontradas

**Soluciones**:
1. Cerrar otras aplicaciones que puedan usar el lector
2. Ejecutar RestauranteApp como Administrador
3. Verificar que `dpfpdd.dll` y `dpfj.dll` estén en la carpeta de la aplicación
4. Reinstalar el SDK

#### La huella no se reconoce

**Causas posibles**:
- Enrollment de baja calidad
- Sensor sucio
- Dedo mojado o sucio
- Umral muy estricto

**Soluciones**:
1. **Re-enrollar la huella**:
   - Eliminar el registro actual
   - Hacer nuevo enrollment asegurando buena calidad
2. **Limpiar el sensor**: Paño suave ligeramente húmedo
3. **Ajustar umbral de coincidencia**:
   - Editar `RestauranteApp.Device\HIDUareUService.cs`
   - Línea ~260: `const int THRESHOLD = 100000;`
   - Aumentar a `150000` para ser más permisivo
   - Recompilar

#### DLLs faltantes al ejecutar

**Error**: `System.DllNotFoundException: Unable to load DLL 'dpfpdd.dll'`

**Solución**:
1. Copiar manualmente las DLLs:
   ```
   Desde: C:\Program Files\DigitalPersona\U.are.U SDK\
   A:     C:\...\RestauranteApp\bin\Release\net8.0-windows\
   
   Archivos:
   - dpfpdd.dll
   - dpfj.dll
   ```
2. Verificar que el .csproj tenga `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>`

### Modo Simulador (Desarrollo sin Hardware)

Si no tienes el lector físico, el sistema funciona con el simulador:

**Características**:
- No requiere hardware
- Simula enrollments (genera plantillas ficticias)
- Permite pruebas de flujo completo
- Útil para desarrollo y demostraciones

**Activar simulador** (por defecto):
```csharp
// En App.xaml.cs
services.AddSingleton<IFingerprintService, RestauranteApp.Device.FingerprintSimulator>();
```

**Probar captura simulada**:
```csharp
// Desde código (solo para testing)
var simulator = (FingerprintSimulator)fingerprintService;
await simulator.SimulateScanAsync(clienteId: 1);
```

---

## 🗄️ Base de Datos

### Tecnología
- **Motor**: SQLite
- **ORM**: Entity Framework Core 8.0
- **Migraciones**: Code-First
- **Ubicación**: 
  ```
  C:\Users\[Usuario]\AppData\Local\RestauranteApp\restaurante.db
  ```

### Esquema de Tablas

#### Clientes
```sql
CREATE TABLE Clientes (
    ClienteId INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    HuellaTemplate BLOB NULL,  -- Plantilla biométrica
    -- Otros campos...
);
```

#### Combos
```sql
CREATE TABLE Combos (
    ComboId INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    Precio REAL NULL,
    Desayuno INTEGER NOT NULL,  -- Boolean (0/1)
    Almuerzo INTEGER NOT NULL,
    Cena INTEGER NOT NULL,
    Activo INTEGER NOT NULL DEFAULT 1
);
```

#### Suscripciones
```sql
CREATE TABLE Suscripciones (
    SuscripcionId INTEGER PRIMARY KEY AUTOINCREMENT,
    ClienteId INTEGER NOT NULL,
    ComboId INTEGER NOT NULL,
    Inicio DATE NOT NULL,
    DuracionDias INTEGER NOT NULL,
    Activo INTEGER NOT NULL DEFAULT 1,
    
    -- Créditos restantes por tipo de comida
    CreditosDesayunoRestantes INTEGER NOT NULL DEFAULT 0,
    CreditosAlmuerzoRestantes INTEGER NOT NULL DEFAULT 0,
    CreditosCenaRestantes INTEGER NOT NULL DEFAULT 0,
    
    FOREIGN KEY (ClienteId) REFERENCES Clientes(ClienteId),
    FOREIGN KEY (ComboId) REFERENCES Combos(ComboId)
);
```

#### Consumos
```sql
CREATE TABLE Consumos (
    ConsumoId INTEGER PRIMARY KEY AUTOINCREMENT,
    SuscripcionId INTEGER NOT NULL,
    ClienteId INTEGER NOT NULL,
    Dia DATE NOT NULL,
    TipoComida INTEGER NOT NULL,  -- 1=Desayuno, 2=Almuerzo, 3=Cena
    Numero INTEGER NOT NULL,       -- 1 o 2 (máximo 2 por día)
    Origen INTEGER NOT NULL,       -- 1=Manual/Biométrico, 2=Pérdida automática
    
    FOREIGN KEY (SuscripcionId) REFERENCES Suscripciones(SuscripcionId),
    FOREIGN KEY (ClienteId) REFERENCES Clientes(ClienteId)
);
```

#### Avisos
```sql
CREATE TABLE Avisos (
    AvisoId INTEGER PRIMARY KEY AUTOINCREMENT,
    SuscripcionId INTEGER NOT NULL,
    ClienteId INTEGER NOT NULL,
    Dia DATE NOT NULL,
    TipoComida INTEGER NOT NULL,  -- 1=Desayuno, 2=Almuerzo, 3=Cena
    MarcadoPor TEXT NOT NULL,     -- Usuario que marcó el aviso
    
    FOREIGN KEY (SuscripcionId) REFERENCES Suscripciones(SuscripcionId),
    FOREIGN KEY (ClienteId) REFERENCES Clientes(ClienteId)
);
```

### Respaldo y Restauración

#### Crear Respaldo (Backup)

**Manualmente**:
```powershell
# Copiar el archivo de base de datos
Copy-Item "$env:LOCALAPPDATA\RestauranteApp\restaurante.db" `
          -Destination "C:\Backups\restaurante_$(Get-Date -Format 'yyyyMMdd').db"
```

**Automatizado** (agregar a un script):
```powershell
# Backup diario
$origen = "$env:LOCALAPPDATA\RestauranteApp\restaurante.db"
$destino = "\\servidor\backups\restaurante_$(Get-Date -Format 'yyyyMMdd_HHmmss').db"
Copy-Item $origen $destino
```

#### Restaurar desde Respaldo

1. **Cerrar la aplicación** RestauranteApp
2. Navegar a:
   ```
   C:\Users\[Usuario]\AppData\Local\RestauranteApp\
   ```
3. Reemplazar `restaurante.db` con el archivo de respaldo
4. Reiniciar la aplicación

---

## ⏰ Horarios Configurados

Los horarios están definidos en: `RestauranteApp.Core\AsistenciaRulesService.cs`

### Horarios Actuales

| Tipo de Comida | Inicio | Fin   |
|----------------|--------|-------|
| ☕ Desayuno    | 07:00  | 09:30 |
| 🍽️ Almuerzo   | 11:00  | 16:00 |
| 🌙 Cena        | 18:00  | 21:00 |

### Comportamiento

#### Registro Manual
- **Dentro de horario**: Muestra notificación verde "✓ DENTRO DE HORARIO"
- **Fuera de horario**: Muestra notificación amarilla "⚠ FUERA DE HORARIO"
- En ambos casos: **Registra el consumo** (modo manual)

#### Registro Biométrico (Automático)
- **Dentro de horario**: Determina automáticamente el tipo de comida y registra
- **Fuera de horario**: Muestra mensaje "Fuera de horario de comida" y NO registra

#### Pérdidas Automáticas
- **Desayuno**: Si pasa de las 09:30 y no hay consumo ni aviso → Descuenta crédito
- **Cena**: Si pasa de las 21:00 y no hay consumo ni aviso → Descuenta crédito
- **Almuerzo**: NO tiene pérdida automática

### Modificar Horarios

**Editar**: `RestauranteApp.Core\AsistenciaRulesService.cs`

```csharp
public class AsistenciaRuleService
{
    // Tipos
    public const int Desayuno = 1;
    public const int Almuerzo = 2;
    public const int Cena = 3;

    // Modificar estos valores según necesidad
    public TimeSpan DesayunoInicio => new(7, 0, 0);    // 07:00
    public TimeSpan DesayunoFin => new(9, 30, 0);      // 09:30

    public TimeSpan AlmuerzoInicio => new(11, 0, 0);   // 11:00
    public TimeSpan AlmuerzoFin => new(16, 0, 0);      // 16:00

    public TimeSpan CenaInicio => new(18, 0, 0);       // 18:00
    public TimeSpan CenaFin => new(21, 0, 0);          // 21:00
    
    // ... resto del código
}
```

**Después de modificar**:
```powershell
dotnet build -c Release
```

---

## 🛠️ Guía de Desarrollo

### Tecnologías Utilizadas

- **.NET 8.0**: Framework principal
- **WPF**: Interfaz de usuario
- **Entity Framework Core 8.0**: ORM
- **SQLite**: Base de datos
- **Dependency Injection**: Inyección de dependencias
- **MVVM Pattern**: Parcialmente implementado

### Estructura de Capas

```
┌─────────────────────────────────────┐
│      Presentation Layer (WPF)       │  ← RestauranteApp
│  - Windows, Forms, Views            │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│       Business Logic Layer          │  ← RestauranteApp.Core
│  - Services, Rules, Interfaces      │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│       Data Access Layer             │  ← RestauranteApp.Data
│  - DbContext, Entities, Migrations  │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│          Database (SQLite)          │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│      Hardware Layer (Device)        │  ← RestauranteApp.Device
│  - Fingerprint Services             │
└─────────────────────────────────────┘
```

### Agregar Nueva Funcionalidad

#### Ejemplo: Agregar Reportes

1. **Crear Servicio** en `RestauranteApp.Core`:

```csharp
// RestauranteApp.Core/ReportesService.cs
public class ReportesService
{
    private readonly AppDbContext _db;
    
    public ReportesService(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<List<ConsumoResumen>> ObtenerConsumosDelMes(int año, int mes)
    {
        // Lógica de negocio
    }
}
```

2. **Registrar en DI** en `App.xaml.cs`:

```csharp
services.AddScoped<ReportesService>();
```

3. **Crear Ventana** en `RestauranteApp`:

```xml
<!-- ReportesWindow.xaml -->
<Window x:Class="RestauranteApp.ReportesWindow"
        Title="Reportes" Height="600" Width="800">
    <!-- Diseño UI -->
</Window>
```

4. **Code-Behind**:

```csharp
// ReportesWindow.xaml.cs
public partial class ReportesWindow : Window
{
    private readonly ReportesService _reportesService;
    
    public ReportesWindow()
    {
        InitializeComponent();
        
        using var scope = App.Services.CreateScope();
        _reportesService = scope.ServiceProvider.GetRequiredService<ReportesService>();
    }
}
```

### Ejecutar Migraciones

#### Crear Nueva Migración

```powershell
cd RestauranteApp.Data
dotnet ef migrations add NombreDeLaMigracion --startup-project ..\RestauranteApp\RestauranteApp.csproj
```

#### Aplicar Migraciones

```powershell
dotnet ef database update --startup-project ..\RestauranteApp\RestauranteApp.csproj
```

#### Revertir Migración

```powershell
dotnet ef database update MigracionAnterior --startup-project ..\RestauranteApp\RestauranteApp.csproj
```

### Compilar Instalador

1. **Instalar Inno Setup**:
   - Descargar desde: https://jrsoftware.org/isdl.php
   - Instalar con opciones por defecto

2. **Compilar aplicación**:
   ```powershell
   dotnet publish -c Release -r win-x64 --self-contained false
   ```

3. **Abrir script de Inno Setup**:
   ```
   installer\RestauranteApp.iss
   ```

4. **Compilar instalador**:
   - Build → Compile
   - El instalador se genera en: `installer\Output\RestauranteAppSetup.exe`

### Testing

#### Datos de Prueba

Para pruebas rápidas, ejecutar este SQL en la base de datos:

```sql
-- Combo de prueba
INSERT INTO Combos (Nombre, Precio, Desayuno, Almuerzo, Cena, Activo)
VALUES ('Combo Test', 100, 1, 1, 1, 1);

-- Cliente de prueba
INSERT INTO Clientes (Nombre)
VALUES ('Usuario Test');

-- Suscripción de prueba (30 días desde hoy)
INSERT INTO Suscripciones (
    ClienteId, ComboId, Inicio, DuracionDias, Activo,
    CreditosDesayunoRestantes, CreditosAlmuerzoRestantes, CreditosCenaRestantes
)
VALUES (
    1, 1, date('now'), 30, 1,
    30, 30, 30
);
```

---

## 🐛 Solución de Problemas

### Errores Comunes

#### 1. "No se puede conectar a la base de datos"

**Error**: `SqliteException: SQLite Error 14: 'unable to open database file'`

**Solución**:
- Verificar permisos en `%LOCALAPPDATA%\RestauranteApp`
- Crear la carpeta manualmente si no existe
- Ejecutar la aplicación como Administrador

#### 2. "Migraciones pendientes"

**Error**: `Microsoft.EntityFrameworkCore.Infrastructure[10403]`

**Solución**:
```powershell
cd RestauranteApp.Data
dotnet ef database update --startup-project ..\RestauranteApp\RestauranteApp.csproj
```

O desde la aplicación: Las migraciones se aplican automáticamente al iniciar.

#### 3. Aplicación no inicia

**Síntomas**: La aplicación se cierra inmediatamente sin mensajes

**Solución**:
1. Revisar logs en:
   ```
   C:\Users\[Usuario]\AppData\Local\RestauranteApp\app.log
   ```
2. Verificar que .NET 8 Runtime esté instalado
3. Reinstalar la aplicación

#### 4. Ventana aparece en blanco

**Síntomas**: La ventana se abre pero no muestra contenido

**Solución**:
- Comprobar que los estilos se carguen:
  ```xml
  <!-- App.xaml -->
  <ResourceDictionary Source="Resources/Styles.xaml"/>
  ```
- Verificar que no haya errores en el constructor del componente
- Revisar logs de excepciones

#### 5. "No se encontró el servicio X"

**Error**: `InvalidOperationException: Unable to resolve service for type 'X'`

**Solución**:
Verificar en `App.xaml.cs` que el servicio esté registrado:
```csharp
services.AddScoped<X>();
```

### Logs y Diagnóstico

#### Ubicación de Logs

```
C:\Users\[Usuario]\AppData\Local\RestauranteApp\app.log
```

#### Limpiar Logs

```powershell
Remove-Item "$env:LOCALAPPDATA\RestauranteApp\app.log"
```

#### Aumentar Nivel de Log

Editar `App.xaml.cs`, método `LogInfo`:

```csharp
private static void LogInfo(string msg)
{
    try
    {
        // Agregar más detalle
        var stackTrace = new System.Diagnostics.StackTrace();
        File.AppendAllText(GetLogPath(),
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] INFO {msg}\n" +
            $"  StackTrace: {stackTrace}\n");
    }
    catch { }
}
```

### Soporte Adicional

Para problemas no cubiertos en esta documentación:

1. **Revisar logs** en `app.log`
2. **Consultar documentación específica**:
   - [INTEGRACION_LECTOR_HUELLAS.md](INTEGRACION_LECTOR_HUELLAS.md) - Lector de huellas
   - [RestauranteApp.Device/README.md](RestauranteApp.Device/README.md) - Hardware
3. **Ejecutar script de diagnóstico**:
   ```powershell
   .\VerificarSDK-Huellero.ps1
   ```

---

## 📄 Licencia

Este proyecto es software privado desarrollado para [Nombre de la Organización].

---

## 📞 Contacto

Para consultas técnicas o soporte:
- **Desarrollador**: [Tu Nombre]
- **Email**: [tu-email@ejemplo.com]
- **Fecha de Última Actualización**: Marzo 2026

---

## 📋 Checklist de Implementación Completa

- [x] Sistema de clientes funcionando
- [x] Sistema de combos funcionando
- [x] Sistema de suscripciones funcionando
- [x] Registro de consumo manual
- [x] Sistema de avisos (ausencias)
- [x] Renovación de suscripciones
- [x] Procesamiento de vencimientos
- [x] Procesamiento de pérdidas automáticas
- [x] Horarios configurables
- [x] Validación de horarios en registro manual
- [x] Notificaciones dentro/fuera de horario
- [x] Base de datos SQLite
- [x] Migraciones automáticas
- [x] Sistema de logs
- [x] Interfaz del lector de huellas
- [x] Simulador de huellas (desarrollo)
- [x] Implementación HID U.are.U 4500
- [x] Enrollment de huellas (4 capturas)
- [x] Modo escucha automático
- [x] Reconocimiento biométrico
- [x] Instalador Inno Setup
- [x] Documentación completa
- [x] Script de verificación del SDK
- [ ] Testing en producción
- [ ] Capacitación de usuarios finales

---

**¡RestauranteApp listo para producción!** 🎉
