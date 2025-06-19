using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PlantWatch.Core.Interfaces;
using PlantWatch.Core.Models.Drivers;
using PlantWatch.Core.Services.Drivers;
using PlantWatch.Drivers.Siemens.Models;
using S7.Net;
using S7.Net.Types;

namespace PlantWatch.Drivers.Siemens.Services;

public class SiemensPLCService : IPLCService, IDriverDiagnostics
{
    public Guid Id { get; internal set; }
    public string Name { get; private set; }
    private readonly string _plcIp;
    private readonly Plc _client;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private CancellationTokenSource _cts;
    private Task _cycleTask;

    private readonly List<SiemensTag> _tags;

    public IEnumerable<ITag> Tags => _tags;
    public bool IsConnected => _client.IsConnected;
    public bool IsRunning => _cycleTask != null && !_cycleTask.IsCompleted;

    private bool _diagnosticMode = true;
    private int _consecutiveFailures = 0;
    private const int FailureThreshold = 3; // configurable


    #region Diagnostic
    // Agregamos los campos privados:
    private System.DateTime? _lastCycleTimestamp;
    private long _lastCycleDurationMs;
    private int _totalReadErrors;
    private int _totalConnectionRetries;
    private int _bulkReadFailures;

    // Implementación de la interfaz IDriverDiagnostics:
    public System.DateTime? LastCycleTimestamp => _lastCycleTimestamp;
    public long LastCycleDurationMs => _lastCycleDurationMs;
    public int TotalReadErrors => _totalReadErrors;
    public int TotalConnectionRetries => _totalConnectionRetries;
    public int BulkReadFailures => _bulkReadFailures;
    #endregion

    public SiemensPLCService(Guid id, string name, string ipAddress, short rack, short slot, List<SiemensTag> tags)
    {
        this.Id = id;
        Name = name;
        _plcIp = ipAddress;
        _tags = tags;
        _client = new Plc(CpuType.S71200, ipAddress, rack, slot);
    }

    public async Task StartAsync()
    {
        if (IsRunning) return;

        _cts = new CancellationTokenSource();
        _cycleTask = Task.Run(() => Cycle(_cts.Token));
        await Task.Delay(100); // Allow some time for the task to start
    }

    public async Task StopAsync()
    {
        if (!IsRunning)
            return;

        _cts.Cancel();

        try
        {
            await _cycleTask;
        }
        catch (OperationCanceledException)
        {
            // esperado, no hay problema
            Console.WriteLine("[PLC] Cycle task canceled cleanly.");
        }
    }

    private async Task ConnectAsync()
    {
        if (!_client.IsConnected)
        {
            try
            {
                await _client.OpenAsync();
                Console.WriteLine("PLC connected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed to {_plcIp}: {ex.Message}");
            }
        }
    }

    private async Task<bool> RunInitialDiagnostics()
    {
        Console.WriteLine("[PLC] Running initial diagnostic pass...");

        var tagsToCheck = _tags.Where(t => !t.Disabled).ToList();
        bool allValid = true;

        foreach (var tag in tagsToCheck)
        {
            Console.WriteLine($"[PLC] Validating tag: {tag.Name}");

            try
            {
                await _client.ReadMultipleVarsAsync(new List<DataItem> { tag.Item });
                tag.Quality = tag.Item?.Value != null;
            }
            catch (Exception ex)
            {
                tag.Quality = false;

                if (ex.Message.Contains("Address out of range"))
                {
                    tag.Disabled = true;
                    Console.WriteLine($"[Tag Disabled] {tag.Name}: permanently excluded due to invalid address.");
                }
                else
                {
                    Console.WriteLine($"[Tag Error] {tag.Name}: {ex.Message}");
                }

                allValid = false;
            }
        }

        return allValid;
    }


    private async Task Cycle(CancellationToken token)
    {
        bool wasConnected = false;
        bool hasDiagnosed = false;

        while (!token.IsCancellationRequested)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await _semaphore.WaitAsync(token);

                if (!_client.IsConnected)
                {
                    Console.WriteLine("[PLC] Attempting to connect...");
                    await ConnectAsync();
                    _totalConnectionRetries++;  // << Aquí contamos los intentos
                    wasConnected = false;
                    hasDiagnosed = false;
                    await Task.Delay(1000, token);
                    continue;
                }

                if (!wasConnected)
                {
                    Console.WriteLine("[PLC] Connected. Running diagnostics...");
                    _diagnosticMode = await RunInitialDiagnostics();
                    hasDiagnosed = true;
                    wasConnected = true;
                }

                if (_diagnosticMode)
                {
                    Console.WriteLine("[PLC] Re-validating tags...");
                    _diagnosticMode = !await RunInitialDiagnostics();
                }
                else
                {
                    try
                    {
                        var items = _tags
                        .Where(t => !t.Disabled)
                        .Select(t => t.Item)
                        .ToList();

                        for (int i = 0; i < items.Count; i += 19)
                        {
                            var batch = items.Skip(i).Take(19).ToList();
                            await _client.ReadMultipleVarsAsync(batch);
                        }

                        foreach (var tag in _tags.Where(t => !t.Disabled))
                        {
                            tag.Quality = tag.Item?.Value != null;
                            tag.Value = tag.Item?.Value;
                        }

                        _consecutiveFailures = 0;
                    }
                    catch (Exception ex)
                    {
                        _consecutiveFailures++;
                        _totalReadErrors++;  // << Aquí contamos los errores de lectura
                        Console.WriteLine($"[PLC] Bulk read failed ({_consecutiveFailures}): {ex.Message}");

                        if (_consecutiveFailures >= FailureThreshold)
                        {
                            _bulkReadFailures++;  // << Aquí sumamos los bulk failures
                            Console.WriteLine("[PLC] Too many failures. Switching back to diagnostic mode.");
                            _diagnosticMode = true;
                            _consecutiveFailures = 0;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _totalReadErrors++;
                Console.WriteLine($"[Cycle Error] Unexpected: {ex.Message}");
            }
            finally
            {
                if (_semaphore.CurrentCount == 0)
                    _semaphore.Release();

                sw.Stop();
                _lastCycleDurationMs = sw.ElapsedMilliseconds;
                _lastCycleTimestamp = System.DateTime.UtcNow;
            }

            await Task.Delay(100, token);
        }

        if (_client.IsConnected)
            _client.Close();
    }


    public async Task<bool> WriteTagAsync(string tagName, object value)
    {
        var tag = _tags.FirstOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        if (tag == null)
            throw new ArgumentException($"Tag not found: {tagName}");

        try
        {
            await _semaphore.WaitAsync();

            tag.Value = value;
            await _client.WriteAsync(tag.Item);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Write error on tag '{tagName}': {ex.Message}");
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void ForceRemap()
    {
        Console.WriteLine("[PLC] Manual remap requested.");
        _diagnosticMode = true;
        _consecutiveFailures = 0;
    }
}