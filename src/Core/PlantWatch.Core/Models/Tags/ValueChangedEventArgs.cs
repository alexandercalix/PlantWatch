using System;
using PlantWatch.Core.Interfaces.Engine.Models;

namespace PlantWatch.Core.Models.Tags;

public class ValueChangedEventArgs : EventArgs
{
    public object NewValue { get; }

    public ValueChangedEventArgs(ITag tag, object newValue)
    {
        NewValue = newValue;
    }
}