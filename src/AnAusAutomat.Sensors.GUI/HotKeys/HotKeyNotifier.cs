using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AnAusAutomat.Sensors.GUI.HotKeys
{
    public class HotKeyNotifier : IHotKeyNotifier
    {
        private static int _id = 0;
        private static volatile MessageWindow _wnd;
        private static volatile IntPtr _hwnd;
        private static ManualResetEvent _windowReadyEvent = new ManualResetEvent(false);

        [DllImport("user32", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public event EventHandler<HotKeyEventArgs> HotKeyPressed;

        public HotKeyNotifier()
        {
            Thread messageLoop = new Thread(delegate ()
            {
                Application.Run(new MessageWindow(this));
            });

            messageLoop.Name = "MessageLoopThread";
            messageLoop.IsBackground = true;
            messageLoop.Start();
        }

        public int RegisterHotKey(KeyModifiers modifiers, Keys key)
        {
            _windowReadyEvent.WaitOne();
            int id = Interlocked.Increment(ref _id);
            _wnd.Invoke(new Action(() => RegisterHotKey(_hwnd, id, (uint)modifiers, (uint)key)));

            return id;
        }

        public void UnregisterHotKey(int id)
        {
            _wnd.Invoke(new Action(() => UnregisterHotKey(_hwnd, id)));
        }

        private void OnHotKeyPressed(HotKeyEventArgs e)
        {
            HotKeyPressed?.Invoke(null, e);
        }

        private class MessageWindow : Form
        {
            private const int WM_HOTKEY = 0x312;
            private HotKeyNotifier _hotKeyNotifier;

            public MessageWindow(HotKeyNotifier hotKeyNotifier)
            {
                _hotKeyNotifier = hotKeyNotifier;
                _wnd = this;
                _hwnd = this.Handle;
                _windowReadyEvent.Set();
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY)
                {
                    _hotKeyNotifier.OnHotKeyPressed(new HotKeyEventArgs(m.LParam));
                }

                base.WndProc(ref m);
            }

            protected override void SetVisibleCore(bool value)
            {
                base.SetVisibleCore(false);
            }
        }
    }
}
