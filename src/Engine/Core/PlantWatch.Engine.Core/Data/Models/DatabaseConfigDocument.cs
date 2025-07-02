using System;
using System.Collections.Generic;

namespace PlantWatch.Engine.Core.Data.Models;

public class DatabaseConfigDocument
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string DriverType { get; set; } = string.Empty;

    // Dynamic parameters such as ConnectionString, Host, Port, Token, etc.
    public Dictionary<string, string> Parameters { get; set; } = new();
}