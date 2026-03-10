using System;

namespace RestauranteApp.Core;

public class AsistenciaRuleService
{
    // Tipos
    public const int Desayuno = 1;
    public const int Almuerzo = 2;
    public const int Cena = 3;

    // Horarios
    public TimeSpan DesayunoInicio => new(7, 0, 0);
    public TimeSpan DesayunoFin => new(9, 30, 0);

    public TimeSpan AlmuerzoInicio => new(11, 0, 0);
    public TimeSpan AlmuerzoFin => new(16, 0, 0);

    public TimeSpan CenaInicio => new(18, 0, 0);
    public TimeSpan CenaFin => new(21, 0, 0);

    public bool EstaEnHorario(int tipoComida, DateTime ahora)
    {
        var t = ahora.TimeOfDay;

        return tipoComida switch
        {
            Desayuno => t >= DesayunoInicio && t <= DesayunoFin,
            Almuerzo => t >= AlmuerzoInicio && t <= AlmuerzoFin,
            Cena => t >= CenaInicio && t <= CenaFin,
            _ => false
        };
    }

    public bool YaPasoCierre(int tipoComida, DateTime ahora)
    {
        var t = ahora.TimeOfDay;

        return tipoComida switch
        {
            Desayuno => t > DesayunoFin,
            Almuerzo => t > AlmuerzoFin,
            Cena => t > CenaFin,
            _ => false
        };
    }
}