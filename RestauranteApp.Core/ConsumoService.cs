// Archivo: RestauranteApp.Core/ConsumoService.cs
// REEMPLAZAR COMPLETO

using Microsoft.EntityFrameworkCore;
using RestauranteApp.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RestauranteApp.Core;

public class ConsumoService
{
    private static readonly TimeSpan VentanaAntiRebote = TimeSpan.FromMilliseconds(1500);

    private readonly AppDbContext _db;
    private readonly AsistenciaRuleService _rules;
    private readonly SuscripcionesService _subs;

    public ConsumoService(AppDbContext db, AsistenciaRuleService rules, SuscripcionesService subs)
    {
        _db = db;
        _rules = rules;
        _subs = subs;
    }

    // =========================================================
    // 1) MÉTODO PRINCIPAL (consumo real)
    // =========================================================
    public async Task ConsumirAsync(int clienteId, int tipoComida, DateTime ahora, bool manual = false)
    {
        var dia = ahora.Date;

        var sus = await _db.Suscripciones
            .Include(s => s.Combo)
            .Where(s => s.ClienteId == clienteId && s.Activo)
            .OrderByDescending(s => s.SuscripcionId)
            .FirstOrDefaultAsync();

        if (sus is null)
            throw new InvalidOperationException("El cliente no tiene suscripción activa.");

        if (!_subs.EstaVigente(sus, dia))
            throw new InvalidOperationException("La suscripción no está vigente para este día.");

        if (sus.Combo is null)
            throw new InvalidOperationException("Suscripción sin combo válido.");

        // ✅ Solo verificar horario si NO es manual
        if (!manual && !_rules.EstaEnHorario(tipoComida, ahora))
            throw new InvalidOperationException("Fuera del horario permitido para esa comida.");

        if (!ComboPermite(sus.Combo, tipoComida))
            throw new InvalidOperationException("El combo no incluye esa comida.");

        if (GetCreditosRestantes(sus, tipoComida) <= 0)
            throw new InvalidOperationException("No quedan créditos disponibles para esa comida.");

        // Anti-rebote: evita doble registro por una sola colocación de dedo.
        // Permite una segunda marcación real si ocurre fuera de esta ventana.
        var ultimoConsumo = await _db.Consumos
            .Where(c => c.SuscripcionId == sus.SuscripcionId &&
                        c.Dia == dia &&
                        c.TipoComida == tipoComida &&
                        c.Origen == 1)
            .OrderByDescending(c => c.CreadoEn)
            .FirstOrDefaultAsync();

        if (ultimoConsumo != null && (ahora - ultimoConsumo.CreadoEn).Duration() < VentanaAntiRebote)
            throw new InvalidOperationException("Lectura duplicada detectada. Retira y vuelve a colocar el dedo.");

        // Máximo 2 por día por tipo (origen=1 consumo normal)
        var consumosHoy = await _db.Consumos.CountAsync(c =>
            c.SuscripcionId == sus.SuscripcionId &&
            c.Dia == dia &&
            c.TipoComida == tipoComida &&
            c.Origen == 1);

        if (consumosHoy >= 2)
            throw new InvalidOperationException("Ya alcanzó el máximo de 2 consumos para esa comida hoy.");

        var numero = consumosHoy + 1; // 1 o 2

        _db.Consumos.Add(new Consumo
        {
            SuscripcionId = sus.SuscripcionId,
            ClienteId = clienteId,
            Dia = dia,
            TipoComida = tipoComida,
            Numero = numero,
            Origen = 1,
            CreadoEn = ahora
        });

        DescontarCredito(sus, tipoComida, 1);

        await _db.SaveChangesAsync();
    }

    // =========================================================
    // 2) OVERLOAD COMPATIBLE con tu UI:
    // ConsumirAsync(clienteId, DateTime, "Desayuno", manual: true)
    // =========================================================
    public Task ConsumirAsync(int clienteId, DateTime ahora, string tipoTexto, bool manual = false)
    {
        var tipo = ParseTipoComida(tipoTexto);
        return ConsumirAsync(clienteId, tipo, ahora, manual);
    }

    // =========================================================
    // 3) AVISO (personal)
    // =========================================================
    public async Task MarcarAvisoAsync(int clienteId, DateTime dia, int tipoComida, string marcadoPor)
    {
        dia = dia.Date;

        var sus = await _db.Suscripciones
            .Where(s => s.ClienteId == clienteId && s.Activo)
            .OrderByDescending(s => s.SuscripcionId)
            .FirstOrDefaultAsync();

        if (sus is null)
            throw new InvalidOperationException("No hay suscripción activa.");

        if (!_subs.EstaVigente(sus, dia))
            throw new InvalidOperationException("La suscripción no está vigente para ese día.");

        _db.Avisos.Add(new Aviso
        {
            SuscripcionId = sus.SuscripcionId,
            ClienteId = clienteId,
            Dia = dia,
            TipoComida = tipoComida,
            MarcadoPor = marcadoPor ?? ""
        });

        await _db.SaveChangesAsync();
    }

    public Task MarcarAvisoAsync(int clienteId, DateTime dia, string tipoTexto, string marcadoPor)
    {
        var tipo = ParseTipoComida(tipoTexto);
        return MarcarAvisoAsync(clienteId, dia, tipo, marcadoPor);
    }

    // =========================================================
    // 4) PÉRDIDAS AUTOMÁTICAS (solo desayuno y cena sin aviso)
    //    ⚠️ ALMUERZO NO tiene pérdida automática
    // =========================================================
    public async Task ProcesarPerdidasAsync(DateTime ahora)
    {
        var hoy = ahora.Date;
        LogInfo($"ProcesarPerdidasAsync: Iniciando verificación para {ahora:yyyy-MM-dd HH:mm:ss}");

        if (_rules.YaPasoCierre(AsistenciaRuleService.Desayuno, ahora))
        {
            LogInfo("ProcesarPerdidasAsync: ✅ Ya pasó cierre de Desayuno, procesando...");
            await ProcesarPerdidaDiaTipoAsync(hoy, AsistenciaRuleService.Desayuno);
        }
        else
        {
            LogInfo("ProcesarPerdidasAsync: ⏰ Aún NO ha pasado cierre de Desayuno");
        }

        // ⚠️ Almuerzo NO se procesa automáticamente
        LogInfo("ProcesarPerdidasAsync: ⚠️ Almuerzo NO se procesa (sin pérdida automática)");

        if (_rules.YaPasoCierre(AsistenciaRuleService.Cena, ahora))
        {
            LogInfo("ProcesarPerdidasAsync: ✅ Ya pasó cierre de Cena, procesando...");
            await ProcesarPerdidaDiaTipoAsync(hoy, AsistenciaRuleService.Cena);
        }
        else
        {
            LogInfo("ProcesarPerdidasAsync: ⏰ Aún NO ha pasado cierre de Cena");
        }

        LogInfo("ProcesarPerdidasAsync: Finalizado");
    }

    private async Task ProcesarPerdidaDiaTipoAsync(DateTime dia, int tipoComida)
    {
        // ⚠️ Solo Desayuno y Cena tienen pérdida automática
        if (tipoComida != AsistenciaRuleService.Desayuno && 
            tipoComida != AsistenciaRuleService.Cena)
            return;

        var tipoNombre = tipoComida == AsistenciaRuleService.Desayuno ? "Desayuno" : "Cena";
        LogInfo($"ProcesarPerdidas: Iniciando para {tipoNombre} del día {dia:yyyy-MM-dd}");

        var suscripciones = await _db.Suscripciones
            .Include(s => s.Combo)
            .Where(s => s.Activo)
            .ToListAsync();

        LogInfo($"ProcesarPerdidas: {suscripciones.Count} suscripciones activas encontradas");

        int procesadas = 0;

        foreach (var sus in suscripciones)
        {
            if (!_subs.EstaVigente(sus, dia)) continue;
            if (sus.Combo is null) continue;
            if (!ComboPermite(sus.Combo, tipoComida)) continue;
            if (GetCreditosRestantes(sus, tipoComida) <= 0) continue;

            var consumio = await _db.Consumos.AnyAsync(c =>
                c.SuscripcionId == sus.SuscripcionId &&
                c.Dia == dia &&
                c.TipoComida == tipoComida &&
                c.Origen == 1);

            if (consumio) continue;

            var aviso = await _db.Avisos.AnyAsync(a =>
                a.SuscripcionId == sus.SuscripcionId &&
                a.Dia == dia &&
                a.TipoComida == tipoComida);

            if (aviso) continue;

            _db.Consumos.Add(new Consumo
            {
                SuscripcionId = sus.SuscripcionId,
                ClienteId = sus.ClienteId,
                Dia = dia,
                TipoComida = tipoComida,
                Numero = 1,
                Origen = 2
            });

            DescontarCredito(sus, tipoComida, 1);
            procesadas++;
        }

        await _db.SaveChangesAsync();
        LogInfo($"ProcesarPerdidas: {procesadas} pérdidas de {tipoNombre} procesadas para {dia:yyyy-MM-dd}");
    }

    // =========================================================
    // 5) MÉTODOS PRINCIPALES (ya)
    // =========================================================
    public async Task<List<Consumo>> ListarDiaAsync(DateTime dia)
    {
        dia = dia.Date;

        return await _db.Consumos
            .Include(c => c.Cliente)
            .Include(c => c.Suscripcion)
            .AsNoTracking()
            .Where(c => c.Dia == dia)
            .OrderBy(c => c.TipoComida)
            .ThenBy(c => c.Numero)
            .ThenBy(c => c.ConsumoId)
            .ToListAsync();
    }

    public async Task<int> CreditosPorDiaAsync(DateTime dia, int tipoComida)
    {
        dia = dia.Date;

        var sus = await _db.Suscripciones
            .Include(s => s.Combo)
            .Where(s => s.Activo)
            .ToListAsync();

        var vigentes = sus.Where(s => _subs.EstaVigente(s, dia) && s.Combo != null && ComboPermite(s.Combo!, tipoComida));
        return vigentes.Count();
    }

    public async Task<int> CreditosUsadosEnDiaAsync(DateTime dia, int tipoComida)
    {
        dia = dia.Date;

        return await _db.Consumos.CountAsync(c =>
            c.Dia == dia && c.TipoComida == tipoComida);
    }

    // =========================================================
    // 6) ✅ ALIAS para lo que tu SuscripcionesWindow llama
    // =========================================================

    // ✅ ListarDiaAsync( DateTime, int )  -> filtra por tipoComida
    public async Task<List<Consumo>> ListarDiaAsync(DateTime dia, int tipoComida)
    {
        dia = dia.Date;

        return await _db.Consumos
            .Include(c => c.Cliente)
            .Include(c => c.Suscripcion)
            .AsNoTracking()
            .Where(c => c.Dia == dia && c.TipoComida == tipoComida)
            .OrderBy(c => c.Numero)
            .ThenBy(c => c.ConsumoId)
            .ToListAsync();
    }

    // ✅ CreditosPorDiaAsync( DateTime ) -> devuelve total del día (sumando tipos)
    public async Task<int> CreditosPorDiaAsync(DateTime dia)
    {
        dia = dia.Date;

        var d = await CreditosPorDiaAsync(dia, AsistenciaRuleService.Desayuno);
        var a = await CreditosPorDiaAsync(dia, AsistenciaRuleService.Almuerzo);
        var c = await CreditosPorDiaAsync(dia, AsistenciaRuleService.Cena);

        return d + a + c;
    }

    // ✅ CreditosUsadosEnDiaAsync( int tipo, DateTime dia ) (orden invertido)
    public Task<int> CreditosUsadosEnDiaAsync(int tipoComida, DateTime dia)
        => CreditosUsadosEnDiaAsync(dia, tipoComida);

    // =========================================================
    // Helpers
    // =========================================================
    private static bool ComboPermite(Combo combo, int tipoComida)
    {
        return tipoComida switch
        {
            AsistenciaRuleService.Desayuno => combo.Desayuno,
            AsistenciaRuleService.Almuerzo => combo.Almuerzo,
            AsistenciaRuleService.Cena => combo.Cena,
            _ => false
        };
    }

    private static int GetCreditosRestantes(Suscripcion sus, int tipoComida)
    {
        return tipoComida switch
        {
            AsistenciaRuleService.Desayuno => sus.CreditosDesayunoRestantes,
            AsistenciaRuleService.Almuerzo => sus.CreditosAlmuerzoRestantes,
            AsistenciaRuleService.Cena => sus.CreditosCenaRestantes,
            _ => 0
        };
    }

    private static void DescontarCredito(Suscripcion sus, int tipoComida, int cantidad)
    {
        if (cantidad <= 0) return;

        switch (tipoComida)
        {
            case AsistenciaRuleService.Desayuno:
                sus.CreditosDesayunoRestantes = Math.Max(0, sus.CreditosDesayunoRestantes - cantidad);
                break;
            case AsistenciaRuleService.Almuerzo:
                sus.CreditosAlmuerzoRestantes = Math.Max(0, sus.CreditosAlmuerzoRestantes - cantidad);
                break;
            case AsistenciaRuleService.Cena:
                sus.CreditosCenaRestantes = Math.Max(0, sus.CreditosCenaRestantes - cantidad);
                break;
        }
    }

    private static int ParseTipoComida(string tipoTexto)
    {
        var t = (tipoTexto ?? "").Trim().ToLowerInvariant();

        if (t.Contains("des")) return AsistenciaRuleService.Desayuno;
        if (t.Contains("alm")) return AsistenciaRuleService.Almuerzo;
        if (t.Contains("cen")) return AsistenciaRuleService.Cena;

        return AsistenciaRuleService.Almuerzo;
    }

    private static void LogInfo(string msg)
    {
        try
        {
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RestauranteApp");

            Directory.CreateDirectory(folder);
            var logPath = Path.Combine(folder, "app.log");

            File.AppendAllText(logPath,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ConsumoService] {msg}\n");
        }
        catch { }
    }
}