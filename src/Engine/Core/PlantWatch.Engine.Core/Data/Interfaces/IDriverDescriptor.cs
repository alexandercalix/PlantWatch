using System;
using System.Collections.Generic;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Core.Data.Interfaces;

public interface IDatabaseDriverDescriptor
{
    string DriverType { get; }
    string FriendlyName { get; }
    List<DriverParameterDefinition> Parameters { get; }
}