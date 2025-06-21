using System;
using PlantWatch.Engine.Core.Models.Drivers;
using S7.Net.Types;
namespace PlantWatch.Engine.Drivers.Siemens.Models
{


    public class SiemensTag : TagBase
    {
        public DataItem Item { get; internal set; }

        public SiemensTag(Guid id, string name, string datatype, string address, object defaultValue)
            : base(id, name, datatype, address)
        {
            Value = defaultValue;
        }


        protected override bool IsCompatibleType(object value)
        {
            if (value == null) return false;

            return Datatype switch
            {
                "Bool" => value is bool,
                "Byte" => value is byte,
                "Word" => value is ushort,
                "DWord" => value is uint,
                "Int" => value is short,
                "DInt" => value is int,
                "Real" => value is float or double,
                _ => false
            };
        }
    }
}
