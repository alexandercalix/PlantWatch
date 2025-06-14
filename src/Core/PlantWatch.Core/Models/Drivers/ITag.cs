using System;

namespace PlantWatch.Core.Models.Drivers
{
    public interface ITag
    {
        string Name { get; }
        string Datatype { get; }
        string Address { get; }
        bool Quality { get; }
        object Value { get; set; }
    }
}
