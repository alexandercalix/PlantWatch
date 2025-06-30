using System;
using PlantWatch.Engine.Core.Data.Interfaces;

namespace PlantWatch.Engine.Data.Drivers.MsSql.Internals;

public class MsSqlValidator : ICommandValidator
{
    public bool Validate(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return false;

        var trimmed = command.TrimStart().ToUpperInvariant();
        return trimmed.StartsWith("SELECT") ||
               trimmed.StartsWith("INSERT") ||
               trimmed.StartsWith("UPDATE") ||
               trimmed.StartsWith("DELETE");
    }
}