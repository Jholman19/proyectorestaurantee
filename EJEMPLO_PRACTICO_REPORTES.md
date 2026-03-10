# 🎬 EJEMPLO PRÁCTICO - Sistema de Reportes en Acción

## Escena: Lunes 15 de Enero, 09:15 am - Desayuno

### Contexto
- **Horario:** 7:00 - 9:30 am (Desayuno activo)
- **Participantes:** 45 clientes con suscripción vigente
- **Consumidos:** 12 personas
- **Faltantes:** 33 personas
- **Avisos:** 0

---

## Acción 1: Primer Consumo del Día

### Juan García coloca su huella

```
⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪
    Lector DigitalPersona U.are.U 4500
         Colocando huella...
⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪
```

### Sistema Procesa

**1. Detecta ClienteId = 5 (Juan García)**

**2. ReportesService obtiene:**
```
ClienteDetalle:
  Nombre: "Juan García"
  Documento: "1234567890"
  Combo: "Combo Premium" (Desayuno ✓, Almuerzo ✓, Cena ✓)
  Créditos: Desayuno=9, Almuerzo=10, Cena=8
  FechaFin: 31/01/2024 (aún vigente)

ConsumoHoy:
  Desayunos: 0 (no ha consumido aún)
  Almuerzos: 0
  Cenas: 0
```

**3. ConsumoService valida:**
```
✓ Suscripción activa
✓ Suscripción vigente (hoy está en rango)
✓ Combo incluye Desayuno
✓ En horario (9:15 am está en 7:00-9:30)
✓ 9 créditos disponibles para Desayuno
✓ 0 consumos hoy < 2 → PERMITIDO

Resultado: ✅ REGISTRA CONSUMO
  Nuevo registro: Consumo(ClienteId=5, Dia=15/01, TipoComida=1, Numero=1)
  Créditos: 9 → 8 (se resta 1)
```

**4. ReportesService obtiene estadísticas:**
```
EstadisticaComida (Desayuno para hoy):
  TotalConSuscripcion: 45
  TotalConsumieron: 13 (12 previos + 1 Juan)
  TotalAvisos: 0
  TotalFaltantes: 32
  
  ClientesFaltantes (primeros 5):
    - Ana García
    - Carlos López
    - Elena Martínez
    - Francisco González
    - Gabriela Rodríguez
    ... y 27 más
```

### Mensaje Mostrado en UI

```
╔════════════════════════════════════════════════════════════╗
║          ✅ CONSUMO REGISTRADO                             ║
╠════════════════════════════════════════════════════════════╣
║  👤 Juan García                                            ║
║  📦 Combo: Combo Premium                                   ║
║  🍽️ Desayuno                                              ║
║                                                            ║
║  💾 CRÉDITOS ACTUALES:                                    ║
║     Desayuno: 8 (bajó de 9)                               ║
║     Almuerzo: 10                                          ║
║     Cena: 8                                               ║
║                                                            ║
║  📊 HOY (15/01):                                          ║
║     Desayuno: 1/2                                         ║
║                                                            ║
║  📈 ASISTENCIA DESAYUNO:                                  ║
║     13/45 consumieron (28.9%)                             ║
║     Faltantes (32): Ana García, Carlos López,             ║
║     Elena Martínez, Francisco González,                   ║
║     Gabriela Rodríguez... y 27 más                        ║
║                                                            ║
║                           [OK]                            ║
╚════════════════════════════════════════════════════════════╝
```

---

## Acción 2: Segundo Consumo del Mismo Cliente

### Juan García coloca huella nuevamente 5 minutos después (09:20)

```
⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪
    Lector DigitalPersona U.are.U 4500
         Colocando huella...
⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪
```

### Sistema Procesa (igual que antes)

**ConsumoService valida:**
```
✓ Suscripción activa
✓ Suscripción vigente
✓ Combo incluye Desayuno
✓ En horario
✓ 8 créditos disponibles (bajó porque debemos recargar)
✓ 1 consumo hoy < 2 → PERMITIDO (ahora es el segundo)

Resultado: ✅ REGISTRA SEGUNDO CONSUMO
  Nuevo registro: Consumo(ClienteId=5, Dia=15/01, TipoComida=1, Numero=2)
  Créditos: 8 → 7 (se resta 1)
```

### Mensaje Mostrado

```
╔════════════════════════════════════════════════════════════╗
║          ✅ CONSUMO REGISTRADO                             ║
╠════════════════════════════════════════════════════════════╣
║  👤 Juan García                                            ║
║  📦 Combo: Combo Premium                                   ║
║  🍽️ Desayuno                                              ║
║                                                            ║
║  💾 CRÉDITOS ACTUALES:                                    ║
║     Desayuno: 7 (bajó de 8)                               ║
║     Almuerzo: 10                                          ║
║     Cena: 8                                               ║
║                                                            ║
║  📊 HOY (15/01):                                          ║
║     Desayuno: 2/2    ← MÁXIMO ALCANZADO                  ║
║                                                            ║
║  📈 ASISTENCIA DESAYUNO:                                  ║
║     14/45 consumieron                                     ║
║     ...                                                   ║
║                                                            ║
║                           [OK]                            ║
╚════════════════════════════════════════════════════════════╝
```

---

## Acción 3: Intento de Tercer Consumo (RECHAZADO)

### Juan García coloca huella por tercera vez (09:40)

```
⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪
    Lector DigitalPersona U.are.U 4500
         Colocando huella...
⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪⚪
```

### Sistema Procesa

**ConsumoService valida:**
```
✓ Suscripción activa
✓ Suscripción vigente
✓ Combo incluye Desayuno
✓ En horario
✓ 7 créditos disponibles
✗ 2 consumos hoy >= 2 → RECHAZADO

❌ FALLA: "Ya alcanzó el máximo de 2 consumos para esa comida hoy."

Base de datos: SIN CAMBIOS (transacción no se guardó)
Créditos: No se restan
```

### Mensaje Mostrado

```
╔════════════════════════════════════════════════════════════╗
║          ❌ CONSUMO RECHAZADO                              ║
╠════════════════════════════════════════════════════════════╣
║  👤 Juan García                                            ║
║  📦 Combo: Combo Premium                                   ║
║  No se pudo registrar desayuno                             ║
║                                                            ║
║  Ya alcanzó el máximo de 2 consumos para esa comida hoy.   ║
║                                                            ║
║                           [OK]                            ║
╚════════════════════════════════════════════════════════════╝
```

**Database:**
```
Consumos hoy para Juan García (ClienteId=5):
  ├─ Numero: 1, TipoComida: 1, Origen: 1 ✓
  ├─ Numero: 2, TipoComida: 1, Origen: 1 ✓
  └─ (intento 3) RECHAZADO - NO se registra

Suscripcion (creditos):
  ├─ CreditosDesayunoRestantes: 7
  └─ Sin cambios en el intento rechazado
```

---

## Acción 4: Cliente Diferente - Almuerzo

### Media hora después, cambio de horario

**Tiempo:** 11:45 am (Almuerzo activo, 11:00-16:00)

**Pedro López coloca huella**

### Sistema Detecta Cambio de Hora

```
// En OnFingerprintCaptured
for (int t = 1; t <= 3; t++)
{
    if (rules.EstaEnHorario(t, ahora)) // NOW = 11:45
    { 
        tipo = t; break; 
    }
}

// EstaEnHorario(1, 11:45) → false (desayuno 7:00-9:30)
// EstaEnHorario(2, 11:45) → true  (almuerzo 11:00-16:00) ✓
tipo = 2 (Almuerzo)
```

### Mensaje Mostrado

```
╔════════════════════════════════════════════════════════════╗
║          ✅ CONSUMO REGISTRADO                             ║
╠════════════════════════════════════════════════════════════╣
║  👤 Pedro López                                            ║
║  📦 Combo: Combo Básico                                    ║
║  🍽️ Almuerzo                                              ║
║                                                            ║
║  💾 CRÉDITOS ACTUALES:                                    ║
║     Desayuno: 12                                          ║
║     Almuerzo: 8 (bajó de 9)                               ║
║     Cena: 10                                              ║
║                                                            ║
║  📊 HOY (15/01):                                          ║
║     Almuerzo: 1/2                                         ║
║                                                            ║
║  📈 ASISTENCIA ALMUERZO:                                  ║
║     9/45 consumieron (20%)                                ║
║     Faltantes (36): Abel García, Beatriz López,           ║
║     Carmen González, Diana García,                        ║
║     Elena Martínez... y 31 más                            ║
║                                                            ║
║                           [OK]                            ║
╚════════════════════════════════════════════════════════════╝
```

---

## Acción 5: Fuera de Horario

### Cliente intenta consumir a las 10:15 am (entre horarios)

**Horarios vigentes a las 10:15:**
```
Desayuno: 7:00-9:30 → CERRADO
Almuerzo: 11:00-16:00 → AÚN NO ABRE
Cena: 18:00-21:00 → CERRADA
```

### Sistema Rechaza

```
// En OnFingerprintCaptured
int tipo = -1;
for (int t = 1; t <= 3; t++)
{
    if (rules.EstaEnHorario(t, 10:15)) { tipo = t; break; }
}
// Ninguno cumple → tipo = -1

MessageBox.Show("⏰ Huella detectada pero no estamos en horario de comida.", 
                "Fuera de horario");
```

### Mensaje Mostrado

```
╔════════════════════════════════════════════════════════════╗
║          ⏰ Fuera de horario                               ║
╠════════════════════════════════════════════════════════════╣
║                                                            ║
║  Huella detectada pero no estamos en horario de comida.    ║
║                                                            ║
║                           [OK]                            ║
╚════════════════════════════════════════════════════════════╝
```

**Database:** Sin cambios

---

## Acción 6: Cliente sin Suscripción

### Usuario fantasma intenta consumir

**Cliente no existe o fue eliminado**

```
// ReportesService.GetClienteDetalleAsync(999)
var cliente = await _db.Clientes.Where(c => c.ClienteId == 999).FirstOrDefaultAsync();
// → null

if (clienteDetalle is null)
{
    MessageBox.Show($"⚠️ Cliente #{999} no encontrado.", "Error");
    return;
}
```

### Mensaje Mostrado

```
╔════════════════════════════════════════════════════════════╗
║          ⚠️ Error                                           ║
╠════════════════════════════════════════════════════════════╣
║                                                            ║
║  Cliente #999 no encontrado.                              ║
║                                                            ║
║                           [OK]                            ║
╚════════════════════════════════════════════════════════════╝
```

---

## 📊 Estado Final de la Base de Datos (15/01/2024)

### Tabla: Consumos (origen=1, solo los registros exitosos)

```
ConsumoId | ClienteId | Dia       | TipoComida | Numero | Origen
----------|-----------|-----------|------------|--------|-------
  101     | 5         | 15/01     | 1          | 1      | 1      ← Juan 1
  102     | 5         | 15/01     | 1          | 2      | 1      ← Juan 2
  103     | 7         | 15/01     | 2          | 1      | 1      ← Pedro 1
  104     | 12        | 15/01     | 2          | 1      | 1      ← otro
  ...
```

### Tabla: Suscripciones (créditos después de consumos)

```
SuscripcionId | ClienteId | CreditosDesayunoRestantes | CreditosAlmuerzoRestantes | CreditosCenaRestantes
--------------|-----------|---------------------------|---------------------------|----------------------
  1           | 5         | 7                         | 10                        | 8
              | (fue 9, se restaron 2 desayunos)
  
  2           | 7         | 12                        | 8                         | 10
              | (almuerzo se restó 1: 9→8)
```

---

## 🎯 Conclusiones del Ejemplo

✅ **Funciona correctamente:**
- Nombres de clientes se muestran
- Conteo en tiempo real de asistencia
- Límite de 2 comidas se aplica
- Diferentes horarios se respetan
- Cambio de fecha a medianoche manejado
- Estadísticas incluyen faltantes con nombres

✅ **Transacciones ACID:**
- Los rechazos NO dejan registros corruptos
- Los créditos NO se restan en fallos
- La DB permanece consistente

✅ **Experiencia de Usuario:**
- Mensajes claros
- Información útil
- Emojis legibles
- Sin errores técnicos expuestos

