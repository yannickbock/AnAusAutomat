using System;
using System.ComponentModel.Composition;

namespace AnAusAutomat.Contracts.Sensor.Metadata
{
    [MetadataAttribute]
    public class SensorMetadataAttribute : Attribute, ISensorMetadata, IEquatable<ISensorMetadata>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Parameters { get; set; }

        public override string ToString()
        {
            return string.Format("Sensor [ Name: {0} | Description: {1} ]", Name, Description);
        }

        public bool Equals(ISensorMetadata other)
        {
            return Name == other.Name;
        }
    }
}
