using System;
using System.Runtime.InteropServices;

namespace AnAusAutomat.Sensors.Keylogger.Internals
{
    public static class User32Adapter
    {
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        public static DateTime LastInput
        {
            get
            {
                return DateTime.Now - TimeSpan.FromSeconds(getLastGlobalInputTime());
            }
        }

        public static uint InputIdleSeconds
        {
            get
            {
                return getLastGlobalInputTime();
            }
        }

        private static uint getLastGlobalInputTime()
        {
            uint idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            uint envTicks = (uint)Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint lastInputTick = lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            return (idleTime > 0) ? (idleTime / 1000) : 0;
        }
    }
}
