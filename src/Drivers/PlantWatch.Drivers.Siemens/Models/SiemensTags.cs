using System;
using PlantWatch.Core.Models.Drivers;
using S7.Net.Types;
namespace PlantWatch.Drivers.Siemens.Models
{


    public class SiemensTag : ITag
    {
        public string Name { get; set; }
        public string Datatype { get; init; }
        public string Address { get; init; }
        public bool Quality { get; internal set; }
        public bool Disabled { get; set; } = false;


        private object _value;

        public object Value
        {
            get => Item?.Value ?? _value;
            set
            {
                if (!IsCompatibleType(value))
                    throw new InvalidCastException($"Value type '{value?.GetType().Name}' is not compatible with PLC type '{Datatype}'.");

                _value = value;
                if (Item != null)
                    Item.Value = value;
            }
        }

        public DataItem Item { get; internal set; }

        internal SiemensTag(string name, string datatype, string address, object value)
        {
            Name = name;
            Datatype = datatype;
            Address = address;
            _value = value;
        }

        private bool IsCompatibleType(object value)
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
