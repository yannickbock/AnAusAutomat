﻿using System;
using System.Windows.Forms;

namespace AnAusAutomat.Sensors.GUI.HotKeys
{
    public class HotKeyEventArgs : EventArgs
    {
        public Keys Key { get; private set; }
        public KeyModifiers Modifiers { get; private set; }

        public HotKeyEventArgs(Keys key, KeyModifiers modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public HotKeyEventArgs(IntPtr hotKeyParam)
        {
            uint param = (uint)hotKeyParam.ToInt64();
            Key = (Keys)((param & 0xffff0000) >> 16);
            Modifiers = (KeyModifiers)(param & 0x0000ffff);
        }
    }
}
