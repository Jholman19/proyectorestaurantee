using System;
using System.Threading.Tasks;

namespace RestauranteApp.Core;

public class FingerprintEventArgs : EventArgs
{
    public int ClienteId { get; }
    public FingerprintEventArgs(int clienteId) => ClienteId = clienteId;
}

public interface IFingerprintService
{
    event EventHandler<FingerprintEventArgs>? FingerprintCaptured;

    // Captura la huella para enrolar y devuelve la plantilla binaria (template)
    Task<byte[]> EnrollAsync(int clienteId);
    Task StartListeningAsync();
    Task StopListeningAsync();
    bool IsListening { get; }

    // Método útil para pruebas/simulador: dispara un escaneo
    Task SimulateScanAsync(int clienteId);

    /// <summary>
    /// Carga un template desde la BD al servicio (para HIDUareUService).
    /// Retorna true si se cargó correctamente.
    /// </summary>
    bool LoadTemplate(int clienteId, byte[] templateData);

    /// <summary>
    /// Obtiene cuántos templates están cargados en memoria.
    /// </summary>
    int GetLoadedTemplatesCount();
}
