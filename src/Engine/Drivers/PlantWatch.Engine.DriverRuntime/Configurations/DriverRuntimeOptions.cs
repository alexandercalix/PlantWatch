using System;

namespace PlantWatch.Engine.DriverRuntime.Configurations;

public class DriverRuntimeOptions
{
    public string LiteDbPath { get; set; } = "plantwatch.db";
    public string LiteDbPassword { get; set; } = "abcd1234";
}