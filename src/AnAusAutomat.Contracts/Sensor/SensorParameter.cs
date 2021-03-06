﻿namespace AnAusAutomat.Contracts.Sensor
{
    public class SensorParameter
    {
        public SensorParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        public string Value { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Name, Value);
        }
    }
}
