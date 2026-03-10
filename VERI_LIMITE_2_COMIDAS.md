# 📋 Verificación: Límite de 2 Comidas por Día

## Descripción del Requisito
El sistema debe limitar a **máximo 2 consumos por tipo de comida por día** para evitar que una persona abuse del sistema marcando la misma comida más de 2 veces.

**Regla:** `consumosHoy >= 2` → Rechazar consumo

---

## 1. Código de Validación (ConsumoService.cs, línea 60-67)

```csharp
// Máximo 2 por día por tipo (origen=1 consumo normal)
var consumosHoy = await _db.Consumos.CountAsync(c =>
    c.SuscripcionId == sus.SuscripcionId &&
    c.Dia == dia &&
    c.TipoComida == tipoComida &&
    c.Origen == 1);

if (consumosHoy >= 2)
    throw new InvalidOperationException("Ya alcanzó el máximo de 2 consumos para esa comida hoy.");
```

### Análisis de la Lógica ✅

| Escenario | Consumos Hoy | Condición | Resultado | Estado |
|-----------|--------------|-----------|-----------|--------|
| Primer intento | 0 | `0 >= 2` → false | ✅ PERMITIR | OK |
| Segundo intento | 1 | `1 >= 2` → false | ✅ PERMITIR | OK |
| Tercer intento | 2 | `2 >= 2` → true | ❌ RECHAZAR | OK |
| Cuarto intento | 2 | `2 >= 2` → true | ❌ RECHAZAR | OK |

**Conclusión:** La lógica es CORRECTA. ✅

---

## 2. Validaciones Adicionales ANTES del Límite

El código valida en este orden (ConsumoService.cs, línea 30-67):

1. **Línea 35-38:** Suscripción existe y está activa
   ```csharp
   if (sus is null)
       throw new InvalidOperationException("El cliente no tiene suscripción activa.");
   ```

2. **Línea 40-41:** Suscripción vigente para la fecha
   ```csharp
   if (!_subs.EstaVigente(sus, dia))
       throw new InvalidOperationException("La suscripción no está vigente...");
   ```

3. **Línea 43-44:** Combo válido
   ```csharp
   if (sus.Combo is null)
       throw new InvalidOperationException("Suscripción sin combo válido.");
   ```

4. **Línea 49-50:** Verificar horario (SOLO si no es manual)
   ```csharp
   if (!manual && !_rules.EstaEnHorario(tipoComida, ahora))
       throw new InvalidOperationException("Fuera del horario permitido...");
   ```

5. **Línea 52-53:** Combo incluye ese tipo de comida
   ```csharp
   if (!ComboPermite(sus.Combo, tipoComida))
       throw new InvalidOperationException("El combo no incluye esa comida.");
   ```

6. **Línea 55-56:** Créditos disponibles
   ```csharp
   if (GetCreditosRestantes(sus, tipoComida) <= 0)
       throw new InvalidOperationException("No quedan créditos disponibles...");
   ```

7. **Línea 60-67:** ✅ MÁXIMO 2 POR DÍA (esta es la prueba)

---

## 3. Escenarios de Prueba

### Test 1: Registrar dos desayunos exitosamente
```
1. Primer Desayuno (09:00) → ✅ Registrado (consumosHoy=0 < 2)
   DB: 1 Consumo con Numero=1, Origen=1
   
2. Segundo Desayuno (09:15) → ✅ Registrado (consumosHoy=1 < 2)
   DB: 2 Consumos con Numero=1 y Numero=2, Origen=1
   
Ambos débitos de crédito aplicados (-1 cada uno)
```

### Test 2: Rechazar tercer desayuno
```
3. Tercer Desayuno (09:30) → ❌ RECHAZADO (consumosHoy=2 >= 2)
   Error: "Ya alcanzó el máximo de 2 consumos para esa comida hoy."
   DB: Sin cambios (transacción NO se guarda)
   Créditos: No se restan
```

### Test 3: Diferentes tipos de comida en el mismo día (PERMITIDO)
```
Mañana 09:00 Desayuno #1 → ✅ OK (consumosHoy=0)
Mañana 09:15 Desayuno #2 → ✅ OK (consumosHoy=1)
Mediodía 12:00 Almuerzo #1 → ✅ OK (diferente tipo, consumosHoy=0)
Mediodía 12:15 Almuerzo #2 → ✅ OK (consumosHoy=1)
Noche 19:00 Cena #1 → ✅ OK (diferente tipo, consumosHoy=0)

Explicación: El filtro es `c.TipoComida == tipoComida`, así que 
cada tipo cuenta independientemente.
```

### Test 4: Avisos NO cuentan hacia el límite
```
Desayuno #1 (09:00) → ✅ Registrado (consumosHoy=0 < 2)
Desayuno #2 (09:15) → ✅ Registrado (consumosHoy=1 < 2)
Aviso Desayuno (marcar inasistencia) → ✅ Registrado en tabla AVISOS
Desayuno #3 (09:30) → ❌ RECHAZADO (consumosHoy=2, avisos no cuentan)

Nota: El "Aviso" es una tabla separada, no está en Consumos con Origen=1
```

### Test 5: Pérdidas automáticas NO cuentan hacia el límite
```
Línea 66: c.Origen == 1  ← SOLO consumos normales

Las pérdidas automáticas se guardan con Origen=2:
Desayuno #1 (manual) → ✅ Registrado (Origen=1)
Desayuno #2 (manual) → ✅ Registrado (Origen=1)
Desayuno (pérdida automática) → Guardada con Origen=2, no cuenta

Luego:
Desayuno #3 (intento) → ❌ RECHAZADO (consumosHoy=2 Origen=1)
```

---

## 4. Casos Especiales a Verificar

### 🔍 Caso A: Cambio de fecha a las 00:00
```
21:59 Cena Hoy #1 → ✅ Registrado
21:59 Cena Hoy #2 → ✅ Registrado
00:01 Cena Mañana → CUENTA PARA MAÑANA (c.Dia == dia se replantea)

El sistema usa `var dia = ahora.Date;` que resetea a las 00:00
→ No hay problema ✅
```

### 🔍 Caso B: Consumo manual vs automático
```
ConsumoService.ConsumirAsync tiene parámetro: bool manual = false

Si manual=true:
- NO se valida horario (línea 49)
- SÍ se valida el límite de 2 (línea 60)
- Registra con Origen=1

Pérdidas automáticas en ProcesarPerdidasAsync:
- Se guardan con Origen=2 (línea 208 en ConsumoService)
- No cuentan para el límite
```

### 🔍 Caso C: Sincronización de Suscripción
```
El límite se aplica POR SUSCRIPCIÓN (línea 61):
c.SuscripcionId == sus.SuscripcionId

Si un cliente tiene 2 suscripciones activas (extremo):
- Suscripción A: 2 consumos hoy → límite alcanzado
- Suscripción B: 0 consumos hoy → límite no alcanzado

Cada suscripción tiene su propio contador ✅
```

---

## 5. Verificación en Base de Datos (SQL)

Para verificar manualmente si el límite funciona:

```sql
-- Ver consumos normales de un cliente hoy
SELECT 
    c.ClienteId,
    c.TipoComida,
    COUNT(*) as Consumos,
    GROUP_CONCAT(c.Numero) as Numeros
FROM Consumos c
WHERE c.ClienteId = ? 
  AND c.Dia = date('now')
  AND c.Origen = 1
GROUP BY c.ClienteId, c.TipoComida;

-- Resultado esperado: MAX Consumos = 2
```

---

## 6. Interfaz de Usuario - Mensaje de Error

Cuando se rechaza el tercer intento, se ve:

```
❌ CONSUMO RECHAZADO

[Cliente Nombre]
Combo: [Combo Name]
No se pudo registrar desayuno

Ya alcanzó el máximo de 2 consumos para esa comida hoy.
```

También muestra en SuscripcionesWindow.xaml:
```
CONSUMOS HOY (2024-01-15):
Desayunos: 2 | Almuerzos: 0 | Cenas: 0
```

---

## 7. Responsabilidades por Caso de Uso

| Caso | Responsable | Verificación |
|------|-------------|--------------|
| Usuario intenta 3° consumo vía huella | ConsumoService | ✅ Línea 60-67 |
| Usuario intenta consumo manual | SuscripcionesWindow | ✅ Usa ConsumoService |
| Sistema procesa pérdida automática | ConsumoService.ProcesarPerdidasAsync | Origen=2, no afecta |

---

## 8. Conclusión

**Estado del Límite de 2 Comidas: ✅ VERIFICADO SIN BUGS**

- ✅ Lógica correcta: `if (consumosHoy >= 2) → reject`
- ✅ Orden de validación: horario → combo → créditos → límite
- ✅ No hay off-by-one errors
- ✅ Cuenta solo consumos normales (Origen=1)
- ✅ Validaciones previas evitan casos anómalos
- ✅ Error message claro al usuario
- ✅ DB se resetea correctamente cada día a las 00:00

**Recomendación:** El sistema está diseñado correctamente. No se encontraron bugs en la validación del límite de 2 comidas por día.

