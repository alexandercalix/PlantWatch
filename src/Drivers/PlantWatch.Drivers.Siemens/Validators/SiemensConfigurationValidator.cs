using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlantWatch.Core.Models.Definitions;
using PlantWatch.Core.Validators;

namespace PlantWatch.Drivers.Siemens.Validators;

public class SiemensConfigurationValidator : IConfigurationValidator
{
    private static readonly string[] AllowedDatatypes = { "Bool", "Byte", "Word", "DWord", "Int", "DInt", "Real" };

    // ---- Interface implementation ----

    public Task ValidatePlcDefinitionAsync(PlcConnectionDefinition plcDefinition)
    {
        ValidatePlcDefinition(plcDefinition);
        return Task.CompletedTask;
    }

    public Task ValidateTagAsync(PlcTagDefinition tag)
    {
        ValidateTag(tag);
        return Task.CompletedTask;
    }

    // ---- Public static reusable helpers ----

    public static void ValidatePlcDefinition(PlcConnectionDefinition plcDefinition)
    {
        if (plcDefinition.Id == Guid.Empty)
            throw new ArgumentException("PLC ID is required.");

        if (string.IsNullOrWhiteSpace(plcDefinition.Name))
            throw new ArgumentException("PLC name is required.");

        if (string.IsNullOrWhiteSpace(plcDefinition.IpAddress))
            throw new ArgumentException("PLC IP address is required.");

        if (plcDefinition.Tags == null)
            throw new ArgumentException("PLC must have a Tags list (even if empty).");

        foreach (var tag in plcDefinition.Tags)
            ValidateTag(tag);
    }

    public static void ValidateTag(PlcTagDefinition tag)
    {
        if (tag.Id == Guid.Empty)
            throw new ArgumentException("Tag ID is required.");

        if (string.IsNullOrWhiteSpace(tag.Name))
            throw new ArgumentException("Tag name is required.");

        if (!AllowedDatatypes.Contains(tag.Datatype, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Unsupported datatype: {tag.Datatype}");

        if (string.IsNullOrWhiteSpace(tag.Address))
            throw new ArgumentException("Tag address is required.");

        if (!IsValidAddressFormat(tag.Address))
            throw new ArgumentException($"Invalid Siemens address format: {tag.Address}");
    }

    public static void ValidateTagOrThrow(Guid id, string name, string datatype, string address)
    {
        ValidateTag(new PlcTagDefinition
        {
            Id = id,
            Name = name,
            Datatype = datatype,
            Address = address
        });
    }

    // ---- Address pattern validation ----

    private static bool IsValidAddressFormat(string address)
    {
        // Valid DB format: DB1.DBX0.0, DB1.DBW2, DB1.DBD4
        if (Regex.IsMatch(address, @"^DB\d+\.DB(X|W|D)(\d+)(\.(\d+))?$", RegexOptions.IgnoreCase))
            return true;

        // Valid Memory/Input/Output: MB100, MW100, MD100
        if (Regex.IsMatch(address, @"^[MIQ](B|W|D)(\d+)$", RegexOptions.IgnoreCase))
            return true;

        // Valid Memory/Input/Output Bit: M100.0, I20.3, Q50.1
        if (Regex.IsMatch(address, @"^[MIQ](\d+)\.(\d+)$", RegexOptions.IgnoreCase))
            return true;

        return false;
    }
}