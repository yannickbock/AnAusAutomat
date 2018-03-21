using System;

namespace AnAusAutomat.Contracts.Sensor.Metadata
{
    public class SensorMetadata : ISensorMetadata
    {
        public SensorMetadata(string name, string description, string parameters)
        {
            Name = name;
            Description = description;
            Parameters = parameters;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string Parameters { get; private set; }

        public bool Equals(ISensorMetadata other)
        {
            return Name == other.Name;
        }
    }
}
