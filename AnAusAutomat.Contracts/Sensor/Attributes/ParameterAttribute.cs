using System;

namespace AnAusAutomat.Contracts.Sensor.Attributes
{
    [Obsolete("readme.md is the future.")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ParameterAttribute : Attribute
    {
        public ParameterAttribute(string name, Type type, string defaultValue) : this(name, type, defaultValue, "")
        {
        }

        public ParameterAttribute(string name, Type type, string defaultValue, string format)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
            Format = format;
        }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public string DefaultValue { get; private set; }

        public string Format { get; private set; }
    }
}
