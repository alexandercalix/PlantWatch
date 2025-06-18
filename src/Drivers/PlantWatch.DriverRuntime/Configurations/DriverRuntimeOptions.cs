using System;

namespace PlantWatch.DriverRuntime.Configurations;

public class DriverRuntimeOptions
{
    public string LiteDbPath { get; set; } = "plantwatch.db";
    public string LiteDbPassword { get; set; } = "abcd1234";
}