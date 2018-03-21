using System.ComponentModel;

namespace AnAusAutomat.Contracts.Sensor.Metadata
{
    public interface ISensorMetadata
    {
        string Name { get; }

        [DefaultValue("")]
        string Description { get; }

        /// <summary>
        /// Name, Description
        /// </summary>
        string Parameters { get; }
    }
}
