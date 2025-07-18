using System;

namespace PlantWatch.Engine.Applications.Common.DTOs;

public class EngineOperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public dynamic Data { get; set; }
}