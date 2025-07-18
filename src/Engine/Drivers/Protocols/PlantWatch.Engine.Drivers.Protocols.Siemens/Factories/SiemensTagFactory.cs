using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlantWatch.Engine.Core.Models.Definitions;
using PlantWatch.Engine.Drivers.Protocols.Siemens.Models;
using PlantWatch.Engine.Drivers.Protocols.Siemens.Validators;
using S7.Net;
using S7.Net.Types;

namespace PlantWatch.Engine.Drivers.Protocols.Siemens.Factories;

public static class SiemensTagFactory
{
    private static readonly SiemensConfigurationValidator _validator = new();

    public static SiemensTag Create(Guid id, string name, string datatype, string address)
    {
        // Primero validamos la entrada
        SiemensConfigurationValidator.ValidateTagOrThrow(id, name, datatype, address);

        // Obtenemos el valor por defecto según el tipo
        var defaultValue = GetDefaultForDatatype(datatype);

        // Creamos el SiemensTag y configuramos el DataItem
        var tag = new SiemensTag(id, name, datatype, address, defaultValue)
        {
            Item = new DataItem
            {
                Count = 1,
                DataType = ParseAreaType(address),
                VarType = ParseVarType(datatype),
                DB = ParseDbNumber(address),
                StartByteAdr = ParseByteOffset(address),
                BitAdr = ParseBitOffset(address),
                Value = defaultValue
            }
        };

        return tag;
    }

    private static object GetDefaultForDatatype(string datatype)
    {
        return datatype switch
        {
            "Bool" => false,
            "Byte" => (byte)0,
            "Word" => (ushort)0,
            "DWord" => (uint)0,
            "Int" => (short)0,
            "DInt" => 0,
            "Real" => 0.0f,
            _ => throw new ArgumentException($"Unsupported datatype: {datatype}")
        };
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
        // Para DB area
        var dbMatch = Regex.Match(address, @"^DB(\d+)\.DB(X|W|D)(\d+)", RegexOptions.IgnoreCase);
        if (dbMatch.Success) return int.Parse(dbMatch.Groups[3].Value);

        // Para M/I/Q (B/W/D)
        var mwMatch = Regex.Match(address, @"^[MIQ](B|W|D)(\d+)$", RegexOptions.IgnoreCase);
        if (mwMatch.Success) return int.Parse(mwMatch.Groups[2].Value);

        // Para bits simples tipo M100.0, I0.0, Q2.3
        var mbBitMatch = Regex.Match(address, @"^[MIQ](\d+)\.(\d+)$");
        if (mbBitMatch.Success) return int.Parse(mbBitMatch.Groups[1].Value);

        throw new ArgumentException($"Invalid address format for byte offset: {address}");
    }

    private static byte ParseBitOffset(string address)
    {
        var match = Regex.Match(address, @"\.(\d+)$");
        return match.Success ? byte.Parse(match.Groups[1].Value) : (byte)0;
    }
}