using System;

namespace PlantWatch.Engine.Core.Data.Models;

public class ExecutionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public object? Data { get; set; }

    public static ExecutionResult Ok(object? data = null)
        => new ExecutionResult { Success = true, Data = data };

    public static ExecutionResult Fail(string errorMessage)
        => new ExecutionResult { Success = false, ErrorMessage = errorMessage };
}