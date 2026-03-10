using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestauranteApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RestauranteApp;

public partial class CombosWindow : Window
{
    private List<Combo> _cache = new();

    public CombosWindow()
    {
        InitializeComponent();
        Loaded += CombosWindow_Loaded;
    }

    private async void CombosWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await CargarCombosAsync();
    }

    private Combo? Seleccionado() => CombosGrid?.SelectedItem as Combo;

    private async void Cargar_Click(object sender, RoutedEventArgs e)
    {
        await CargarCombosAsync();
    }

    private async Task CargarCombosAsync()
    {
        using var scope = App.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        _cache = await db.Combos
            .OrderByDescending(c => c.ComboId)
            .ToListAsync();

        AplicarFiltro();
    }

    private void BuscarBox_TextChanged(object sender, TextChangedEventArgs e) => AplicarFiltro();

    private void Filtro_Checked(object sender, RoutedEventArgs e) => AplicarFiltro();

    private void AplicarFiltro()
    {
        // Proteger contra acceso a controles no inicializados
        if (BuscarBox == null || SoloActivosCheck == null || CombosGrid == null)
            return;

        IEnumerable<Combo> q = _cache;

        var texto = (BuscarBox.Text ?? "").Trim();
        var soloActivos = SoloActivosCheck.IsChecked == true;

        if (soloActivos)
            q = q.Where(c => c.Activo);

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var t = texto.ToLowerInvariant();
            q = q.Where(c => (c.Nombre ?? "").ToLowerInvariant().Contains(t));
        }

        CombosGrid.ItemsSource = q.ToList();
    }

    private async void Nuevo_Click(object sender, RoutedEventArgs e)
    {
        var nombre = (NombreBox?.Text ?? "").Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MessageBox.Show("Escribe un nombre para el combo.");
            return;
        }

        var des = DesayunoCheck?.IsChecked == true;
        var alm = AlmuerzoCheck?.IsChecked == true;
        var cena = CenaCheck?.IsChecked == true;
        var activo = ActivoCheck?.IsChecked == true;

        if (!des && !alm && !cena)
        {
            MessageBox.Show("Selecciona al menos una comida.");
            return;
        }

        using var scope = App.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var nuevo = new Combo
        {
            Nombre = nombre,
            Desayuno = des,
            Almuerzo = alm,
            Cena = cena,
            Activo = activo
        };

        db.Combos.Add(nuevo);
        await db.SaveChangesAsync();

        LimpiarFormulario();
        await CargarCombosAsync();
    }

    private async void Editar_Click(object sender, RoutedEventArgs e)
    {
        var sel = Seleccionado();
        if (sel is null) return;

        var nombre = (NombreBox?.Text ?? "").Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MessageBox.Show("Escribe un nombre para el combo.");
            return;
        }

        var des = DesayunoCheck?.IsChecked == true;
        var alm = AlmuerzoCheck?.IsChecked == true;
        var cena = CenaCheck?.IsChecked == true;
        var activo = ActivoCheck?.IsChecked == true;

        if (!des && !alm && !cena)
        {
            MessageBox.Show("Selecciona al menos una comida.");
            return;
        }

        using var scope = App.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var combo = await db.Combos.FirstOrDefaultAsync(x => x.ComboId == sel.ComboId);
        if (combo is null) return;

        combo.Nombre = nombre;
        combo.Desayuno = des;
        combo.Almuerzo = alm;
        combo.Cena = cena;
        combo.Activo = activo;

        await db.SaveChangesAsync();

        LimpiarFormulario();
        await CargarCombosAsync();
    }

    private async void ToggleActivo_Click(object sender, RoutedEventArgs e)
    {
        var sel = Seleccionado();
        if (sel is null) return;

        using var scope = App.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var combo = await db.Combos.FirstOrDefaultAsync(x => x.ComboId == sel.ComboId);
        if (combo is null) return;

        combo.Activo = !combo.Activo;
        await db.SaveChangesAsync();

        await CargarCombosAsync();
    }

    private void CombosGrid_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        var sel = Seleccionado();
        if (sel is null) return;

        if (NombreBox != null) NombreBox.Text = sel.Nombre ?? "";
        if (DesayunoCheck != null) DesayunoCheck.IsChecked = sel.Desayuno;
        if (AlmuerzoCheck != null) AlmuerzoCheck.IsChecked = sel.Almuerzo;
        if (CenaCheck != null) CenaCheck.IsChecked = sel.Cena;
        if (ActivoCheck != null) ActivoCheck.IsChecked = sel.Activo;
    }

    private string ObtenerTipoComida()
    {
        var des = DesayunoCheck?.IsChecked == true;
        var alm = AlmuerzoCheck?.IsChecked == true;
        var cena = CenaCheck?.IsChecked == true;

        if (alm) return "Almuerzo";
        if (cena) return "Cena";
        return "Desayuno";
    }

    private void SeleccionarTipoComida(Combo combo)
    {
        if (DesayunoCheck != null) DesayunoCheck.IsChecked = combo.Desayuno;
        if (AlmuerzoCheck != null) AlmuerzoCheck.IsChecked = combo.Almuerzo;
        if (CenaCheck != null) CenaCheck.IsChecked = combo.Cena;
    }

    private void LimpiarFormulario()
    {
        if (NombreBox != null) NombreBox.Text = "";
        if (DesayunoCheck != null) DesayunoCheck.IsChecked = false;
        if (AlmuerzoCheck != null) AlmuerzoCheck.IsChecked = false;
        if (CenaCheck != null) CenaCheck.IsChecked = false;
        if (ActivoCheck != null) ActivoCheck.IsChecked = true;
    }
}