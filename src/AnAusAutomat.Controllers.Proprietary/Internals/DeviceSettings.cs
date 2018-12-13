using System.Collections.Generic;

namespace AnAusAutomat.Controllers.Proprietary.Internals
{
    public class DeviceSettings
    {
        public DeviceSettings(string name, Dictionary<int, int> mapping)
        {
            Name = name;
            Mapping = mapping;
        }

        public string Name { get; private set; }

        /// <summary>
        /// Key = AnAusAutomat.Contracts.Socket.ID, Value = ID@Device
        /// </summary>
        public Dictionary<int, int> Mapping { get; private set; }
    }
}
