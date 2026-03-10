using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using RestauranteApp.Core;

namespace RestauranteApp.Device;

public class FingerprintSimulator : IFingerprintService
{
    private readonly ConcurrentDictionary<int, bool> _enrolled = new();

    public event EventHandler<FingerprintEventArgs>? FingerprintCaptured;

    public bool IsListening { get; private set; }

    public Task<byte[]> EnrollAsync(int clienteId)
    {
        // Generar una "plantilla" simulada (no es segura, solo para pruebas)
        var template = BitConverter.GetBytes(clienteId);
        _enrolled[clienteId] = true;
        return Task.FromResult(template);
    }

    public Task StartListeningAsync()
    {
        IsListening = true;
        return Task.CompletedTask;
    }

    public Task StopListeningAsync()
    {
        IsListening = false;
        return Task.CompletedTask;
    }

    public bool LoadTemplate(int clienteId, byte[] templateData)
    {
        // Simulador no necesita cargar templates reales
        return true;
    }

    public int GetLoadedTemplatesCount()
    {
        // Simulador siempre dice que tiene templates
        return 999;
    }

    public Task SimulateScanAsync(int clienteId)
    {
        if (IsListening && _enrolled.ContainsKey(clienteId))
        {
            FingerprintCaptured?.Invoke(this, new FingerprintEventArgs(clienteId));
        }

        return Task.CompletedTask;
    }
}
