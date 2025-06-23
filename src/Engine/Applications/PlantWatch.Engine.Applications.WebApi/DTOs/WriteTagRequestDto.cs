using System;

namespace PlantWatch.Engine.Applications.WebApi.DTOs;

public class WriteTagRequest
{
    public Guid PlcId { get; set; }
    public Guid TagId { get; set; }
    public object Value { get; set; }
}
