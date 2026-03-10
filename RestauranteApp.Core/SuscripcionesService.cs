using Microsoft.EntityFrameworkCore;
using RestauranteApp.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RestauranteApp.Core;

public class SuscripcionesService
{
    private readonly AppDbContext _db;

    public SuscripcionesService(AppDbContext db)
    {
        _db = db;
    }

    // ====== MÉTODO PRINCIPAL (nuevo) ======
    public async Task<Suscripcion> CrearSuscripcionAsync(int clienteId, int comboId, DateTime inicio, int duracionDias)
    {
        if (duracionDias <= 0)
            throw new InvalidOperationException("La duración en días debe ser mayor que 0.");

        if (duracionDias > 365)
            throw new InvalidOperationException("La duración en días es demasiado alta (máximo 365).");

        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.ClienteId == clienteId);
        if (cliente is null) throw new InvalidOperationException("Cliente no existe.");

        var combo = await _db.Combos.FirstOrDefaultAsync(c => c.ComboId == comboId && c.Activo);
        if (combo is null) throw new InvalidOperationException("Combo no existe o está inactivo.");

        // Créditos iniciales = DuracionDias si el combo incluye ese tipo
        var credDes = combo.Desayuno ? duracionDias : 0;
        var credAlm = combo.Almuerzo ? duracionDias : 0;
        var credCen = combo.Cena ? duracionDias : 0;

        var s = new Suscripcion
        {
            ClienteId = clienteId,
            ComboId = comboId,
            Inicio = inicio.Date,
            DuracionDias = duracionDias,
            Activo = true,

            CreditosDesayunoRestantes = credDes,
            CreditosAlmuerzoRestantes = credAlm,
            CreditosCenaRestantes = credCen
        };

        _db.Suscripciones.Add(s);
        await _db.SaveChangesAsync();
        return s;
    }

    public bool EstaVigente(Suscripcion s, DateTime dia)
    {
        var inicio = s.Inicio.Date;
        var finExclusivo = inicio.AddDays(s.DuracionDias).Date;
        var d = dia.Date;

        return d >= inicio && d < finExclusivo && s.Activo;
    }

    // ====== ALIAS para tu UI (NO cambias pantallas) ======

    // Tu UI llama CrearAsync(...)
    public Task<Suscripcion> CrearAsync(int clienteId, int comboId, DateTime inicio, int duracionDias)
        => CrearSuscripcionAsync(clienteId, comboId, inicio, duracionDias);

    // Tu UI llama ProcesarVencimientosAsync()
    public async Task ProcesarVencimientosAsync(DateTime? hoy = null)
    {
        var d = (hoy ?? DateTime.Today).Date;

        var lista = await _db.Suscripciones
            .Where(s => s.Activo)
            .ToListAsync();

        foreach (var s in lista)
        {
            var fin = s.Inicio.Date.AddDays(s.DuracionDias).Date;
            if (d >= fin)
                s.Activo = false;
        }

        await _db.SaveChangesAsync();
    }

    // ====== RENOVACIÓN =====
    public async Task<Suscripcion> RenovarAsync(int clienteId, int duracionDias)
    {
        if (duracionDias <= 0)
            throw new InvalidOperationException("La duración en días debe ser mayor que 0.");

        if (duracionDias > 365)
            throw new InvalidOperationException("La duración en días es demasiado alta (máximo 365).");

        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.ClienteId == clienteId);
        if (cliente is null) 
            throw new InvalidOperationException("Cliente no existe.");

        // Buscar última suscripción (activa o vencida)
        var ultimaSuscripcion = await _db.Suscripciones
            .Where(s => s.ClienteId == clienteId)
            .OrderByDescending(s => s.SuscripcionId)
            .FirstOrDefaultAsync();

        if (ultimaSuscripcion?.Combo is null)
            throw new InvalidOperationException("No hay suscripción previa para renovar.");

        var combo = ultimaSuscripcion.Combo;

        // Crear nueva suscripción con mismo combo
        var credDes = combo.Desayuno ? duracionDias : 0;
        var credAlm = combo.Almuerzo ? duracionDias : 0;
        var credCen = combo.Cena ? duracionDias : 0;

        var suscripcionNueva = new Suscripcion
        {
            ClienteId = clienteId,
            ComboId = combo.ComboId,
            Inicio = DateTime.Today,
            DuracionDias = duracionDias,
            Activo = true,

            CreditosDesayunoRestantes = credDes,
            CreditosAlmuerzoRestantes = credAlm,
            CreditosCenaRestantes = credCen
        };

        _db.Suscripciones.Add(suscripcionNueva);
        await _db.SaveChangesAsync();

        return suscripcionNueva;
    }
}