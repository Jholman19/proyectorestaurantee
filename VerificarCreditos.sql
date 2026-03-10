-- ============================================================
-- SCRIPT DE VERIFICACIÓN DE CRÉDITOS
-- Base de datos: restaurante.db (SQLite)
-- Ubicación: C:\Users\jholm\AppData\Local\RestauranteApp\
-- ============================================================

-- 1. Ver todas las suscripciones activas con sus créditos
SELECT 
    s.SuscripcionId,
    c.Nombre AS Cliente,
    cb.Nombre AS Combo,
    s.Inicio,
    s.DuracionDias,
    date(s.Inicio, '+' || s.DuracionDias || ' days') AS FechaFin,
    s.CreditosDesayunoRestantes AS 'Desayuno 🌅',
    s.CreditosAlmuerzoRestantes AS 'Almuerzo 🍽️',
    s.CreditosCenaRestantes AS 'Cena 🌙',
    CASE WHEN s.Activo = 1 THEN '✅' ELSE '❌' END AS Estado
FROM Suscripciones s
JOIN Clientes c ON s.ClienteId = c.ClienteId
JOIN Combos cb ON s.ComboId = cb.ComboId
WHERE s.Activo = 1
ORDER BY c.Nombre;

-- 2. Ver consumos de hoy (cambiar fecha según necesites)
SELECT 
    c.Nombre AS Cliente,
    co.Dia AS Fecha,
    CASE co.TipoComida 
        WHEN 1 THEN '🌅 Desayuno'
        WHEN 2 THEN '🍽️ Almuerzo'
        WHEN 3 THEN '🌙 Cena'
    END AS TipoComida,
    co.Numero AS 'Consumo #',
    CASE co.Origen
        WHEN 1 THEN '✅ Normal'
        WHEN 2 THEN '❌ Pérdida'
    END AS Origen,
    co.CreadoEn AS 'Hora Registro'
FROM Consumos co
JOIN Clientes c ON co.ClienteId = c.ClienteId
WHERE co.Dia = date('now')
ORDER BY co.TipoComida, co.CreadoEn;

-- 3. Historial de créditos consumidos por cliente
SELECT 
    c.Nombre AS Cliente,
    COUNT(CASE WHEN co.TipoComida = 1 THEN 1 END) AS 'Desayunos',
    COUNT(CASE WHEN co.TipoComida = 2 THEN 1 END) AS 'Almuerzos',
    COUNT(CASE WHEN co.TipoComida = 3 THEN 1 END) AS 'Cenas',
    COUNT(*) AS 'Total Consumos',
    COUNT(CASE WHEN co.Origen = 2 THEN 1 END) AS 'Pérdidas'
FROM Consumos co
JOIN Clientes c ON co.ClienteId = c.ClienteId
GROUP BY c.Nombre
ORDER BY COUNT(*) DESC;

-- 4. Verificar créditos vs consumos
SELECT 
    c.Nombre AS Cliente,
    s.CreditosDesayunoRestantes + COUNT(CASE WHEN co.TipoComida = 1 THEN 1 END) AS 'Desayuno: Usados + Restantes',
    s.CreditosAlmuerzoRestantes + COUNT(CASE WHEN co.TipoComida = 2 THEN 1 END) AS 'Almuerzo: Usados + Restantes',
    s.CreditosCenaRestantes + COUNT(CASE WHEN co.TipoComida = 3 THEN 1 END) AS 'Cena: Usados + Restantes',
    s.DuracionDias AS 'Días Totales'
FROM Suscripciones s
JOIN Clientes c ON s.ClienteId = c.ClienteId
LEFT JOIN Consumos co ON s.SuscripcionId = co.SuscripcionId
WHERE s.Activo = 1
GROUP BY s.SuscripcionId
ORDER BY c.Nombre;

-- 5. Avisos marcados por cliente
SELECT 
    c.Nombre AS Cliente,
    a.Dia AS Fecha,
    CASE a.TipoComida 
        WHEN 1 THEN '🌅 Desayuno'
        WHEN 2 THEN '🍽️ Almuerzo'
        WHEN 3 THEN '🌙 Cena'
    END AS TipoComida,
    a.MarcadoPor AS 'Marcado Por',
    a.CreadoEn AS 'Hora'
FROM Avisos a
JOIN Clientes c ON a.ClienteId = c.ClienteId
ORDER BY a.Dia DESC, a.CreadoEn DESC
LIMIT 20;

-- 6. Suscripciones próximas a vencer (últimos 3 días)
SELECT 
    c.Nombre AS Cliente,
    s.Inicio,
    date(s.Inicio, '+' || s.DuracionDias || ' days') AS FechaVencimiento,
    julianday(date(s.Inicio, '+' || s.DuracionDias || ' days')) - julianday(date('now')) AS 'Días Restantes',
    s.CreditosDesayunoRestantes AS 'Des🌅',
    s.CreditosAlmuerzoRestantes AS 'Alm🍽️',
    s.CreditosCenaRestantes AS 'Cen🌙'
FROM Suscripciones s
JOIN Clientes c ON s.ClienteId = c.ClienteId
WHERE s.Activo = 1
  AND julianday(date(s.Inicio, '+' || s.DuracionDias || ' days')) - julianday(date('now')) <= 3
ORDER BY julianday(date(s.Inicio, '+' || s.DuracionDias || ' days'));

-- 7. Resumen general del sistema
SELECT 
    'Clientes Activos' AS Concepto,
    COUNT(*) AS Cantidad
FROM Clientes WHERE Activo = 1
UNION ALL
SELECT 
    'Suscripciones Activas',
    COUNT(*) 
FROM Suscripciones WHERE Activo = 1
UNION ALL
SELECT 
    'Consumos Hoy',
    COUNT(*) 
FROM Consumos WHERE Dia = date('now')
UNION ALL
SELECT 
    'Consumos Total',
    COUNT(*) 
FROM Consumos
UNION ALL
SELECT 
    'Pérdidas Automáticas',
    COUNT(*) 
FROM Consumos WHERE Origen = 2
UNION ALL
SELECT 
    'Avisos Registrados',
    COUNT(*) 
FROM Avisos;

-- ============================================================
-- CÓMO USAR ESTE SCRIPT:
-- ============================================================
-- 1. Abrir DB Browser for SQLite o similar
-- 2. Abrir: C:\Users\jholm\AppData\Local\RestauranteApp\restaurante.db
-- 3. Ir a "Execute SQL"
-- 4. Copiar y pegar las consultas que necesites
-- 5. Ejecutar
-- ============================================================
