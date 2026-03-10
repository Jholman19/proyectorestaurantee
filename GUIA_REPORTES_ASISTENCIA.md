# 🎯 Guía de Uso - Sistema de Reportes y Conteo de Asistencia

## 📌 Resumen

Se ha agregado un nuevo **ReportesService** que proporciona:

1. **Estadísticas de asistencia** en tiempo real
2. **Información detallada de clientes** con suscripciones
3. **Conteo de consumos** por tipo de comida
4. **Listado de faltantes** con nombres

Toda esta información se muestra automáticamente cuando se captura una huella en el lector de huellas.

---

## 🔧 Componentes Nuevos

### 1. **ReportesService.cs** (RestauranteApp.Core)

Servicio inyectable que proporciona:

```csharp
// Estadísticas de un tipo de comida
Task<EstadisticaComidaDto> GetEstadisticaComidaAsync(int tipoComida, DateTime? fecha = null)

// Estadísticas del día completo (3 comidas)
Task<EstadisticasDiaCompleto> GetEstadisticasDiaCompleto(DateTime? fecha = null)

// Detalle de cliente con suscripción y créditos
Task<ClienteDetalleDto?> GetClienteDetalleAsync(int clienteId)

// Consumo de hoy de un cliente
Task<ConsumoClienteHoyDto> GetConsumoHoyAsync(int clienteId, DateTime? fecha = null)
```

### 2. **DTOs (Data Transfer Objects)**

#### `EstadisticaComidaDto`
```csharp
public class EstadisticaComidaDto
{
    public int TipoComida { get; set; }           // 1=Desayuno, 2=Almuerzo, 3=Cena
    public string NombreTipo { get; set; }        // "Desayuno"
    public DateTime Fecha { get; set; }           // Fecha del reporte
    public int TotalConSuscripcion { get; set; }  // Total de clientes aptos
    public int TotalConsumieron { get; set; }     // Cuántos consumieron
    public int TotalAvisos { get; set; }          // Cuántos marcaron aviso
    public int TotalFaltantes { get; set; }       // Cuántos faltan
    public List<ClienteInfoDto> ClientesFaltantes { get; set; } // Nombres
}
```

**Ejemplo:**
```json
{
  "TipoComida": 2,
  "NombreTipo": "Almuerzo",
  "Fecha": "2024-01-15",
  "TotalConSuscripcion": 45,
  "TotalConsumieron": 38,
  "TotalAvisos": 2,
  "TotalFaltantes": 5,
  "ClientesFaltantes": [
    { "ClienteId": 10, "Nombre": "Juan García" },
    { "ClienteId": 15, "Nombre": "María López" },
    ...
  ]
}
```

#### `ClienteDetalleDto`
```csharp
public class ClienteDetalleDto
{
    public int ClienteId { get; set; }
    public string Nombre { get; set; }
    public string Documento { get; set; }
    public string Telefono { get; set; }
    public bool Activo { get; set; }
    public bool SuscripcionActiva { get; set; }
    public int? SuscripcionId { get; set; }
    public string? Combo { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int CreditosDesayuno { get; set; }
    public int CreditosAlmuerzo { get; set; }
    public int CreditosCena { get; set; }
    public bool TieneDesayuno { get; set; }
    public bool TieneAlmuerzo { get; set; }
    public bool TieneCena { get; set; }
}
```

#### `ConsumoClienteHoyDto`
```csharp
public class ConsumoClienteHoyDto
{
    public int ClienteId { get; set; }
    public DateTime Fecha { get; set; }
    public int DesayunosHoy { get; set; }  // 0, 1, o 2
    public int AlmuerzosHoy { get; set; }  // 0, 1, o 2
    public int CenasHoy { get; set; }      // 0, 1, o 2
}
```

---

## 🎬 Flujo de Uso - Captura de Huella

### Antes (Antiguo)
```
Persona coloca huella
    ↓
Sistema detecta Cliente ID
    ↓
Mensaje simple: "✓ Consumido: cliente 42 - Desayuno"
```

### Después (Nuevo)
```
Persona coloca huella
    ↓
Sistema detecta Cliente ID (42)
    ↓
ReportesService obtiene:
  - Datos del cliente (Juan García)
  - Combo actual (Combo Premium)
  - Créditos restantes (8 desc, 9 alm, 7 cena)
  - Consumos de hoy (1 desayuno)
    ↓
ConsumoService registra consumo
    ↓
ReportesService obtiene estadísticas de hoy:
  - Desayunos: 24/45 consumieron (falta: Pedro, Ana, Luis...)
  - Almuerzos: 22/45
  - Cenas: pendiente
    ↓
Mensaje detallado mostrado:
╔══════════════════════════════════════╗
║ ✅ CONSUMO REGISTRADO                ║
╠══════════════════════════════════════╣
║ 👤 Juan García                       ║
║ 📦 Combo: Combo Premium             ║
║ 🍽️ Desayuno                         ║
║                                      ║
║ 💾 CRÉDITOS ACTUALES:               ║
║   Desayuno: 8                        ║
║   Almuerzo: 9                        ║
║   Cena: 7                            ║
║                                      ║
║ 📊 HOY (15/01):                     ║
║   Desayuno: 2/2                      ║
║                                      ║
║ 📈 ASISTENCIA DESAYUNO:             ║
║   24/45 consumieron                  ║
║   Faltantes (21): Pedro García,      ║
║   Ana Martínez, Luis González... y 18 más
║                                      ║
║                     [OK]             ║
╚══════════════════════════════════════╝
```

---

## 🧪 Casos de Uso / Pruebas

### Caso 1: Consumo Normal - Mañana
**Tiempo:** 09:00 am  
**Acción:** Juan García coloca huella  
**Resultado esperado:**
```
✅ CONSUMO REGISTRADO
👤 Juan García
📦 Combo: Combo Premium
🍽️ Desayuno

💾 CRÉDITOS ACTUALES:
  Desayuno: 9
  Almuerzo: 10
  Cena: 8

📊 HOY (15/01):
  Desayuno: 1/2

📈 ASISTENCIA DESAYUNO:
  1/45 consumieron
  Faltantes (44): María López, Pedro García...
```

### Caso 2: Segundo Consumo - Mismo Día
**Tiempo:** 09:20 am  
**Precondición:** Juan ya consumió una vez  
**Acción:** Juan García coloca huella nuevamente  
**Resultado esperado:**
```
✅ CONSUMO REGISTRADO
👤 Juan García
...
💾 CRÉDITOS ACTUALES:
  Desayuno: 8    ← Bajó de 9 a 8
  ...

📊 HOY (15/01):
  Desayuno: 2/2   ← Ahora está al máximo
```

### Caso 3: Tercer Intento - Rechazado
**Tiempo:** 09:40 am  
**Precondición:** Juan ya consumió 2 veces  
**Acción:** Juan García coloca huella por tercera vez  
**Resultado esperado:**
```
❌ CONSUMO RECHAZADO

👤 Juan García
📦 Combo: Combo Premium
No se pudo registrar desayuno

Ya alcanzó el máximo de 2 consumos para esa comida hoy.
```

### Caso 4: Intervalo de Estadísticas - Almuerzo
**Tiempo:** 12:10 pm  
**Acción:** María López coloca huella  
**Resultado esperado:**
```
✅ CONSUMO REGISTRADO
👤 María López
...
🍽️ Almuerzo

📈 ASISTENCIA ALMUERZO:
  12/45 consumieron
  Faltantes (33): Juan García, Pedro García...
```

### Caso 5: Fuera de Horario
**Tiempo:** 10:00 am (entre desayuno y almuerzo)  
**Acción:** Pedro García coloca huella  
**Resultado esperado:**
```
⏰ Huella detectada pero no estamos en horario de comida.
```

### Caso 6: Cliente sin Suscripción Activa
**Acción:** Cliente no registrado o sin suscripción coloca huella  
**Resultado esperado:**
```
⚠️ Cliente #123 no encontrado.
```
O:
```
❌ CONSUMO RECHAZADO
El cliente no tiene suscripción activa.
```

### Caso 7: Sin Créditos Disponibles
**Precondición:** Cliente con 0 créditos para ese tipo  
**Acción:** Cliente coloca huella  
**Resultado esperado:**
```
❌ CONSUMO RECHAZADO
No quedan créditos disponibles para esa comida.
```

---

## 🔍 Verificación del Comportamiento

### Verificar Estadísticas Correctas

**SQL para validar:**
```sql
-- Ver consumos de hoy
SELECT 
    TipoComida,
    COUNT(DISTINCT ClienteId) as ConsumidoCount
FROM Consumos
WHERE Dia = date('now')
  AND Origen = 1
GROUP BY TipoComida;

-- Ver avisos de hoy
SELECT 
    TipoComida,
    COUNT(DISTINCT ClienteId) as AvisoCount
FROM Avisos
WHERE Dia = date('now')
GROUP BY TipoComida;

-- Ver clientes activos con suscripción vigente
SELECT 
    COUNT(DISTINCT ClienteId) as TotalActivos
FROM Suscripciones
WHERE Activo = 1
  AND Inicio <= date('now')
  AND date(Inicio, '+' || DuracionDias || ' days') >= date('now');
```

### Verificar Nombres se Muestran Correctamente

En el mensaje mostrado, debería aparecer:
```
Faltantes (5): Juan García, María López, Pedro García, Ana Martínez, Luis González
```

Si los nombres NO aparecen:
- ✅ Base datos tiene registros de Clientes
- ✅ Clientes están vinculados a Suscripciones
- ✅ Suscripciones son vigentes hoy
- ✅ Combo incluye el tipo de comida

---

## 📝 Cambios en el Código

### App.xaml.cs
```csharp
// Registro del servicio
services.AddScoped<ReportesService>();
```

### SuscripcionesWindow.xaml.cs
```csharp
// En OnFingerprintCaptured (línea 585+)
var reportesSvc = scope.ServiceProvider.GetRequiredService<ReportesService>();
var clienteDetalle = await reportesSvc.GetClienteDetalleAsync(clienteId);
var estadisticas = await reportesSvc.GetEstadisticaComidaAsync(tipo);
```

---

## ⚙️ Rendimiento

- **GetClienteDetalleAsync**: 2-3 queries (Cliente + Suscripción)
- **GetEstadisticaComidaAsync**: 4-5 queries (Suscripciones activas + Consumos + Avisos + Clientes)
- **GetConsumoHoyAsync**: 1-2 queries (Consumos del día)

**Optimización:** Se usan `AsNoTracking()` para mejorar rendimiento.

---

## 🐛 Troubleshooting

### Problema: "ReportesService no se encuentra"
**Solución:** Verificar que esté registrado en App.xaml.cs
```csharp
services.AddScoped<ReportesService>();  // Línea ~108
```

### Problema: Nombres de faltantes no aparecen
**Solución:** 
1. Verificar que Cliente está vinculado a Suscripción
2. Verificar que Suscripción es vigente para hoy
3. Ejecutar SQL de validación arriba

### Problema: Conteo de consumidos es incorrecto
**Solución:**
1. Verificar campo `Origen = 1` (consumo normal, no pérdida)
2. Verificar que `Dia` es hoy (no ayer/mañana)
3. Ejecutar SQL de validación arriba

---

## 🎓 Próximas Mejoras (Opcional)

1. **Ventana de Reportes Detallada**
   - Dashboard con gráficos
   - Estadísticas por día/semana/mes
   - Exportar a Excel/PDF

2. **Notificaciones**
   - Aviso cuando falta poco por alcanzar el máximo de 2
   - Recordatorio cuando alguien no ha consumido en x horas

3. **Historial**
   - Ver consumos históricos
   - Tendencias de asistencia
   - Detección de absentismo

4. **Integración con SMS**
   - Notificar a clientes faltantes
   - Avisos de gestión

