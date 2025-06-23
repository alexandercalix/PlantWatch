using System;

namespace PlantWatch.Engine.Applications.WebApi.DTOs;

public class TagValueDto
{
    public Guid Id { get; set; }
    public object Value { get; set; }
}
public class TagFullDto
{
    public Guid Id { get; set; }
    public Guid TagId { get; set; }
    public string Name { get; set; }
    public string Datatype { get; set; }
    public object Value { get; set; }
    public bool Quality { get; set; }
}

public class PlcConnection
{
    public Guid Id { get; set; }
    public bool IsConnected { get; set; }
}