
using System;
using System.Text.Json;

namespace PlantWatch.Engine.Core.Models.Drivers
{
    public abstract class TagBase : ITag
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Datatype { get; init; }
        public string Address { get; init; }
        public bool Quality { get; set; }
        public bool Disabled { get; set; } = false;

        private object _value;
        public object Value
        {
            get => _value;
            set
            {
                object convertedValue = ConvertToInternalType(value);

                if (IsSignificantChange(_value, convertedValue))
                {
                    _value = convertedValue;
                    OnValueAssigned(convertedValue);
                    LastChangeTimestamp = DateTime.UtcNow;
                    OnValueChanged?.Invoke(this, new TagChangeEventArgs(this, convertedValue));
                }
                else
                {
                    _value = convertedValue;
                }
            }
        }

        public DateTime LastChangeTimestamp { get; private set; } = DateTime.UtcNow;

        // Deadband numérico absoluto
        public double? Deadband { get; set; }

        // Deadband temporal (por ejemplo: aunque cambie poco, notificar si pasó 1 minuto)
        public TimeSpan? DeadbandTime { get; set; }

        private DateTime _lastEventTimestamp = DateTime.UtcNow;
        protected virtual void OnValueAssigned(object convertedValue) { }

        public event EventHandler<TagChangeEventArgs> OnValueChanged;

        protected TagBase(Guid id, string name, string datatype, string address)
        {
            Id = id;
            Name = name;
            Datatype = datatype;
            Address = address;
        }

        /// <summary>
        /// Verifica si el valor es compatible con el tipo del tag
        /// </summary>
        private object ConvertToInternalType(object input)
        {
            if (input is JsonElement jsonElement)
            {
                switch (Datatype.ToLower())
                {
                    case "bool": return jsonElement.GetBoolean();
                    case "byte":
                    case "word":
                    case "dword":
                    case "int":
                    case "dint": return jsonElement.GetInt32();
                    case "real": return jsonElement.GetDouble();
                    default: throw new InvalidOperationException($"Unsupported datatype: {Datatype}");
                }
            }
            else
            {
                try
                {
                    switch (Datatype.ToLower())
                    {
                        case "bool": return Convert.ToBoolean(input);
                        case "byte":
                        case "word":
                        case "dword":
                        case "int":
                        case "dint": return Convert.ToInt32(input);
                        case "real": return Convert.ToDouble(input);
                        default: throw new InvalidOperationException($"Unsupported datatype: {Datatype}");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to convert value '{input}' to datatype '{Datatype}': {ex.Message}");
                }
            }
        }
        /// <summary>
        /// Detecta si el nuevo valor es un cambio significativo
        /// </summary>
        protected bool IsSignificantChange(object oldValue, object newValue)
        {
            if (oldValue == null || newValue == null)
                return true;

            if (Datatype == "Real" || Datatype == "DInt" || Datatype == "Int" || Datatype == "Double")
            {
                double oldVal = Convert.ToDouble(oldValue);
                double newVal = Convert.ToDouble(newValue);
                double delta = Math.Abs(newVal - oldVal);

                if (Deadband.HasValue && delta < Deadband.Value)
                {
                    if (DeadbandTime.HasValue && DateTime.UtcNow - _lastEventTimestamp >= DeadbandTime.Value)
                    {
                        _lastEventTimestamp = DateTime.UtcNow;
                        return true; // forzar evento por tiempo
                    }
                    return false; // cambio muy pequeño y dentro del tiempo
                }
                else
                {
                    _lastEventTimestamp = DateTime.UtcNow;
                    return true; // cambio significativo
                }
            }

            // Para booleanos o strings
            if (!oldValue.Equals(newValue))
            {
                _lastEventTimestamp = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validación de compatibilidad de tipos (se implementa en cada driver concreto)
        /// </summary>
        protected abstract bool IsCompatibleType(object value);
    }

    /// <summary>
    /// Argumento de evento para cambios de tag
    /// </summary>
    public class TagChangeEventArgs : EventArgs
    {
        public ITag Tag { get; }
        public object NewValue { get; }

        public TagChangeEventArgs(ITag tag, object newValue)
        {
            Tag = tag;
            NewValue = newValue;
        }
    }
}
