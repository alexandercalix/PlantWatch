using System;

namespace PlantWatch.Engine.Core.Common;

public class StorageConfigurationOptions
{
    public string LiteDbPath { get; set; } = "data/plantwatch.db";
    public string LiteDbPassword { get; set; } = "abcd1234";
}