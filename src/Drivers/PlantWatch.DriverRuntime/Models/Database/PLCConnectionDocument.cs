using System;
using LiteDB;

namespace PlantWatch.DriverRuntime.Models.Database;

public class PlcTagDocument
{
    [BsonId]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Datatype { get; set; }
    public string Address { get; set; }
}

public class PlcConnectionDocument
{
    [BsonId]
    public string Name { get; set; }
    public string DriverType { get; set; }
    public string IpAddress { get; set; }
    public int Rack { get; set; }
    public int Slot { get; set; }

    public List<PlcTagDocument> Tags { get; set; } = new();
}