using AnAusAutomat.Contracts;
using System.Collections.Generic;

namespace AnAusAutomat.Sensors.GUI.HotKeys
{
    public class HotKeySettings
    {
        public HotKey PowerOn { get; set; }

        public HotKey PowerOff { get; set; }

        public HotKey Undefined { get; set; }

        public Dictionary<Socket, HotKey> Sockets { get; set; }
    }
}
