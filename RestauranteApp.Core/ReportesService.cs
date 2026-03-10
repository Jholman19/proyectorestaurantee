using Microsoft.EntityFrameworkCore;
using RestauranteApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestauranteApp.Core;

/// <summary>
/// Servicio para generar reportes y estadísticas de asistencia y consumo
/// </summary>
public class ReportesService
{
    private readonly AppDbContext _db;

    public ReportesService(AppDbContext db)
    {
        _db = db;
    }

    // =========================================================
    // ESTADÍSTICAS DEL DÍA
    // =========================================================

    /// <summary>
    /// Obtiene estadísticas de asistencia de un tipo de comida para hoy
    /// </summary>
    public async Task<EstadisticaComidaDto> GetEstadisticaComidaAsync(int tipoComida, DateTime? fecha = null)
    {
        fecha ??= DateTime.Today;
        var dia = fecha.Value.Date;

        using var scope = _db.Database.BeginTransaction();
        
        // Obtener todos los clientes activos con suscripción vigente
        var clientesConSuscripcion = await _db.Suscripciones
            .AsNoTracking()
            .Where(s => s.Activo && s.Inicio.Date <= dia && s.Inicio.Date.AddDays(s.DuracionDias) >= dia)
            .Include(s => s.Combo)
            .GroupBy(s => s.ClienteId)
            .Select(g => new { ClienteId = g.Key, Sus = g.OrderByDescending(x => x.SuscripcionId).First() })
            .ToListAsync();

        // Filtrar solo los que tienen ese tipo de comida en su combo
        var clientesAptosIds = clientesConSuscripcion
            .Where(x => TipoComidaEnCombo(x.Sus.Combo, tipoComida))
            .Select(x => x.ClienteId)
            .ToList();

        // Obtener consumos de hoy para este tipo
        var consumosHoy = await _db.Consumos
            .AsNoTracking()
            .Where(c => c.Dia == dia && c.TipoComida == tipoComida && c.Origen == 1)
            .Select(c => c.ClienteId)
            .Distinct()
            .ToListAsync();

        // Obtener avisos (justificantes de inasistencia) de hoy para este tipo
        var avisos = await _db.Avisos
            .AsNoTracking()
            .Where(a => a.Dia == dia && a.TipoComida == tipoComida)
            .Select(a => a.ClienteId)
            .Distinct()
            .ToListAsync();

        var idsFaltantes = clientesAptosIds
            .Where(id => !consumosHoy.Contains(id) && !avisos.Contains(id))
            .ToList();

        // Obtener nombres de los que faltaron
        var nombresFaltantes = await _db.Clientes
            .AsNoTracking()
            .Where(c => idsFaltantes.Contains(c.ClienteId))
            .OrderBy(c => c.Nombre)
            .Select(c => new ClienteInfoDto { ClienteId = c.ClienteId, Nombre = c.Nombre })
            .ToListAsync();

        return new EstadisticaComidaDto
        {
            TipoComida = tipoComida,
            NombreTipo = GetNombreTipoComida(tipoComida),
            Fecha = dia,
            TotalConSuscripcion = clientesAptosIds.Count,
            TotalConsumieron = consumosHoy.Count,
            TotalAvisos = avisos.Count,
            TotalFaltantes = idsFaltantes.Count,
            ClientesFaltantes = nombresFaltantes
        };
    }

    /// <summary>
    /// Obtiene estadísticas de los tres tipos de comida para hoy
    /// </summary>
    public async Task<EstadisticasDiaCompleto> GetEstadisticasDiaCompleto(DateTime? fecha = null)
    {
        fecha ??= DateTime.Today;

        var desayuno = await GetEstadisticaComidaAsync(AsistenciaRuleService.Desayuno, fecha);
        var almuerzo = await GetEstadisticaComidaAsync(AsistenciaRuleService.Almuerzo, fecha);
        var cena = await GetEstadisticaComidaAsync(AsistenciaRuleService.Cena, fecha);

        return new EstadisticasDiaCompleto
        {
            Fecha = fecha.Value.Date,
            Desayuno = desayuno,
            Almuerzo = almuerzo,
            Cena = cena
        };
    }

    // =========================================================
    // INFORMACIÓN DE CLIENTES
    // =========================================================

    /// <summary>
    /// Obtiene información del cliente por ID
    /// </summary>
    public async Task<ClienteDetalleDto?> GetClienteDetalleAsync(int clienteId)
    {
        var cliente = await _db.Clientes
            .AsNoTracking()
            .Where(c => c.ClienteId == clienteId)
            .FirstOrDefaultAsync();

        if (cliente is null) return null;

        var suscripcion = await _db.Suscripciones
            .AsNoTracking()
            .Where(s => s.ClienteId == clienteId && s.Activo)
            .OrderByDescending(s => s.SuscripcionId)
            .Include(s => s.Combo)
            .FirstOrDefaultAsync();

        return new ClienteDetalleDto
        {
            ClienteId = cliente.ClienteId,
            Nombre = cliente.Nombre,
            Documento = cliente.Documento,
            Telefono = cliente.Telefono,
            Activo = cliente.Activo,
            SuscripcionActiva = suscripcion != null,
            SuscripcionId = suscripcion?.SuscripcionId,
            Combo = suscripcion?.Combo?.Nombre,
            FechaInicio = suscripcion?.Inicio,
            FechaFin = suscripcion?.Fin,
            CreditosDesayuno = suscripcion?.CreditosDesayunoRestantes ?? 0,
            CreditosAlmuerzo = suscripcion?.CreditosAlmuerzoRestantes ?? 0,
            CreditosCena = suscripcion?.CreditosCenaRestantes ?? 0,
            TieneDesayuno = suscripcion?.Combo?.Desayuno ?? false,
            TieneAlmuerzo = suscripcion?.Combo?.Almuerzo ?? false,
            TieneCena = suscripcion?.Combo?.Cena ?? false
        };
    }

    /// <summary>
    /// Obtiene consumo de hoy de un cliente
    /// </summary>
    public async Task<ConsumoClienteHoyDto> GetConsumoHoyAsync(int clienteId, DateTime? fecha = null)
    {
        fecha ??= DateTime.Today;
        var dia = fecha.Value.Date;

        var consumos = await _db.Consumos
            .AsNoTracking()
            .Where(c => c.ClienteId == clienteId && c.Dia == dia && c.Origen == 1)
            .GroupBy(c => c.TipoComida)
            .Select(g => new { TipoComida = g.Key, Cantidad = g.Count() })
            .ToListAsync();

        return new ConsumoClienteHoyDto
        {
            ClienteId = clienteId,
            Fecha = dia,
            DesayunosHoy = consumos.FirstOrDefault(c => c.TipoComida == AsistenciaRuleService.Desayuno)?.Cantidad ?? 0,
            AlmuerzosHoy = consumos.FirstOrDefault(c => c.TipoComida == AsistenciaRuleService.Almuerzo)?.Cantidad ?? 0,
            CenasHoy = consumos.FirstOrDefault(c => c.TipoComida == AsistenciaRuleService.Cena)?.Cantidad ?? 0
        };
    }

    // =========================================================
    // HELPERS
    // =========================================================

    private bool TipoComidaEnCombo(Combo? combo, int tipoComida) =>
        combo switch
        {
            null => false,
            _ when tipoComida == AsistenciaRuleService.Desayuno => combo.Desayuno,
            _ when tipoComida == AsistenciaRuleService.Almuerzo => combo.Almuerzo,
            _ when tipoComida == AsistenciaRuleService.Cena => combo.Cena,
            _ => false
        };

    private string GetNombreTipoComida(int tipoComida) =>
        tipoComida switch
        {
            AsistenciaRuleService.Desayuno => "Desayuno",
            AsistenciaRuleService.Almuerzo => "Almuerzo",
            AsistenciaRuleService.Cena => "Cena",
            _ => "Desconocido"
        };
}

// =========================================================
// DTOs
// =========================================================

public class EstadisticaComidaDto
{
    public int TipoComida { get; set; }
    public string NombreTipo { get; set; } = "";
    public DateTime Fecha { get; set; }
    public int TotalConSuscripcion { get; set; }
    public int TotalConsumieron { get; set; }
    public int TotalAvisos { get; set; }
    public int TotalFaltantes { get; set; }
    public List<ClienteInfoDto> ClientesFaltantes { get; set; } = new();

    public string Resumen => 
        $"{NombreTipo}: {TotalConsumieron}/{TotalConSuscripcion} " +
        $"(Avisos: {TotalAvisos}, Faltantes: {TotalFaltantes})";
}

public class EstadisticasDiaCompleto
{
    public DateTime Fecha { get; set; }
    public EstadisticaComidaDto Desayuno { get; set; } = new();
    public EstadisticaComidaDto Almuerzo { get; set; } = new();
    public EstadisticaComidaDto Cena { get; set; } = new();

    public string ResumenCompleto =>
        $"📊 ESTADÍSTICAS {Fecha:yyyy-MM-dd}\n" +
        $"{Desayuno.Resumen}\n" +
        $"{Almuerzo.Resumen}\n" +
        $"{Cena.Resumen}";
}

public class ClienteInfoDto
{
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = "";
}

public class ClienteDetalleDto
{
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Telefono { get; set; } = "";
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

    public string NombreYDocumento => $"{Nombre} ({Documento})";
    
    public string ResumenCreditos =>
        $"Desayuno: {CreditosDesayuno} | " +
        $"Almuerzo: {CreditosAlmuerzo} | " +
        $"Cena: {CreditosCena}";
}

public class ConsumoClienteHoyDto
{
    public int ClienteId { get; set; }
    public DateTime Fecha { get; set; }
    public int DesayunosHoy { get; set; }
    public int AlmuerzosHoy { get; set; }
    public int CenasHoy { get; set; }

    public string Resumen =>
        $"Desayunos: {DesayunosHoy} | " +
        $"Almuerzos: {AlmuerzosHoy} | " +
        $"Cenas: {CenasHoy}";
}
