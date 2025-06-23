using System;

namespace PlantWatch.Engine.Core.Interfaces;

public interface IDriverDiagnostics
{
    Guid Id { get; }
    bool IsConnected { get; }
    bool IsRunning { get; }
    DateTime? LastCycleTimestamp { get; }
    long LastCycleDurationMs { get; }
    int TotalReadErrors { get; }
    int TotalConnectionRetries { get; }
    int BulkReadFailures { get; }
}