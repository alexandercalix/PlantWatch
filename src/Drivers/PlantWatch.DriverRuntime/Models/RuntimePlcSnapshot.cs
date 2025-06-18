namespace PlantWatch.DriverRuntime.Models;

public class RuntimePlcSnapshot
{
    public string PlcName { get; set; }
    public bool IsConnected { get; set; }
    public IEnumerable<RuntimeTagSnapshot> Tags { get; set; }
}