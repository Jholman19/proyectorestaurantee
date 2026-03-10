using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestauranteApp.Core;
using RestauranteApp.Data;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RestauranteApp;

public partial class SuscripcionesWindow : Window
{
    private int? _clienteId;
    private IFingerprintService? _fingerprintService;
    private bool _listening = false;
    private int _templatesCargados = 0;
    private int _templatesInvalidos = 0;
    private DateTime _ultimaHuellaProcesada = DateTime.MinValue;
    private int _ultimoClienteProcesado = -1;
    private bool _procesandoHuella = false;

    public SuscripcionesWindow()
    {
        InitializeComponent();
        Loaded += SuscripcionesWindow_Loaded;
        Closed += SuscripcionesWindow_Closed;
    }

    private async void SuscripcionesWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            InicioPicker.SelectedDate = DateTime.Today;
            DiaPicker.SelectedDate = DateTime.Today;

            await CargarCombosYClientesAsync();
            await RefrescarAsync();

            // Obtener servicio de huella (puede ser simulador o implementación real)
            _fingerprintService = App.Services.GetService(typeof(IFingerprintService)) as IFingerprintService;
            if (_fingerprintService != null)
            {
                // Evita suscripciones duplicadas si la ventana se vuelve a inicializar.
                _fingerprintService.FingerprintCaptured -= OnFingerprintCaptured;
                _fingerprintService.FingerprintCaptured += OnFingerprintCaptured;

                // ✅ CARGAR TEMPLATES DESDE LA BASE DE DATOS
                await CargarTemplatesDesdeBaseDeDatosAsync();

                // ✅ Iniciar escucha automáticamente para evitar que parezca "no lee".
                if (!_fingerprintService.IsListening)
                    await _fingerprintService.StartListeningAsync();

                _listening = _fingerprintService.IsListening;
                ListenFingerprintButton.Content = _listening ? "Detener escucha" : "Iniciar escucha";
                FingerprintStatusText.Text = _listening
                    ? $"Escuchando huella... ({_templatesCargados} huellas cargadas)"
                    : $"Escucha detenida ({_templatesCargados} huellas cargadas)";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error abriendo Suscripciones");
        }
    }

    private void SuscripcionesWindow_Closed(object? sender, EventArgs e)
    {
        if (_fingerprintService is null)
            return;

        _fingerprintService.FingerprintCaptured -= OnFingerprintCaptured;
    }

    private async Task CargarCombosYClientesAsync()
    {
        using var scope = App.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ClienteBox.ItemsSource = await db.Clientes
            .AsNoTracking()
            .OrderBy(x => x.Nombre)
            .ToListAsync();

        ComboBox.ItemsSource = await db.Combos
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Nombre)
            .ToListAsync();
    }

    private async Task RefrescarAsync()
    {
        await CargarInfoSuscripcionActivaAsync();
        await CargarConsumosDelDiaAsync();
    }

    private async Task<int?> SuscripcionActivaIdAsync(AppDbContext db, int clienteId)
    {
        return await db.Suscripciones
            .AsNoTracking()
            .Where(s => s.ClienteId == clienteId && s.Activo)
            .OrderByDescending(s => s.SuscripcionId)
            .Select(s => (int?)s.SuscripcionId)
            .FirstOrDefaultAsync();
    }

    private async Task<Suscripcion?> SuscripcionActivaAsync()
    {
        if (_clienteId is null) return null;

        using var scope = App.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await db.Suscripciones
            .Include(s => s.Combo)
            .AsNoTracking()
            .Where(s => s.ClienteId == _clienteId.Value && s.Activo)
            .OrderByDescending(s => s.SuscripcionId)
            .FirstOrDefaultAsync();
    }

    private async Task CargarInfoSuscripcionActivaAsync()
    {
        InfoText.Text = "";

        if (_clienteId is null)
        {
            InfoText.Text = "Selecciona un cliente.";
            return;
        }

        var sus = await SuscripcionActivaAsync();
        if (sus is null)
        {
            InfoText.Text = "Este cliente no tiene suscripción activa.";
            SetBotonesEnabled(false, false, false);
            return;
        }

        var combo = sus.Combo;
        var incluyeD = combo?.Desayuno == true;
        var incluyeA = combo?.Almuerzo == true;
        var incluyeC = combo?.Cena == true;

        InfoText.Text =
            $"Combo: {combo?.Nombre}\n" +
            $"Incluye: {(incluyeD ? "Desayuno " : "")}{(incluyeA ? "Almuerzo " : "")}{(incluyeC ? "Cena" : "")}\n" +
            $"Inicio: {sus.Inicio:yyyy-MM-dd}\n" +
            $"Fin: {sus.Fin:yyyy-MM-dd}\n" +
            $"Duración: {sus.DuracionDias} días\n" +
            $"Activa: {(sus.Activo ? "Sí" : "No")}";

        SetBotonesEnabled(incluyeD, incluyeA, incluyeC);
    }

    private void SetBotonesEnabled(bool d, bool a, bool c)
    {
        BtnDesayuno.IsEnabled = d;
        BtnAlmuerzo.IsEnabled = a;
        BtnCena.IsEnabled = c;
    }

    private async Task CargarConsumosDelDiaAsync()
    {
        ConsumosGrid.ItemsSource = null;

        if (_clienteId is null)
        {
            ResumenText.Text = "Selecciona un cliente.";
            return;
        }

        var dia = (DiaPicker.SelectedDate ?? DateTime.Today).Date;

        using var scope = App.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var sus = await db.Suscripciones
            .AsNoTracking()
            .Where(s => s.ClienteId == _clienteId.Value && s.Activo)
            .OrderByDescending(s => s.SuscripcionId)
            .FirstOrDefaultAsync();

        if (sus is null)
        {
            ResumenText.Text = "Sin suscripción activa.";
            return;
        }

        var consumoSvc = scope.ServiceProvider.GetRequiredService<ConsumoService>();

        // Cargar consumos del día (todos los tipos)
        var listaDesayuno = await consumoSvc.ListarDiaAsync(dia, AsistenciaRuleService.Desayuno);
        var listaAlmuerzo = await consumoSvc.ListarDiaAsync(dia, AsistenciaRuleService.Almuerzo);
        var listaCena = await consumoSvc.ListarDiaAsync(dia, AsistenciaRuleService.Cena);

        var listaTotal = listaDesayuno.Concat(listaAlmuerzo).Concat(listaCena).ToList();

        ConsumosGrid.ItemsSource = listaTotal.Select(x => new
        {
            Tipo = ObtenerNombreTipo(x.TipoComida),
            Dia = x.Dia.ToString("yyyy-MM-dd")
        }).ToList();

        // Mostrar estado de créditos por tipo de comida
        ResumenText.Text =
            $"CRÉDITOS POR COMIDA:\n" +
            $"Desayuno:  {sus.CreditosDesayunoRestantes} restantes\n" +
            $"Almuerzo:  {sus.CreditosAlmuerzoRestantes} restantes\n" +
            $"Cena:      {sus.CreditosCenaRestantes} restantes\n" +
            $"━━━━━━━━━━━━━━━━━━━━━━━━\n" +
            $"CONSUMOS HOY ({dia:dd/MM/yyyy}):\n" +
            $"Desayunos: {listaDesayuno.Count} | " +
            $"Almuerzos: {listaAlmuerzo.Count} | " +
            $"Cenas: {listaCena.Count}";
    }

    private int DuracionSeleccionada()
    {
        try
        {
            var texto = DuracionBox.Text?.Trim();
            
            if (string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show("⚠ Ingresa la duración en días.", "Validación");
                return -1;
            }

            if (!int.TryParse(texto, out var dias))
            {
                MessageBox.Show("⚠ La duración debe ser un número entero.", "Validación");
                return -1;
            }

            if (dias < 1)
            {
                MessageBox.Show("⚠ La duración mínima es 1 día.", "Validación");
                return -1;
            }

            if (dias > 365)
            {
                MessageBox.Show("⚠ La duración máxima es 365 días.", "Validación");
                return -1;
            }

            return dias;
        }
        catch
        {
            MessageBox.Show("⚠ Error al procesar la duración.", "Error");
            return -1;
        }
    }

    private async void CrearSuscripcion_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ClienteBox.SelectedValue is not int clienteId)
            {
                MessageBox.Show("⚠ Selecciona un cliente.", "Validación");
                return;
            }

            if (ComboBox.SelectedValue is not int comboId)
            {
                MessageBox.Show("⚠ Selecciona un combo.", "Validación");
                return;
            }

            var inicio = (InicioPicker.SelectedDate ?? DateTime.Today).Date;
            if (inicio < DateTime.Today)
            {
                MessageBox.Show("⚠ La fecha de inicio debe ser hoy o un día futuro.", "Validación");
                return;
            }

            var duracion = DuracionSeleccionada();
            if (duracion <= 0)
                return; // Ya mostró mensaje de error en DuracionSeleccionada()

            using var scope = App.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var combo = await db.Combos.AsNoTracking().FirstOrDefaultAsync(c => c.ComboId == comboId);

            if (combo is null)
            {
                MessageBox.Show("⚠ El combo no existe.", "Error");
                return;
            }

            // Mostrar información de créditos que se asignarán
            var credDes = combo.Desayuno ? duracion : 0;
            var credAlm = combo.Almuerzo ? duracion : 0;
            var credCen = combo.Cena ? duracion : 0;
            var totalCreditos = credDes + credAlm + credCen;

            var info = new StringBuilder();
            info.AppendLine($"NUEVA SUSCRIPCIÓN");
            info.AppendLine($"━━━━━━━━━━━━━━━━━━━━━━━");
            info.AppendLine($"Combo: {combo.Nombre}");
            info.AppendLine($"Inicio: {inicio:dd/MM/yyyy}");
            info.AppendLine($"Duración: {duracion} días");
            info.AppendLine($"Fin: {inicio.AddDays(duracion):dd/MM/yyyy}");
            info.AppendLine();
            info.AppendLine($"CRÉDITOS A ASIGNAR:");
            if (credDes > 0) info.AppendLine($"• Desayunos: {credDes}");
            if (credAlm > 0) info.AppendLine($"• Almuerzos: {credAlm}");
            if (credCen > 0) info.AppendLine($"• Cenas: {credCen}");
            info.AppendLine($"━━━━━━━━━━━━━━━━━━━━━━━");
            info.AppendLine($"TOTAL: {totalCreditos} créditos");
            info.AppendLine();
            info.AppendLine("¿Confirmar?");

            var resultado = MessageBox.Show(info.ToString(), "Confirmar Suscripción", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado != MessageBoxResult.Yes)
                return;

            var svc = scope.ServiceProvider.GetRequiredService<SuscripcionesService>();
            await svc.CrearAsync(clienteId, comboId, inicio, duracion);

            _clienteId = clienteId;

            MessageBox.Show("✓ Suscripción creada correctamente.", "Éxito");
            await RefrescarAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "No se pudo crear");
        }
    }

    private async void ProcesarVencimientos_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            using var scope = App.Services.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<SuscripcionesService>();
            await svc.ProcesarVencimientosAsync(DateTime.Today);

            MessageBox.Show("Vencimientos procesados.");
            await RefrescarAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }

    private async Task Consumir(string tipo)
    {
        if (_clienteId is null)
        {
            MessageBox.Show("Selecciona un cliente.");
            return;
        }

        var dia = (DiaPicker.SelectedDate ?? DateTime.Today).Date;

        using var scope = App.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var susId = await SuscripcionActivaIdAsync(db, _clienteId.Value);
        if (susId is null)
        {
            MessageBox.Show("El cliente no tiene suscripción activa.");
            return;
        }

        var consumoSvc = scope.ServiceProvider.GetRequiredService<ConsumoService>();
        var rulesService = scope.ServiceProvider.GetRequiredService<AsistenciaRuleService>();

        var ahora = DateTime.Now;
        var tipoComidaNum = tipo switch
        {
            "Desayuno" => AsistenciaRuleService.Desayuno,
            "Almuerzo" => AsistenciaRuleService.Almuerzo,
            "Cena" => AsistenciaRuleService.Cena,
            _ => AsistenciaRuleService.Almuerzo
        };

        // ✅ Verificar si está en horario y mostrar notificación
        var estaEnHorario = rulesService.EstaEnHorario(tipoComidaNum, ahora);
        var mensaje = estaEnHorario 
            ? $"✓ DENTRO DE HORARIO\n\nRegistrando {tipo} para el cliente.\nHora: {ahora:HH:mm}" 
            : $"⚠ FUERA DE HORARIO\n\nRegistrando {tipo} de manera manual.\nHora: {ahora:HH:mm}\n\nEsta comida está fuera del horario establecido.";

        var icono = estaEnHorario ? MessageBoxImage.Information : MessageBoxImage.Warning;
        MessageBox.Show(mensaje, "Registro de Consumo", MessageBoxButton.OK, icono);

        // ✅ Marcar como manual=true para permitir fuera de horario
        await consumoSvc.ConsumirAsync(_clienteId.Value, ahora, tipo, manual: true);

        await CargarConsumosDelDiaAsync();
    }

    private async void ConsumirDesayuno_Click(object sender, RoutedEventArgs e)
    {
        try { await Consumir("Desayuno"); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "No permitido"); }
    }

    private async void ConsumirAlmuerzo_Click(object sender, RoutedEventArgs e)
    {
        try { await Consumir("Almuerzo"); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "No permitido"); }
    }

    private async void ConsumirCena_Click(object sender, RoutedEventArgs e)
    {
        try { await Consumir("Cena"); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "No permitido"); }
    }

    private async void ClienteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            _clienteId = ClienteBox.SelectedValue as int?;
            await RefrescarAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error");
        }
    }

    // ========== MARCAR AUSENCIAS ==========
    private async Task MarcarAusencia(string tipoTexto)
    {
        try
        {
            if (_clienteId is null)
            {
                MessageBox.Show("⚠ Selecciona un cliente.", "Validación");
                return;
            }

            var dia = (DiaPicker.SelectedDate ?? DateTime.Today).Date;
            var ahora = DateTime.Now;

            using var scope = App.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var rulesService = scope.ServiceProvider.GetRequiredService<AsistenciaRuleService>();

            // Obtener el tipo de comida
            var tipo = tipoTexto switch
            {
                "Desayuno" => AsistenciaRuleService.Desayuno,
                "Almuerzo" => AsistenciaRuleService.Almuerzo,
                "Cena" => AsistenciaRuleService.Cena,
                _ => -1
            };

            if (tipo == -1)
            {
                MessageBox.Show("⚠ Tipo de comida inválido.", "Error");
                return;
            }

            // Marcar ausencia para el día seleccionado (hoy por defecto)
            // No cambiamos automáticamente a mañana - respetamos lo que el usuario eligió

            // Buscar una suscripción que cubra la fecha objetivo (dia)
            var sus = await db.Suscripciones
                .Include(s => s.Combo)
                .AsNoTracking()
                .Where(s => s.ClienteId == _clienteId.Value
                            && s.Inicio.Date <= dia
                            && s.Inicio.AddDays(s.DuracionDias).Date > dia)
                .OrderByDescending(s => s.SuscripcionId)
                .FirstOrDefaultAsync();

            if (sus is null)
            {
                MessageBox.Show("⚠ El cliente no tiene suscripción activa.", "Error");
                return;
            }

            var consumoSvc = scope.ServiceProvider.GetRequiredService<ConsumoService>();

            var confirmacion = MessageBox.Show(
                $"¿Confirmar ausencia de {tipoTexto}?\n\nNo se descenderán créditos.",
                "Marcar Ausencia",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes)
                return;

            await consumoSvc.MarcarAvisoAsync(_clienteId.Value, dia, tipoTexto, "Usuario - Aviso de ausencia");

            MessageBox.Show($"✓ Ausencia de {tipoTexto} registrada para mañana.", "Éxito");
            await CargarConsumosDelDiaAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error al marcar ausencia");
        }
    }

    private async void AusenciaDesayuno_Click(object sender, RoutedEventArgs e)
    {
        await MarcarAusencia("Desayuno");
    }

    private async void AusenciaAlmuerzo_Click(object sender, RoutedEventArgs e)
    {
        await MarcarAusencia("Almuerzo");
    }

    private async void AusenciaCena_Click(object sender, RoutedEventArgs e)
    {
        await MarcarAusencia("Cena");
    }

    private string ObtenerNombreTipo(int tipoComida)
    {
        return tipoComida switch
        {
            1 => "Desayuno",
            2 => "Almuerzo",
            3 => "Cena",
            _ => "Desconocido"
        };
    }

    /// <summary>
    /// Carga todos los templates de huellas desde la BD al servicio de reconocimiento.
    /// Se ejecuta automáticamente al iniciar la ventana.
    /// </summary>
    private async Task CargarTemplatesDesdeBaseDeDatosAsync()
    {
        try
        {
            if (_fingerprintService is null)
                return;

            using var scope = App.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var clientesConHuella = await db.Clientes
                .Where(c => c.HuellaTemplate != null && c.HuellaTemplate.Length > 0)
                .Select(c => new { c.ClienteId, c.HuellaTemplate })
                .ToListAsync();

            _templatesInvalidos = 0;

            foreach (var cliente in clientesConHuella)
            {
                if (!_fingerprintService.LoadTemplate(cliente.ClienteId, cliente.HuellaTemplate!))
                    _templatesInvalidos++;
            }

            _templatesCargados = _fingerprintService.GetLoadedTemplatesCount();

            if (_templatesCargados == 0)
                FingerprintStatusText.Text = "⚠️ No hay huellas cargadas en memoria. Registra huellas y vuelve a intentar.";
            else if (_templatesInvalidos > 0)
                FingerprintStatusText.Text = $"⚠️ {_templatesCargados} huellas activas. {_templatesInvalidos} huellas deben re-registrarse.";
            else
                FingerprintStatusText.Text = $"✓ {_templatesCargados} huellas cargadas desde la BD";
        }
        catch (Exception ex)
        {
            FingerprintStatusText.Text = $"⚠️ Error al cargar huellas: {ex.Message}";
        }
    }

    private async void DiaPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        try { await CargarConsumosDelDiaAsync(); }
        catch (Exception ex) { MessageBox.Show(ex.ToString(), "Error"); }
    }

    private async void EnrollFingerprint_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_clienteId is null)
            {
                MessageBox.Show("Selecciona un cliente antes de registrar huella.", "Error");
                return;
            }

            if (_fingerprintService is null)
            {
                MessageBox.Show("Servicio de huella no está disponible.", "Error");
                return;
            }

            var template = await _fingerprintService.EnrollAsync(_clienteId.Value);

            // Guardar la plantilla en la BD
            using var scope = App.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cliente = await db.Clientes.Where(c => c.ClienteId == _clienteId.Value).FirstOrDefaultAsync();
            if (cliente is null)
            {
                MessageBox.Show("Cliente no encontrado al guardar la huella.", "Error");
                return;
            }

            // Usar una columna que agregaremos a la entidad (tipo byte[])
            cliente.GetType().GetProperty("HuellaTemplate")?.SetValue(cliente, template);
            await db.SaveChangesAsync();

            // Asegura que quede disponible para reconocimiento inmediato.
            _fingerprintService.LoadTemplate(_clienteId.Value, template);
            _templatesCargados = _fingerprintService.GetLoadedTemplatesCount();

            MessageBox.Show("✓ Huella registrada, guardada y activada para reconocimiento inmediato.", "Éxito");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error al registrar huella");
        }
    }

    private async void ToggleListen_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_fingerprintService is null)
            {
                MessageBox.Show("Servicio de huella no está disponible.", "Error");
                return;
            }

            if (!_listening)
            {
                await _fingerprintService.StartListeningAsync();
                _listening = true;
                ListenFingerprintButton.Content = "Detener escucha";
                FingerprintStatusText.Text = "Escuchando huella... Acerca la huella al lector.";
            }
            else
            {
                await _fingerprintService.StopListeningAsync();
                _listening = false;
                ListenFingerprintButton.Content = "Iniciar escucha";
                FingerprintStatusText.Text = "Escucha detenida";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error");
        }
    }

    private async void OnFingerprintCaptured(object? sender, FingerprintEventArgs e)
    {
        var ahoraEvento = DateTime.Now;

        // Debounce: ignora lecturas repetidas del mismo cliente en ventana corta.
        if (_procesandoHuella)
            return;

        if (e.ClienteId == _ultimoClienteProcesado && (ahoraEvento - _ultimaHuellaProcesada).TotalMilliseconds < 2500)
            return;

        _procesandoHuella = true;
        _ultimoClienteProcesado = e.ClienteId;
        _ultimaHuellaProcesada = ahoraEvento;

        // Ejecutar en hilo UI
        await Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                var clienteId = e.ClienteId;
                using var scope = App.Services.CreateScope();
                var consumoSvc = scope.ServiceProvider.GetRequiredService<ConsumoService>();
                var reportesSvc = scope.ServiceProvider.GetRequiredService<ReportesService>();
                var rules = scope.ServiceProvider.GetRequiredService<AsistenciaRuleService>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var ahora = DateTime.Now;

                // Determinar tipo de comida vigente
                int tipo = -1;
                for (int t = 1; t <= 3; t++)
                {
                    if (rules.EstaEnHorario(t, ahora)) { tipo = t; break; }
                }

                if (tipo == -1)
                {
                    MessageBox.Show($"⏰ Huella detectada pero no estamos en horario de comida.", "Fuera de horario");
                    return;
                }

                // Obtener información del cliente
                var clienteDetalle = await reportesSvc.GetClienteDetalleAsync(clienteId);
                if (clienteDetalle is null)
                {
                    MessageBox.Show($"⚠️ Cliente #{clienteId} no encontrado.", "Error");
                    return;
                }

                // Obtener consumo de hoy
                var consumoHoy = await reportesSvc.GetConsumoHoyAsync(clienteId);

                // Ejecutar consumo (puede fallar si ya pasó el máximo)
                try
                {
                    await consumoSvc.ConsumirAsync(clienteId, ahora, ObtenerNombreTipo(tipo));
                }
                catch (InvalidOperationException ioex)
                {
                    // Error esperado (fuera de horario, sin créditos, máximo alcanzado, etc.)
                    var tipoUnderstandable = ObtenerNombreTipo(tipo);
                    MessageBox.Show(
                        $"❌ {clienteDetalle.Nombre}\n" +
                        $"No se pudo registrar {tipoUnderstandable.ToLower()}\n\n" +
                        $"{ioex.Message}",
                        "Consumo rechazado");
                    return;
                }

                // Recargar detalle para mostrar créditos ya actualizados tras el descuento.
                clienteDetalle = await reportesSvc.GetClienteDetalleAsync(clienteId);
                if (clienteDetalle is null)
                {
                    MessageBox.Show("⚠️ Consumo registrado, pero no se pudo refrescar el detalle del cliente.", "Aviso");
                    return;
                }

                // Recargar consumo de hoy tras registro exitoso
                consumoHoy = await reportesSvc.GetConsumoHoyAsync(clienteId);

                // Obtener estadísticas de asistencia para mostrar quién falta
                var estadisticas = await reportesSvc.GetEstadisticaComidaAsync(tipo);

                // Construir mensaje detallado
                var tipoNombre = ObtenerNombreTipo(tipo);
                var mensaje = new StringBuilder();
                
                mensaje.AppendLine($"✅ CONSUMO REGISTRADO");
                mensaje.AppendLine($"━━━━━━━━━━━━━━━━━━━━━");
                mensaje.AppendLine($"👤 {clienteDetalle.Nombre}");
                mensaje.AppendLine($"📦 Combo: {clienteDetalle.Combo}");
                mensaje.AppendLine($"🍽️ {tipoNombre}");
                mensaje.AppendLine($"💳 Se descontó 1 crédito de {tipoNombre.ToLower()}.");
                mensaje.AppendLine();
                mensaje.AppendLine($"💾 CRÉDITOS ACTUALES (ya descontados):");
                mensaje.AppendLine($"  Desayuno: {clienteDetalle.CreditosDesayuno}");
                mensaje.AppendLine($"  Almuerzo: {clienteDetalle.CreditosAlmuerzo}");
                mensaje.AppendLine($"  Cena: {clienteDetalle.CreditosCena}");
                mensaje.AppendLine();
                mensaje.AppendLine($"📊 HOY ({DateTime.Today:dd/MM}):");
                mensaje.AppendLine($"  {tipoNombre}: {(tipoNombre == "Desayuno" ? consumoHoy.DesayunosHoy : tipoNombre == "Almuerzo" ? consumoHoy.AlmuerzosHoy : consumoHoy.CenasHoy)}/2");
                mensaje.AppendLine();
                mensaje.AppendLine($"📈 ASISTENCIA {tipoNombre.ToUpper()}:");
                mensaje.AppendLine($"  {estadisticas.TotalConsumieron}/{estadisticas.TotalConSuscripcion} consumieron");
                if (estadisticas.TotalFaltantes > 0)
                {
                    mensaje.AppendLine($"  Faltantes ({estadisticas.TotalFaltantes}): " + String.Join(", ", estadisticas.ClientesFaltantes.Take(5).Select(x => x.Nombre)));
                    if (estadisticas.ClientesFaltantes.Count > 5)
                        mensaje.AppendLine($"    ... y {estadisticas.ClientesFaltantes.Count - 5} más");
                }

                MessageBox.Show(mensaje.ToString(), $"✓ {clienteDetalle.Nombre}");
                
                // Refrescar estadísticas en UI si hay cliente seleccionado
                if (_clienteId.HasValue)
                    await CargarConsumosDelDiaAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error al procesar huella:\n{ex.Message}", "Error");
            }
            finally
            {
                _procesandoHuella = false;
            }
        });
    }

    private async void Renovar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!_clienteId.HasValue)
            {
                MessageBox.Show("⚠ Selecciona un cliente primero.", "Error");
                return;
            }

            // Validar duración
            if (!int.TryParse(RenovacionBox.Text, out var dias) || dias < 1 || dias > 365)
            {
                MessageBox.Show("⚠ Duración debe estar entre 1 y 365 días.", "Error");
                RenovacionBox.Focus();
                return;
            }

            // Mostrar resumen de renovación
            using var scope = App.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var suscripcionesService = scope.ServiceProvider.GetRequiredService<SuscripcionesService>();

            var ultimaSuscripcion = await db.Suscripciones
                .Include(s => s.Combo)
                .Where(s => s.ClienteId == _clienteId.Value)
                .OrderByDescending(s => s.SuscripcionId)
                .FirstOrDefaultAsync();

            if (ultimaSuscripcion?.Combo is null)
            {
                MessageBox.Show("⚠ No hay suscripción previa para renovar.", "Error");
                return;
            }

            var combo = ultimaSuscripcion.Combo;
            var credDes = combo.Desayuno ? dias : 0;
            var credAlm = combo.Almuerzo ? dias : 0;
            var credCen = combo.Cena ? dias : 0;

            var resumen = $"RENOVACIÓN\n\n" +
                         $"Combo: {combo.Nombre}\n" +
                         $"Duración: {dias} días\n" +
                         $"Inicio: {DateTime.Today:dd/MM/yyyy}\n\n" +
                         $"Créditos:\n" +
                         $"  • Desayuno: {credDes}\n" +
                         $"  • Almuerzo: {credAlm}\n" +
                         $"  • Cena: {credCen}";

            var confirmacion = MessageBox.Show($"{resumen}\n\n¿Confirmar renovación?",
                "Renovar suscripción", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (confirmacion != MessageBoxResult.Yes)
                return;

            // Renovar
            var suscripcionNueva = await suscripcionesService.RenovarAsync(_clienteId.Value, dias);

            MessageBox.Show($"✓ Suscripción renovada por {dias} días.\n\n" +
                           $"Créditos reiniciados:\n" +
                           $"  • Desayuno: {credDes}\n" +
                           $"  • Almuerzo: {credAlm}\n" +
                           $"  • Cena: {credCen}",
                "Éxito");

            RenovacionBox.Text = "30"; // Reset input
            await CargarInfoSuscripcionActivaAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error al renovar");
        }
    }
}
