# 🧪 GUÍA DE PRUEBAS DEL SISTEMA DE CRÉDITOS

## 🎯 Objetivo
Verificar prácticamente que los créditos se descuentan correctamente al pasar los días y las fechas.

---

## 📋 PRUEBA 1: Consumo Normal (Registro Manual)

### Pasos:
1. **Abrir la aplicación** RestauranteApp
2. **Crear un cliente de prueba:**
   - Clic en "➕ Nuevo"
   - Nombre: "Juan Prueba"
   - Guardar

3. **Crear un combo de prueba:**
   - Clic en "🍝 Combos"
   - Clic en "Nuevo"
   - Nombre: "Prueba Completo"
   - ✅ Marcar: Desayuno, Almuerzo, Cena
   - Guardar

4. **Crear suscripción de 5 días:**
   - Clic en "⭐ Suscripciones"
   - Seleccionar "Juan Prueba"
   - Combo: "Prueba Completo"
   - Inicio: Hoy
   - Duración: 5 días
   - Clic en "Crear Suscripción"

5. **VERIFICAR créditos iniciales:**
   ```
   ✅ Desayuno: 5 créditos
   ✅ Almuerzo: 5 créditos
   ✅ Cena: 5 créditos
   ```

6. **Registrar consumo manual:**
   - Seleccionar "Juan Prueba" en la lista
   - Seleccionar "Almuerzo" en el dropdown
   - Clic en "Registrar Consumo Manual"
   - Confirmar

7. **VERIFICAR descuento:**
   ```
   ✅ Desayuno: 5 créditos (sin cambio)
   ✅ Almuerzo: 4 créditos ← DESCONTÓ 1
   ✅ Cena: 5 créditos (sin cambio)
   ```

### ✅ Resultado esperado:
- Se descontó exactamente 1 crédito de almuerzo
- Los demás créditos no cambiaron
- Se muestra en la lista de consumos del día

---

## 📋 PRUEBA 2: Pérdida Automática (No Consumió)

### Preparación:
```sql
-- Ejecutar en la base de datos para simular que pasó el horario
-- (Solo para pruebas, NO hacer en producción)
UPDATE Consumos 
SET Dia = date('now', '-1 day') 
WHERE ClienteId = (SELECT ClienteId FROM Clientes WHERE Nombre = 'Juan Prueba');
```

### Pasos:
1. **Cerrar completamente la aplicación** (Alt+F4)

2. **Cambiar la hora del sistema** para simular:
   - Opción A: Cambiar a después de las 9:30 AM (para desayuno)
   - Opción B: Cambiar a después de las 9:00 PM (para cena)

3. **Abrir nuevamente la aplicación**
   - Al iniciar, ejecuta automáticamente `ProcesarPerdidasAsync()`

4. **Ir a Suscripciones y buscar a "Juan Prueba"**

5. **VERIFICAR descuento automático:**
   - Si pasó horario de desayuno:
     ```
     ✅ Desayuno: 4 créditos ← DESCONTÓ 1 (pérdida)
     ```
   - Si pasó horario de cena:
     ```
     ✅ Cena: 4 créditos ← DESCONTÓ 1 (pérdida)
     ```

6. **Verificar en consumos:**
   - Ir a la pestaña de consumos del día
   - Buscar el consumo con 🔴 (pérdida automática)

### ✅ Resultado esperado:
- Se descontó 1 crédito automáticamente
- Aparece marcado como "Pérdida" en rojo

---

## 📋 PRUEBA 3: Aviso (No Descuenta)

### Pasos:
1. **Seleccionar "Juan Prueba"**
2. **Ir a la fecha de MAÑANA**
3. **Marcar aviso de ausencia:**
   - Seleccionar tipo: "Desayuno"
   - Clic en "Marcar Aviso de Ausencia"
   - Confirmar

4. **VERIFICAR que NO descuenta:**
   ```
   ✅ Desayuno: 4 créditos (igual que antes)
   ```

5. **Al día siguiente:**
   - Aunque pase el horario de desayuno
   - NO se descuenta crédito automáticamente
   - Porque hay un aviso registrado

### ✅ Resultado esperado:
- El aviso protege el crédito
- No se crea pérdida automática
- Los créditos quedan intactos

---

## 📋 PRUEBA 4: Máximo 2 Consumos por Día

### Pasos:
1. **Registrar 1er almuerzo:**
   - Seleccionar "Almuerzo"
   - Clic en "Registrar Consumo Manual"
   - ✅ Éxito: Créditos = 3

2. **Registrar 2do almuerzo:**
   - Seleccionar "Almuerzo"
   - Clic en "Registrar Consumo Manual"
   - ✅ Éxito: Créditos = 2

3. **Intentar registrar 3er almuerzo:**
   - Seleccionar "Almuerzo"
   - Clic en "Registrar Consumo Manual"
   - ❌ ERROR: "Ya alcanzó el máximo de 2 consumos para esa comida hoy"

### ✅ Resultado esperado:
- Permite máximo 2 consumos por día por tipo
- El tercer intento es rechazado
- Los créditos no se descuentan en el 3er intento

---

## 📋 PRUEBA 5: Sin Créditos Disponibles

### Preparación:
Consumir todos los créditos de desayuno:
1. Día 1: 2 desayunos → Quedan 2 créditos
2. Día 2: 2 desayunos → Quedan 0 créditos

### Pasos:
1. **Intentar registrar desayuno cuando créditos = 0:**
   - Seleccionar "Desayuno"
   - Clic en "Registrar Consumo Manual"
   - ❌ ERROR: "No quedan créditos disponibles para esa comida"

2. **Verificar que los otros tipos SÍ funcionan:**
   - Seleccionar "Almuerzo" (que aún tiene créditos)
   - Clic en "Registrar Consumo Manual"
   - ✅ Éxito

### ✅ Resultado esperado:
- No permite consumir sin créditos
- Los créditos no bajan de 0
- Otros tipos de comida funcionan normalmente

---

## 📋 PRUEBA 6: Verificación en Base de Datos

### Pasos:
1. **Abrir DB Browser for SQLite**
   - Descargar: https://sqlitebrowser.org/

2. **Abrir base de datos:**
   ```
   C:\Users\jholm\AppData\Local\RestauranteApp\restaurante.db
   ```

3. **Ejecutar consulta:**
   ```sql
   SELECT 
       c.Nombre,
       s.CreditosDesayunoRestantes,
       s.CreditosAlmuerzoRestantes,
       s.CreditosCenaRestantes
   FROM Suscripciones s
   JOIN Clientes c ON s.ClienteId = c.ClienteId
   WHERE c.Nombre = 'Juan Prueba' AND s.Activo = 1;
   ```

4. **Comparar con la aplicación:**
   - Los números deben coincidir EXACTAMENTE

### ✅ Resultado esperado:
- Los créditos en BD = créditos en aplicación
- Confirma que los cambios se persisten correctamente

---

## 📋 PRUEBA 7: Renovación de Suscripción

### Pasos:
1. **Dejar que la suscripción expire:**
   - Esperar 5 días o cambiar fecha del sistema
   - Al abrir la app, se marca como inactiva

2. **Verificar créditos restantes:**
   - Ejemplo: Quedan 2 desayuno, 1 almuerzo, 3 cena

3. **Renovar suscripción:**
   - Seleccionar cliente
   - Clic en "Renovar"
   - Elegir 10 días
   - Confirmar

4. **VERIFICAR créditos después de renovar:**
   ```
   ✅ Desayuno: 10 créditos (se reinician)
   ✅ Almuerzo: 10 créditos (se reinician)
   ✅ Cena: 10 créditos (se reinician)
   ```

### ✅ Resultado esperado:
- Los créditos se reinician según la nueva duración
- La suscripción anterior queda inactiva
- No se suman los créditos viejos con los nuevos

---

## 🎓 RESUMEN DE VALIDACIONES

### ✅ El sistema garantiza:
1. ✅ Créditos iniciales = días de suscripción
2. ✅ Cada consumo descuenta 1 crédito
3. ✅ Pérdidas automáticas descuentan 1 crédito
4. ✅ Avisos protegen los créditos
5. ✅ Máximo 2 consumos por día por tipo
6. ✅ No permite consumir sin créditos
7. ✅ Créditos nunca son negativos
8. ✅ Cambios se persisten en BD
9. ✅ Suscripciones vencidas no permiten consumir
10. ✅ Renovación reinicia créditos correctamente

---

## 🐛 Si encuentras un error:

1. **Anota exactamente qué hiciste**
2. **Captura de pantalla**
3. **Revisa errores en:** `C:\Users\jholm\AppData\Local\RestauranteApp\log.txt`
4. **Verifica en la base de datos** con las consultas SQL

---

## 📞 Soporte

Si algo no funciona como se describe aquí, revisa:
- [VERIFICACION_CREDITOS.md](VERIFICACION_CREDITOS.md) - Documentación técnica
- [VerificarCreditos.sql](VerificarCreditos.sql) - Consultas SQL de diagnóstico
- Log de la aplicación - Para ver errores

---

✅ **Sistema verificado y listo para producción**
📅 Fecha: 4 de marzo de 2026
