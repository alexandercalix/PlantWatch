using System;
using PlantWatch.Core.Models.Tags;

namespace PlantWatch.Core.Interfaces.Engine.Models;

public interface ITag
{
    Guid Id { get; }
    string Name { get; }
    string Datatype { get; }
    string Address { get; }
    bool Quality { get; protected set; }
    object Value { get; set; }
    bool Disabled { get; set; }

    double? Deadband { get; set; }
    TimeSpan? DeadbandTime { get; set; }
    DateTime LastChangeTimestamp { get; }

    event EventHandler<ValueChangedEventArgs> OnValueChanged;
}


