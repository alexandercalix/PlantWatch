using System;
using System.Text.RegularExpressions;
using PlantWatch.Drivers.Siemens.Models;
using S7.Net;
using S7.Net.Types;

namespace PlantWatch.Drivers.Siemens.Factories;


public static class SiemensTagFactory
{
    public static SiemensTag Create(Guid id, string name, string datatype, string address, object value)
    {
        var validatedType = ValidateDatatype(datatype);

        var tag = new SiemensTag(id, name, validatedType, address, value)
        {
            Item = new DataItem
            {
                Count = 1,
                DataType = ParseAreaType(address),
                VarType = ParseVarType(validatedType),
                DB = ParseDbNumber(address),
                StartByteAdr = ParseByteOffset(address),
                BitAdr = ParseBitOffset(address),
                Value = value
            }
        };

        return tag;
    }

    private static string ValidateDatatype(string datatype)
    {
        string[] allowed = { "Bool", "Byte", "Word", "DWord", "Int", "DInt", "Real" };
        if (!Array.Exists(allowed, d => d.Equals(datatype, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException($"Unsupported Datatype: '{datatype}'");
        return datatype;
    }

    private static DataType ParseAreaType(string address)
    {
        if (address.StartsWith("DB")) return DataType.DataBlock;
        if (address.StartsWith("M")) return DataType.Memory;
        if (address.StartsWith("I")) return DataType.Input;
        if (address.StartsWith("Q")) return DataType.Output;

        throw new ArgumentException($"Unknown address area: {address}");
    }

    private static VarType ParseVarType(string datatype)
    {
        return datatype switch
        {
            "Bool" => VarType.Bit,
            "Byte" => VarType.Byte,
            "Word" => VarType.Word,
            "DWord" => VarType.DWord,
            "Int" => VarType.Int,
            "DInt" => VarType.DInt,
            "Real" => VarType.Real,
            _ => throw new ArgumentException($"Unsupported VarType: {datatype}")
        };
    }

    private static int ParseDbNumber(string address)
    {
        var match = Regex.Match(address, @"^DB(\d+)\.");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }

    private static int ParseByteOffset(string address)
    {
        var dbMatch = Regex.Match(address, @"^DB(\d+)\.DB[XDW](\d+)");
        if (dbMatch.Success)
            return int.Parse(dbMatch.Groups[2].Value);

        var areaMatch = Regex.Match(address, @"^[MIQ](B|W|D)?X?(\d+)(?:\.\d+)?$");
        if (areaMatch.Success)
            return int.Parse(areaMatch.Groups[2].Value);

        throw new ArgumentException($"Invalid address format for byte offset: {address}");
    }

    private static byte ParseBitOffset(string address)
    {
        var match = Regex.Match(address, @"\.(\d+)$");
        return match.Success ? byte.Parse(match.Groups[1].Value) : (byte)0;
    }
}
