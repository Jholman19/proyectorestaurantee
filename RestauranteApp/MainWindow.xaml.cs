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

public partial class MainWindow : Window
{
    private List<Cliente> _cache = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await CargarClientesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error cargando clientes:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private Cliente? Seleccionado()
        => ClientesGrid?.SelectedItem as Cliente;

    private async void Cargar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await CargarClientesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error recargando:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task CargarClientesAsync()
    {
        using var scope = App.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        _cache = await db.Clientes
            .AsNoTracking()
            .OrderByDescending(x => x.ClienteId)
            .ToListAsync();

        AplicarFiltro();
    }

    private void BuscarBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Puede dispararse mientras el XAML carga
        if (!IsLoaded) return;
        AplicarFiltro();
    }

    private void Filtro_Checked(object sender, RoutedEventArgs e)
    {
        // Puede dispararse mientras el XAML carga
        if (!IsLoaded) return;
        AplicarFiltro();
    }

    private void AplicarFiltro()
    {
        // 🔒 Guardas para evitar NullReference si el XAML no tiene esos nombres
        if (ClientesGrid is null || BuscarBox is null || SoloActivosCheck is null)
            return;

        IEnumerable<Cliente> q = _cache ?? new List<Cliente>();

        var texto = (BuscarBox.Text ?? "").Trim();
        var soloActivos = SoloActivosCheck.IsChecked == true;

        if (soloActivos)
            q = q.Where(c => c.Activo);

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var t = texto.ToLowerInvariant();
            q = q.Where(c =>
                (c.Nombre ?? "").ToLowerInvariant().Contains(t) ||
                (c.Documento ?? "").ToLowerInvariant().Contains(t) ||
                (c.Telefono ?? "").ToLowerInvariant().Contains(t));
        }

        ClientesGrid.ItemsSource = q.ToList();
    }

    private async void Nuevo_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var win = new ClienteFormWindow(this, null);
            win.Owner = this;
            win.ShowDialog();

            await CargarClientesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creando cliente:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void Editar_Click(object sender, RoutedEventArgs e)
    {
        var sel = Seleccionado();
        if (sel is null) return;

        try
        {
            var win = new ClienteFormWindow(this, sel.ClienteId);
            win.Owner = this;
            win.ShowDialog();

            await CargarClientesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error editando cliente:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void ToggleActivo_Click(object sender, RoutedEventArgs e)
    {
        var sel = Seleccionado();
        if (sel is null) return;

        try
        {
            using var scope = App.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var c = await db.Clientes.FirstOrDefaultAsync(x => x.ClienteId == sel.ClienteId);
            if (c is null) return;

            c.Activo = !c.Activo;
            await db.SaveChangesAsync();

            await CargarClientesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error activando/inactivando:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void Eliminar_Click(object sender, RoutedEventArgs e)
    {
        var sel = Seleccionado();
        if (sel is null)
        {
            MessageBox.Show("Seleccione un cliente primero.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            using var scope = App.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var cliente = await db.Clientes
                .Include(c => c.Suscripciones)
                .FirstOrDefaultAsync(x => x.ClienteId == sel.ClienteId);

            if (cliente is null)
            {
                MessageBox.Show("Cliente no encontrado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verificar si tiene suscripciones activas
            var tieneSuscripcionesActivas = cliente.Suscripciones.Any(s => s.Activo);

            if (tieneSuscripcionesActivas)
            {
                var resultado = MessageBox.Show(
                    $"El cliente '{cliente.Nombre}' tiene suscripciones activas.\n\n" +
                    "¿Desea eliminarlo de todas formas?\n\n" +
                    "ADVERTENCIA: Esto eliminará todas sus suscripciones, consumos y avisos asociados.",
                    "Confirmación requerida",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado != MessageBoxResult.Yes)
                    return;
            }
            else
            {
                var resultado = MessageBox.Show(
                    $"¿Está seguro que desea eliminar al cliente '{cliente.Nombre}'?\n\n" +
                    "Esta acción no se puede deshacer.",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado != MessageBoxResult.Yes)
                    return;
            }

            // Eliminar el cliente (Entity Framework eliminará los registros relacionados en cascada)
            db.Clientes.Remove(cliente);
            await db.SaveChangesAsync();

            MessageBox.Show($"Cliente '{cliente.Nombre}' eliminado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

            await CargarClientesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error eliminando cliente:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void ClientesGrid_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        var sel = Seleccionado();
        if (sel is null) return;

        try
        {
            var win = new ClienteFormWindow(this, sel.ClienteId);
            win.Owner = this;
            win.ShowDialog();

            await CargarClientesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error abriendo cliente:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Combos_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var win = new CombosWindow { Owner = this };
            win.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error abriendo combos:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Suscripciones_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var win = new SuscripcionesWindow { Owner = this };
            win.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error abriendo suscripciones:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Reportes_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var win = new DashboardWindow { Owner = this };
            win.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error abriendo reportes:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
