using System;
using System.Windows.Forms;

namespace AnAusAutomat.Sensors.GUI.HotKeys
{
    public interface IHotKeyNotifier
    {
        event EventHandler<HotKeyEventArgs> HotKeyPressed;

        int RegisterHotKey(KeyModifiers modifiers, Keys key);
        void UnregisterHotKey(int id);
    }
}