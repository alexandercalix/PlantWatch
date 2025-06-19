using System;
using System.Collections.Generic;

namespace PlantWatch.Core.Models.Definitions;

public class PlcConnectionDefinition
{

    public Guid Id { get; set; } = Guid.NewGuid();  // ID único asignado siempre
    public string Name { get; set; }          // Ej: "PLC_Main", "PLC_Remoto"
    public string DriverType { get; set; }        // Ej: "Siemens", "Modbus", "AllenBradley"
    public string IpAddress { get; set; }     // Ej: "192.168.0.10"
    public int Rack { get; set; } = 0;
    public int Slot { get; set; } = 1;
    public List<PlcTagDefinition> Tags { get; set; } = new();
}
