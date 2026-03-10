using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestauranteApp.Data;
using RestauranteApp.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RestauranteApp;

public partial class App : Application
{
    private IHost _host = null!;
    private System.Threading.Timer? _timerPerdidas;

    public static IServiceProvider Services =>
        ((App)Current)._host.Services;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        SetupGlobalExceptionHandlers();

        try
        {
            LogInfo("OnStartup: iniciando...");

            _host = CreateHostBuilder().Build();
            LogInfo("Host: build OK");

            await _host.StartAsync();
            LogInfo("Host: start OK");

            await ApplyMigrationsAsync();
            LogInfo("Migraciones: OK");

            await InicializarHuellaServiceAsync();
            LogInfo("Huella: inicialización OK");

            // ✅ Procesar pérdidas al iniciar (NO rompe compilación si el método no existe)
            await ProcesarPerdidasAlInicioSafeAsync();

            // ✅ Iniciar timer para procesar pérdidas automáticamente cada 30 minutos
            IniciarTimerPerdidas();

            var main = new MainWindow();
            LogInfo("MainWindow: creada");

            MainWindow = main;
            main.Show();

            LogInfo("MainWindow: mostrada");
        }
        catch (Exception ex)
        {
            Log(ex);

            MessageBox.Show(
                "Falló el inicio de la app.\n\n" +
                "DETALLE COMPLETO:\n" +
                ex + "\n\n" +
                "Log:\n" + GetLogPath(),
                "Error al iniciar",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {// Detener timer de pérdidas
            _timerPerdidas?.Dispose();

            
        try
        {
            if (_host != null)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(2));
                _host.Dispose();
            }
        }
        catch (Exception ex)
        {
            Log(ex);
        }

        base.OnExit(e);
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // ✅ DB estable en LocalAppData
                var dbPath = GetDbPath();
                LogInfo("DB PATH (GetDbPath): " + dbPath);

                var dir = Path.GetDirectoryName(dbPath);
                if (string.IsNullOrWhiteSpace(dir))
                    throw new InvalidOperationException("No se pudo determinar el directorio de la BD.");

                Directory.CreateDirectory(dir);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite($"Data Source={dbPath}"));

                // ✅ Servicios Core (SIN cambiar nombres)
                services.AddScoped<SuscripcionesService>();
                services.AddScoped<AsistenciaRuleService>();
                services.AddScoped<ConsumoService>();
                services.AddScoped<ReportesService>();
                // Servicio de huella (lector HID U.are.U 4500 con One Touch SDK)
                services.AddSingleton<IFingerprintService, RestauranteApp.Device.HIDUareUService>();
            });
    }

    private async Task ApplyMigrationsAsync()
    {
        using var scope = _host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    private async Task InicializarHuellaServiceAsync()
    {
        try
        {
            var fp = _host.Services.GetService(typeof(IFingerprintService)) as IFingerprintService;
            if (fp is null)
            {
                LogInfo("Huella: IFingerprintService no registrado.");
                return;
            }

            using var scope = _host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var clientesConHuella = await db.Clientes
                .Where(c => c.HuellaTemplate != null && c.HuellaTemplate.Length > 0)
                .Select(c => new { c.ClienteId, c.HuellaTemplate })
                .ToListAsync();

            var cargadasOk = 0;
            foreach (var c in clientesConHuella)
            {
                if (fp.LoadTemplate(c.ClienteId, c.HuellaTemplate!))
                    cargadasOk++;
            }

            await fp.StartListeningAsync();
            LogInfo($"Huella: escucha auto-iniciada. Templates BD={clientesConHuella.Count}, cargadas={cargadasOk}.");
        }
        catch (Exception ex)
        {
            Log(ex);
            LogInfo("Huella: fallo inicialización automática (se continúa). ");
        }
    }

    /// <summary>
    /// Intenta llamar ConsumoService.ProcesarPerdidasAsync(...)
    /// sin romper compilación si aún no existe el método.
    /// </summary>
    private async Task ProcesarPerdidasAlInicioSafeAsync()
    {
        try
        {
            using var scope = _host.Services.CreateScope();
            var consumoService = scope.ServiceProvider.GetRequiredService<ConsumoService>();

            // Busca:
            // - ProcesarPerdidasAsync(DateTime)
            // - ProcesarPerdidasAsync()
            var t = consumoService.GetType();

            MethodInfo? mi =
                t.GetMethod("ProcesarPerdidasAsync", new[] { typeof(DateTime) }) ??
                t.GetMethod("ProcesarPerdidasAsync", Type.EmptyTypes);

            if (mi == null)
            {
                LogInfo("ProcesarPerdidasAlInicio: método ProcesarPerdidasAsync NO existe aún. Se omite.");
                return;
            }

            object? result;
            if (mi.GetParameters().Length == 1)
                result = mi.Invoke(consumoService, new object[] { DateTime.Now });
            else
                result = mi.Invoke(consumoService, null);

            if (result is Task task)
                await task;

            LogInfo("ProcesarPerdidasAlInicio: OK");
        }
        catch (Exception ex)
        {
            // No queremos que la app muera por esto, pero sí loguear
            Log(ex);
            LogInfo("ProcesarPerdidasAlInicio: falló (se continúa).");
        }
    }

    /// <summary>
    /// Inicia un timer que ejecuta ProcesarPerdidasAsync cada 30 minutos
    /// </summary>
    private void IniciarTimerPerdidas()
    {
        try
        {
            // Ejecutar cada 30 minutos
            var intervalo = TimeSpan.FromMinutes(30);

            _timerPerdidas = new System.Threading.Timer(
                TimerPerdidas_Callback,
                null,
                intervalo,  // Primer disparo después de 30 minutos
                intervalo); // Luego cada 30 minutos

            LogInfo($"Timer de pérdidas automáticas: iniciado (cada {intervalo.TotalMinutes} minutos)");
        }
        catch (Exception ex)
        {
            Log(ex);
            LogInfo("Timer de pérdidas: falló al iniciar (se continúa sin timer).");
        }
    }

    /// <summary>
    /// Callback del timer que procesa pérdidas automáticas
    /// </summary>
    private async void TimerPerdidas_Callback(object? state)
    {
        try
        {
            LogInfo("Timer de pérdidas: ejecutando...");

            using var scope = _host.Services.CreateScope();
            var consumoService = scope.ServiceProvider.GetRequiredService<ConsumoService>();

            await consumoService.ProcesarPerdidasAsync(DateTime.Now);

            LogInfo("Timer de pérdidas: OK");
        }
        catch (Exception ex)
        {
            Log(ex);
            LogInfo("Timer de pérdidas: error al procesar (se continúa).");
        }
    }

    private void SetupGlobalExceptionHandlers()
    {
        DispatcherUnhandledException += (s, e) =>
        {
            Log(e.Exception);

            MessageBox.Show(
                "Error inesperado (UI Thread)\n\n" +
                e.Exception + "\n\nLog:\n" + GetLogPath(),
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            if (e.ExceptionObject is Exception ex)
                Log(ex);
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            Log(e.Exception);
            e.SetObserved();
        };
    }

    private static string GetDbPath()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RestauranteApp");

        return Path.Combine(folder, "restaurante.db");
    }

    private static string GetLogPath()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RestauranteApp");

        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "app.log");
    }

    private static void Log(Exception ex)
    {
        try
        {
            File.AppendAllText(GetLogPath(),
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] EXCEPTION\n{ex}\n\n");
        }
        catch { }
    }

    private static void LogInfo(string msg)
    {
        try
        {
            File.AppendAllText(GetLogPath(),
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO {msg}\n");
        }
        catch { }
    }
}