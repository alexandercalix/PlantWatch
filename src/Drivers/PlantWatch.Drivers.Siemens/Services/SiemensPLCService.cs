using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PlantWatch.Core.Models.Drivers;
using PlantWatch.Core.Services.Drivers;
using PlantWatch.Drivers.Siemens.Models;
using S7.Net;

namespace PlantWatch.Drivers.Siemens.Services;

public class SiemensPLCService : IPLCService
{
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

    public SiemensPLCService(string name, string ipAddress, short rack, short slot, List<SiemensTag> tags)
    {
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
        if (!IsRunning) return;

        _cts.Cancel();
        await _cycleTask;
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

    private async Task Cycle(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await _semaphore.WaitAsync(token);

                if (!_client.IsConnected)
                    await ConnectAsync();

                var items = _tags.Select(t => t.Item).ToList();

                for (int i = 0; i < items.Count; i += 19)
                {
                    var batch = items.Skip(i).Take(19).ToList();
                    await _client.ReadMultipleVarsAsync(batch);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal stop
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cycle error: {ex.Message}");
            }
            finally
            {
                if (_semaphore.CurrentCount == 0)
                    _semaphore.Release();
            }

            await Task.Delay(100); // adjustable
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
}