
using System;

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
                if (IsSignificantChange(_value, value))
                {
                    _value = value;
                    LastChangeTimestamp = DateTime.UtcNow;
                    OnValueChanged?.Invoke(this, new TagChangeEventArgs(this, value));
                }
                else
                {
                    _value = value; // silent update sin evento
                }
            }
        }

        public DateTime LastChangeTimestamp { get; private set; } = DateTime.UtcNow;

        // Deadband numérico absoluto
        public double? Deadband { get; set; }

        // Deadband temporal (por ejemplo: aunque cambie poco, notificar si pasó 1 minuto)
        public TimeSpan? DeadbandTime { get; set; }

        private DateTime _lastEventTimestamp = DateTime.UtcNow;

        public event EventHandler<TagChangeEventArgs> OnValueChanged;

        protected TagBase(Guid id, string name, string datatype, string address)
        {
            Id = id;
            Name = name;
            Datatype = datatype;
            Address = address;
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
