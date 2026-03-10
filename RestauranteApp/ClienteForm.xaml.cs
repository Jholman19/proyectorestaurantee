using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestauranteApp.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestauranteApp;

public partial class ClienteFormWindow : Window
{
    private readonly int? _clienteId;

    // ✅ Este constructor con 2 argumentos es el que tu MainWindow está usando
    public ClienteFormWindow(Window owner, int? clienteId)
    {
        Owner = owner;
        _clienteId = clienteId;

        InitializeComponent();
        Loaded += ClienteFormWindow_Loaded;
    }

    // ✅ Constructor extra por si en algún lado lo llamas con 1 argumento
    public ClienteFormWindow(Window owner) : this(owner, null) { }

    private async void ClienteFormWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_clienteId is not null)
                await CargarClienteAsync(_clienteId.Value);
            else
                SuscripcionInfoText.Text = "Sin suscripción activa.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }
    }

    private async Task CargarClienteAsync(int clienteId)
    {
        using var scope = App.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var c = await db.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ClienteId == clienteId);

        if (c is null)
        {
            MessageBox.Show("Cliente no encontrado.");
            return;
        }

        NombreBox.Text = c.Nombre ?? "";
        DocumentoBox.Text = c.Documento ?? "";
        TelefonoBox.Text = c.Telefono ?? "";
        ActivoCheck.IsChecked = c.Activo;

        await CargarResumenSuscripcionAsync(clienteId);
    }

    private async Task CargarResumenSuscripcionAsync(int clienteId)
    {
        using var scope = App.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var s = await db.Suscripciones
            .Include(x => x.Combo)
            .AsNoTracking()
            .Where(x => x.ClienteId == clienteId && x.Activo)
            .OrderByDescending(x => x.SuscripcionId)
            .FirstOrDefaultAsync();

        if (s is null)
        {
            SuscripcionInfoText.Text = "Sin suscripción activa.";
            return;
        }

        SuscripcionInfoText.Text =
            $"Combo: {s.Combo?.Nombre}\n" +
            $"Inicio: {s.Inicio:yyyy-MM-dd}\n" +
            $"Fin: {s.Fin:yyyy-MM-dd}\n" +
            $"Duración: {s.DuracionDias} días";
    }

    private async void Guardar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var nombre = (NombreBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Nombre es obligatorio.");
                return;
            }

            var documento = (DocumentoBox.Text ?? "").Trim();
            var telefono = (TelefonoBox.Text ?? "").Trim();
            var activo = ActivoCheck.IsChecked == true;

            using var scope = App.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (_clienteId is null)
            {
                var nuevo = new Cliente
                {
                    Nombre = nombre,
                    Documento = string.IsNullOrWhiteSpace(documento) ? null : documento,
                    Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono,
                    Activo = activo
                };

                db.Clientes.Add(nuevo);
            }
            else
            {
                var c = await db.Clientes.FirstOrDefaultAsync(x => x.ClienteId == _clienteId.Value)
                    ?? throw new InvalidOperationException("Cliente no encontrado.");

                c.Nombre = nombre;
                c.Documento = string.IsNullOrWhiteSpace(documento) ? null : documento;
                c.Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono;
                c.Activo = activo;
            }

            await db.SaveChangesAsync();

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error al guardar");
        }
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}