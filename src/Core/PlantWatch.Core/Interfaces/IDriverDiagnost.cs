using System;

namespace PlantWatch.Core.Interfaces;

public interface IDriverDiagnostics
{
    bool IsConnected { get; }
    bool IsRunning { get; }
    DateTime? LastCycleTimestamp { get; }
    long LastCycleDurationMs { get; }
    int TotalReadErrors { get; }
    int TotalConnectionRetries { get; }
    int BulkReadFailures { get; }
}