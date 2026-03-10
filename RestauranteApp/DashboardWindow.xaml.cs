using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestauranteApp.Core;
using RestauranteApp.Data;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RestauranteApp;

public partial class DashboardWindow : Window
{
    private readonly ReportesService _reportesService;

    public DashboardWindow()
    {
        InitializeComponent();
        Loaded += DashboardWindow_Loaded;

        // Obtener servicio de reportes
        _reportesService = App.Services.GetRequiredService<ReportesService>();
    }

    private async void DashboardWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            FechaTextBlock.Text = DateTime.Today.ToString("dddd, dd \\de MMMM \\de yyyy").ToUpper();
            await ActualizarEstadisticasAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar estadísticas: {ex.Message}", "Error");
        }
    }

    private async Task ActualizarEstadisticasAsync()
    {
        try
        {
            // Obtener estadísticas del día completo
            var estadisticas = await _reportesService.GetEstadisticasDiaCompleto();

            // ========== DESAYUNO ==========
            ActualizarComida(
                estadisticas.Desayuno,
                DesayunoConsumoText,
                DesayunoTotalText,
                DesayunoProgressBar,
                DesayunoProgressText,
                FaltantesDesayunoLabel,
                FaltantesDesayunoList
            );

            // ========== ALMUERZO ==========
            ActualizarComida(
                estadisticas.Almuerzo,
                AlmuerzoConsumoText,
                AlmuerzoTotalText,
                AlmuerzoProgressBar,
                AlmuerzoProgressText,
                FaltantesAlmuerzoLabel,
                FaltantesAlmuerzoList
            );

            // ========== CENA ==========
            ActualizarComida(
                estadisticas.Cena,
                CenaConsumoText,
                CenaTotalText,
                CenaProgressBar,
                CenaProgressText,
                FaltantasCenaLabel,
                FaltantasCenaList
            );

            // Actualizar timestamp
            UltimaActualizacionText.Text = $"Última actualización: {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al actualizar: {ex.Message}", "Error");
        }
    }

    private void ActualizarComida(
        EstadisticaComidaDto estadistica,
        System.Windows.Controls.TextBlock consumoTextBlock,
        System.Windows.Controls.TextBlock totalTextBlock,
        System.Windows.Controls.ProgressBar progressBar,
        System.Windows.Controls.TextBlock progressTextBlock,
        System.Windows.Controls.TextBlock faltantesLabelBlock,
        System.Windows.Controls.TextBlock faltantesListBlock
    )
    {
        // Actualizar números
        consumoTextBlock.Text = estadistica.TotalConsumieron.ToString();
        totalTextBlock.Text = estadistica.TotalConSuscripcion.ToString();

        // Calcular porcentaje
        var total = estadistica.TotalConSuscripcion;
        var porcentaje = total > 0 ? (double)estadistica.TotalConsumieron / total * 100 : 0;

        // Actualizar barra
        progressBar.Value = porcentaje;
        progressTextBlock.Text = 
            $"{porcentaje:F0}% - {estadistica.TotalFaltantes} faltantes" +
            (estadistica.TotalAvisos > 0 ? $" ({estadistica.TotalAvisos} avisos)" : "");

        // Actualizar lista de faltantes
        faltantesLabelBlock.Text = $"{GetEmojiTipoComida(estadistica.TipoComida)} {estadistica.NombreTipo} " +
                                   $"({estadistica.TotalFaltantes} faltantes):";

        if (estadistica.TotalFaltantes == 0)
        {
            faltantesListBlock.Text = "✅ Todos han consumido";
            faltantesListBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(39, 174, 96)); // Verde
        }
        else
        {
            var nombres = string.Join(", ", 
                estadistica.ClientesFaltantes
                    .Take(5)
                    .Select(c => c.Nombre));

            if (estadistica.ClientesFaltantes.Count > 5)
                nombres += $"... y {estadistica.ClientesFaltantes.Count - 5} más";

            faltantesListBlock.Text = nombres;
            faltantesListBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(85, 85, 85)); // Gris oscuro
        }
    }

    private string GetEmojiTipoComida(int tipoComida)
    {
        return tipoComida switch
        {
            AsistenciaRuleService.Desayuno => "☀️",
            AsistenciaRuleService.Almuerzo => "🌤️",
            AsistenciaRuleService.Cena => "🌙",
            _ => "🍽️"
        };
    }

    private async void Actualizar_Click(object sender, RoutedEventArgs e)
    {
        ActualizarButton.IsEnabled = false;
        try
        {
            await ActualizarEstadisticasAsync();
        }
        finally
        {
            ActualizarButton.IsEnabled = true;
        }
    }

    private void Cerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
