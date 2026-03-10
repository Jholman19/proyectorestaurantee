using System;
using System.Collections.Generic;

namespace RestauranteApp.Data;

public class Cliente
{
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = "";
    public string? Documento { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;

    // Plantilla de huella binaria (nullable). Guardar solo template, no imagen.
    public byte[]? HuellaTemplate { get; set; }

    public List<Suscripcion> Suscripciones { get; set; } = new();
}

public class Combo
{
    public int ComboId { get; set; }
    public string Nombre { get; set; } = "";

    // Flags combinables
    public bool Desayuno { get; set; }
    public bool Almuerzo { get; set; }
    public bool Cena { get; set; }

    public bool Activo { get; set; } = true;

    public List<Suscripcion> Suscripciones { get; set; } = new();
}

public class Suscripcion
{
    public int SuscripcionId { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int ComboId { get; set; }
    public Combo? Combo { get; set; }

    public DateTime Inicio { get; set; } = DateTime.Today;

    // ✅ Duración variable: 7, 10, 15, 20, 22, 30...
    public int DuracionDias { get; set; } = 30;

    // ✅ Campo real en BD
    public bool Activo { get; set; } = true;

    // ✅ COMPATIBILIDAD con tu UI (que usa "Activa")
    // NO cambia el nombre anterior, solo lo mapea
    public bool Activa
    {
        get => Activo;
        set => Activo = value;
    }

    // ✅ COMPATIBILIDAD con tu UI (que usa "Fin")
    // NO se guarda en BD, se calcula con Inicio + DuracionDias
    public DateTime Fin => Inicio.Date.AddDays(DuracionDias);

    // ✅ Créditos restantes (se gastan en consumos y pérdidas)
    public int CreditosDesayunoRestantes { get; set; }
    public int CreditosAlmuerzoRestantes { get; set; }
    public int CreditosCenaRestantes { get; set; }

    public List<Consumo> Consumos { get; set; } = new();
    public List<Aviso> Avisos { get; set; } = new();
}

public class Consumo
{
    public int ConsumoId { get; set; }

    public int SuscripcionId { get; set; }
    public Suscripcion? Suscripcion { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public DateTime Dia { get; set; } = DateTime.Today;

    // 1=Desayuno, 2=Almuerzo, 3=Cena
    public int TipoComida { get; set; }

    // ✅ 1 o 2 (máximo 2 por día por tipo)
    public int Numero { get; set; } = 1;

    // ✅ 1=consumo normal, 2=pérdida automática
    public int Origen { get; set; } = 1;

    public DateTime CreadoEn { get; set; } = DateTime.Now;
}

public class Aviso
{
    public int AvisoId { get; set; }

    public int SuscripcionId { get; set; }
    public Suscripcion? Suscripcion { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public DateTime Dia { get; set; } = DateTime.Today;

    // 1=Desayuno, 2=Almuerzo, 3=Cena
    public int TipoComida { get; set; }

    public DateTime CreadoEn { get; set; } = DateTime.Now;

    public string MarcadoPor { get; set; } = "";
}