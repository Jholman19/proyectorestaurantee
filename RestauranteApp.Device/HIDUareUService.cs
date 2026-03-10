#if USE_HID_SDK
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RestauranteApp.Core;

namespace RestauranteApp.Device;

public class HIDUareUService : IFingerprintService, DPFP.Capture.EventHandler
{
    private readonly object _sync = new();
    private readonly DPFP.Capture.Capture _capturer;
    private readonly Dictionary<int, DPFP.Template> _templates = new();

    private DPFP.Processing.Enrollment? _enroller;
    private int? _enrollingClienteId;
    private TaskCompletionSource<byte[]>? _enrollTcs;
    private bool _captureStarted;

    public event EventHandler<FingerprintEventArgs>? FingerprintCaptured;

    public bool IsListening { get; private set; }

    public HIDUareUService()
    {
        _capturer = new DPFP.Capture.Capture
        {
            EventHandler = this
        };
    }

    public Task<byte[]> EnrollAsync(int clienteId)
    {
        lock (_sync)
        {
            if (_enrollTcs != null)
                throw new InvalidOperationException("Ya hay un proceso de enrolamiento en curso.");

            _enrollingClienteId = clienteId;
            _enroller = new DPFP.Processing.Enrollment();
            _enrollTcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
            EnsureCaptureStarted();
        }

        return _enrollTcs.Task;
    }

    public Task StartListeningAsync()
    {
        lock (_sync)
        {
            IsListening = true;
            EnsureCaptureStarted();
        }

        return Task.CompletedTask;
    }

    public Task StopListeningAsync()
    {
        lock (_sync)
        {
            IsListening = false;
            if (_enrollTcs == null)
                StopCaptureInternal();
        }

        return Task.CompletedTask;
    }

    public Task SimulateScanAsync(int clienteId)
    {
        if (IsListening && _templates.ContainsKey(clienteId))
            FingerprintCaptured?.Invoke(this, new FingerprintEventArgs(clienteId));

        return Task.CompletedTask;
    }

    /// <summary>
    /// Carga un template desde la base de datos al diccionario de templates en memoria.
    /// Debe llamarse al iniciar la aplicación para cargar todas las huellas registradas.
    /// </summary>
    public bool LoadTemplate(int clienteId, byte[] templateData)
    {
        if (templateData == null || templateData.Length == 0)
            return false;

        try
        {
            var template = new DPFP.Template();
            using var ms = new MemoryStream(templateData);
            template.DeSerialize(ms);

            lock (_sync)
            {
                _templates[clienteId] = template;
            }

            return true;
        }
        catch (Exception)
        {
            // Si falla la deserialización, simplemente no cargamos ese template
            return false;
        }
    }

    /// <summary>
    /// Obtiene el número de templates cargados en memoria.
    /// </summary>
    public int GetLoadedTemplatesCount()
    {
        lock (_sync)
        {
            return _templates.Count;
        }
    }

    private void EnsureCaptureStarted()
    {
        if (_captureStarted)
            return;

        _capturer.StartCapture();
        _captureStarted = true;
    }

    private void StopCaptureInternal()
    {
        if (!_captureStarted)
            return;

        _capturer.StopCapture();
        _captureStarted = false;
    }

    private static DPFP.FeatureSet? ExtractFeatures(DPFP.Sample sample, DPFP.Processing.DataPurpose purpose)
    {
        var extractor = new DPFP.Processing.FeatureExtraction();
        var feedback = DPFP.Capture.CaptureFeedback.None;
        var features = new DPFP.FeatureSet();

        extractor.CreateFeatureSet(sample, purpose, ref feedback, ref features);
        return feedback == DPFP.Capture.CaptureFeedback.Good ? features : null;
    }

    public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
    {
        Task.Run(() => HandleSample(Sample));
    }

    private void HandleSample(DPFP.Sample sample)
    {
        TaskCompletionSource<byte[]>? enrollTcs;
        int? enrollingClienteId;
        DPFP.Processing.Enrollment? enroller;
        bool listening;

        lock (_sync)
        {
            enrollTcs = _enrollTcs;
            enrollingClienteId = _enrollingClienteId;
            enroller = _enroller;
            listening = IsListening;
        }

        if (enrollTcs != null && enrollingClienteId.HasValue && enroller != null)
        {
            var featuresEnroll = ExtractFeatures(sample, DPFP.Processing.DataPurpose.Enrollment);
            if (featuresEnroll == null)
                return;

            try
            {
                enroller.AddFeatures(featuresEnroll);
            }
            catch (Exception ex)
            {
                lock (_sync)
                {
                    _enroller?.Clear();
                    _enroller = null;
                    _enrollingClienteId = null;
                    _enrollTcs = null;
                    if (!IsListening)
                        StopCaptureInternal();
                }
                enrollTcs.TrySetException(new InvalidOperationException("Falló el enrolamiento de huella.", ex));
                return;
            }

            if (enroller.TemplateStatus == DPFP.Processing.Enrollment.Status.Ready)
            {
                var template = enroller.Template;
                using var ms = new MemoryStream();
                template.Serialize(ms);
                var bytes = ms.ToArray();

                lock (_sync)
                {
                    _templates[enrollingClienteId.Value] = template;
                    _enroller = null;
                    _enrollingClienteId = null;
                    _enrollTcs = null;
                    if (!IsListening)
                        StopCaptureInternal();
                }

                enrollTcs.TrySetResult(bytes);
                return;
            }

            if (enroller.TemplateStatus == DPFP.Processing.Enrollment.Status.Failed)
            {
                lock (_sync)
                {
                    _enroller?.Clear();
                    _enroller = null;
                    _enrollingClienteId = null;
                    _enrollTcs = null;
                    if (!IsListening)
                        StopCaptureInternal();
                }

                enrollTcs.TrySetException(new InvalidOperationException("No se pudo crear una plantilla válida. Repite el registro."));
                return;
            }
        }

        if (!listening)
            return;

        var featuresVerify = ExtractFeatures(sample, DPFP.Processing.DataPurpose.Verification);
        if (featuresVerify == null)
            return;

        var verifier = new DPFP.Verification.Verification();
        foreach (var pair in _templates)
        {
            var result = new DPFP.Verification.Verification.Result();
            verifier.Verify(featuresVerify, pair.Value, ref result);
            if (result.Verified)
            {
                FingerprintCaptured?.Invoke(this, new FingerprintEventArgs(pair.Key));
                break;
            }
        }
    }

    public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
    public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
    public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }
    public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }
    public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback) { }
}
#endif
