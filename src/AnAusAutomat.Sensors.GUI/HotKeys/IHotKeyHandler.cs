using System;

namespace AnAusAutomat.Sensors.GUI.HotKeys
{
    public interface IHotKeyHandler
    {
        event EventHandler<HotKeyPressedEventArgs> HotKeyPressed;

        void Start();
        void Stop();
    }
}