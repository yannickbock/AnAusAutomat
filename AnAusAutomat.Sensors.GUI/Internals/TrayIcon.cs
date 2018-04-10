using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Sensors.GUI.Internals.Dialogs;
using AnAusAutomat.Sensors.GUI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SensorPowerStatus = AnAusAutomat.Contracts.Sensor.PowerStatus;

namespace AnAusAutomat.Sensors.GUI.Internals
{
    public class TrayIcon
    {
        private TrayIconFactory _factory;

        private NotifyIcon _notifyIcon;
        private Translation _translation;
        private SensorSettings _settings;
        private List<string> _modes;
        private string _currentMode;

        public event EventHandler<ModeChangedEventArgs> ModeChanged;
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<ApplicationExitEventArgs> ApplicationExit;

        public TrayIcon(SensorSettings settings, IEnumerable<string> modes, string currentMode)
        {
            _settings = settings;
            _modes = modes.ToList();
            _currentMode = currentMode;

            _translation = new Translation();
            _factory = new TrayIconFactory();
        }

        public void OnSensorStatusHasChanged(object sender, StatusChangedEventArgs e)
        {
            // only in winForms mode?
        }

        public void OnPhysicalStatusHasChanged(object sender, StatusChangedEventArgs e)
        {
            var item = getSocketMenuItem(e.Socket);

            switch (e.Status)
            {
                case SensorPowerStatus.On:
                    item.Image = Resources.On;
                    item.Text = _translation.GetSocketNameAndStatus(e.Socket, e.Status);
                    break;
                case SensorPowerStatus.Off:
                    item.Image = Resources.Off;
                    item.Text = _translation.GetSocketNameAndStatus(e.Socket, e.Status);
                    break;
            }

            showBalloonTipPhysicalStatusHasChanged(e, sender.GetType().Name);
        }

        public void OnStatusChangesIn(object sender, StatusChangesInEventArgs e)
        {
            // only in winForms mode?
            
            string title = e.Status == SensorPowerStatus.On ?
                _translation.GetBallonTipTitleOnInCountDown(e.Socket, e.CountDown) : _translation.GetBallonTipTitleOffInCountDown(e.Socket, e.CountDown);
            string text = sender.GetType().Name;

            _notifyIcon.ShowBalloonTip(1000, title, text, ToolTipIcon.Info);
        }

        public void OnModeHasChanged(object sender, ModeChangedEventArgs e)
        {
            var items = getModeMenuItem().DropDownItems.Cast<ToolStripMenuItem>();
            
            foreach (var item in items)
            {
                item.Checked = false;
            }

            items.FirstOrDefault(x => x.Text == e.Mode);
        }

        public void Show()
        {
            if (_notifyIcon == null)
            {
                initializeUI();
            }

            _notifyIcon.Visible = true;
            _notifyIcon.Icon = Resources.Icon;
        }

        public void Hide()
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Icon = null;
        }

        private void initializeUI()
        {
            _notifyIcon = _factory.CreateNotifyIcon();
            
            var socketToolStrips = createSocketToolStrips(_settings.Sockets);
            var modeToolStrip = createModeToolStrip(_modes, _currentMode);
            var exitToolStrip = createExitToolStrip();

            var items = new List<ToolStripItem>();
            items.AddRange(socketToolStrips);
            items.Add(new ToolStripSeparator());
            items.Add(modeToolStrip);
            items.Add(new ToolStripSeparator());
            items.Add(exitToolStrip);
            _notifyIcon.ContextMenuStrip.Items.AddRange(items.ToArray());
        }

        private ToolStripItem createModeToolStrip(IEnumerable<string> modes, string currentMode)
        {
            var modesItem = _factory.CreateModeToolStrip(modes, currentMode);

            foreach (ToolStripMenuItem item in modesItem.DropDownItems)
            {
                item.Click += (sender, e) =>
                {
                    ModeChanged?.Invoke(this, new ModeChangedEventArgs("", ((ToolStripMenuItem)sender).Text));
                };
            }

            return modesItem;
        }

        private ToolStripMenuItem createExitToolStrip()
        {
            var exitItem = _factory.CreateExitToolStrip();
            exitItem.Click += (sender, args) =>
            {
                var dialogResult = MessageBox.Show(_translation.GetMessageBoxExitText(), "AnAusAutomat", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    // not supported exception thrown... strange...
                    ApplicationExit?.Invoke(this, new ApplicationExitEventArgs(""));
                }
            };

            return exitItem;
        }

        private ToolStripMenuItem[] createSocketToolStrips(IEnumerable<Socket> sockets)
        {
            return sockets.Select(x => createSocketToolStrip(x)).ToArray();
        }

        private ToolStripMenuItem createSocketToolStrip(Socket socket)
        {
            var menuItem = _factory.CreateSocketToolStrip(socket);
            menuItem.DropDownItems.Cast<ToolStripItem>().FirstOrDefault(x => x.Text == _translation.GetOn()).Click += (sender, e) =>
            {
                StatusChanged?.Invoke(this, buildStatusChangedEventArgs(sender, SensorPowerStatus.On));
            };
            menuItem.DropDownItems.Cast<ToolStripItem>().FirstOrDefault(x => x.Text == _translation.GetOff()).Click += (sender, e) =>
            {
                StatusChanged?.Invoke(this, buildStatusChangedEventArgs(sender, SensorPowerStatus.Off));
            };
            menuItem.DropDownItems.Cast<ToolStripItem>().FirstOrDefault(x => x.Text == _translation.GetUndefined()).Click += (sender, e) =>
            {
                StatusChanged?.Invoke(this, buildStatusChangedEventArgs(sender, SensorPowerStatus.Undefined));
            };
            menuItem.DropDownItems.Cast<ToolStripItem>().FirstOrDefault(x => x.Text == _translation.GetMoreOptions()).Click += (sender, e) =>
            {
                var dialogResult = new MoreOptionsDialog(socket).ShowDialog();

                if (!dialogResult.Canceled)
                {
                    throw new NotImplementedException();
                    //var s = _settings.FirstOrDefault(x => x.Socket.ID == settings.Socket.ID);
                    //s.DelayExecutionTime = DateTime.Now + result.Delay;
                    //s.DelayExecutionSocketStatus = result.Status;
                    //s.DelayExecutionEnabled = result.DelayExecutionEnabled;
                }
            };

            return menuItem;
        }

        private void showBalloonTipPhysicalStatusHasChanged(StatusChangedEventArgs e, string triggeredBy)
        {
            string title = e.Status == SensorPowerStatus.On ?
                _translation.GetBallonTipTitleOn(e.Socket) : _translation.GetBallonTipTitleOff(e.Socket);
            string text = _translation.GetBalloonTipText(e.TimeStamp, triggeredBy, e.Condition);

            _notifyIcon.ShowBalloonTip(1000, title.PadRight(title.Length + 10), text, ToolTipIcon.Info);
        }

        private StatusChangedEventArgs buildStatusChangedEventArgs(object sender, SensorPowerStatus status)
        {
            var ownerItem = ((ToolStripMenuItem)sender).OwnerItem;
            var socket = getSocket(ownerItem);
            return new StatusChangedEventArgs("", "", socket, status);
        }

        private Socket getSocket(ToolStripItem item)
        {
            int id = int.Parse(item.Name.Replace("ID", ""));
            return _settings.Sockets.FirstOrDefault(x => x.ID == id);
        }

        private ToolStripMenuItem getSocketMenuItem(Socket socket)
        {
            return _notifyIcon.ContextMenuStrip.Items.Cast<ToolStripMenuItem>().FirstOrDefault(x => x.Name == string.Format("ID{0}", socket.ID));
        }

        private ToolStripMenuItem getModeMenuItem()
        {
            return _notifyIcon.ContextMenuStrip.Items.Cast<ToolStripMenuItem>().FirstOrDefault(x => x.Text == "Mode");
        }
    }
}