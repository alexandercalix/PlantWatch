using System;

namespace PlantWatch.DriverRuntime.Models;

public class RuntimeTagSnapshot
{
    public Guid Id { get; set; }
    public string TagName { get; set; }
    public string Datatype { get; set; }
    public object Value { get; set; }
    public bool Quality { get; set; }
}