﻿using AnAusAutomat.Contracts;
using AnAusAutomat.Sensors.GUI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SensorPowerStatus = AnAusAutomat.Contracts.PowerStatus;

namespace AnAusAutomat.Sensors.GUI.TrayIcon
{
    public class TrayIconMenu : ITrayIconMenu
    {
        private NotifyIcon _notifyIcon;
        private Translation _translation;

        public event EventHandler<ModeOnClickEventArgs> ModeOnClick;

        public event EventHandler<StatusOnClickEventArgs> StatusOnClick;

        public event EventHandler<MoreOptionsOnClickEventArgs> MoreOptionsOnClick;

        public event EventHandler<ExitOnClickEventArgs> ExitOnClick;

        public TrayIconMenu(NotifyIcon notifyIcon, Translation translation)
        {
            _notifyIcon = notifyIcon;
            _translation = translation;
            assignEvents();
        }

        private void assignEvents()
        {
            var modeItems = getModeItems();
            if (modeItems?.Any() == true)
            {
                foreach (var modeItem in modeItems)
                {
                    modeItem.Click += (sender, e) =>
                    {
                        var item = sender as ToolStripMenuItem;
                        var mode = (ConditionMode)item?.Tag;
                        ModeOnClick?.Invoke(this, new ModeOnClickEventArgs(mode));
                    };
                }
            }

            var exitItem = getExitItem();
            if (exitItem != null)
            {
                exitItem.Click += (sender, e) =>
                {
                    var item = sender as ToolStripMenuItem;
                    ExitOnClick?.Invoke(this, new ExitOnClickEventArgs());
                };
            }

            var socketItems = getSocketItems();
            if (socketItems?.Any() == true)
            {
                foreach (var socketItem in socketItems)
                {
                    var socket = (Socket)socketItem.Tag;

                    foreach (ToolStripItem socketChildItem in socketItem.DropDownItems)
                    {
                        if (socketChildItem.Name == "moreOptions")
                        {
                            socketChildItem.Click += (sender, e) =>
                            {
                                MoreOptionsOnClick?.Invoke(this, new MoreOptionsOnClickEventArgs(socket));
                            };
                        }
                        else if (new string[] { "on", "off", "undefined" }.Contains(socketChildItem.Name))
                        {
                            socketChildItem.Click += (sender, e) =>
                            {
                                var status = Enum.Parse(typeof(SensorPowerStatus), socketChildItem.Tag.ToString());
                                StatusOnClick?.Invoke(this, new StatusOnClickEventArgs(socket, (SensorPowerStatus)socketChildItem.Tag));
                            };
                        }
                    }
                }
            }
        }

        public void SetModeState(ConditionMode mode)
        {
            invokeIfRequired(new Action(() =>
            {
                var items = getItems();
                var modeItems = getModeItems();

                if (modeItems != null)
                {
                    var item = items.FirstOrDefault(x => x.Text == mode.Name);
                    item.Checked = mode.IsActive;
                }
            }));
        }

        public void SetSensorStatus(Socket socket, SensorPowerStatus status)
        {
            // ...
        }

        public void SetPhysicalStatus(Socket socket, SensorPowerStatus status)
        {
            invokeIfRequired(new Action(() =>
            {
                var item = getSocketItem(socket);

                switch (status)
                {
                    case SensorPowerStatus.On:
                        item.Image = Resources.On;
                        item.Text = _translation.GetSocketNameAndStatus(socket, status);
                        break;
                    case SensorPowerStatus.Off:
                        item.Image = Resources.Off;
                        item.Text = _translation.GetSocketNameAndStatus(socket, status);
                        break;
                }
            }));
        }

        public void ShowPhysicalStatusBalloonTip(Socket socket, SensorPowerStatus status, DateTime timeStamp, string triggeredBy, string condition)
        {
            invokeIfRequired(new Action(() =>
            {
                string title = string.Empty;
                string text = _translation.GetBalloonTipText(timeStamp, triggeredBy, condition);

                if (status == SensorPowerStatus.On)
                {
                    title = _translation.GetBallonTipTitleOn(socket);
                }
                else
                {
                    title = _translation.GetBallonTipTitleOff(socket);
                }

                _notifyIcon.ShowBalloonTip(1000, title.PadRight(title.Length + 10), text, ToolTipIcon.Info);
            }));            
        }

        public void ShowStatusForecastBalloonTip(Socket socket, SensorPowerStatus status, TimeSpan countdown, string triggeredBy)
        {
            invokeIfRequired(new Action(() =>
            {
                string title = _translation.GetBallonTipTitleForecast(socket, triggeredBy);
                string text = _translation.GetBalloonTipTextForecast(triggeredBy, countdown, status);

                _notifyIcon.ShowBalloonTip(1000, title.PadRight(title.Length + 10), text, ToolTipIcon.Info);
            }));
        }

        public void Show()
        {
            _notifyIcon.Visible = true;
            _notifyIcon.Icon = Resources.Icon;
        }

        public void Hide()
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Icon = null;
        }

        private ToolStripMenuItem getSocketItem(Socket socket)
        {
            var items = getSocketItems();
            return items?.FirstOrDefault(x => x.Tag.Equals(socket));
        }

        private IEnumerable<ToolStripMenuItem> getSocketItems()
        {
            var items = getItems();
            return items?.Where(x => x.Tag as Socket != null).Cast<ToolStripMenuItem>();
        }

        private IEnumerable<ToolStripMenuItem> getModeItems()
        {
            var items = getItems();
            return items?.FirstOrDefault(x => x.Name == "mode")?.DropDownItems?.Cast<ToolStripMenuItem>();
        }

        private ToolStripMenuItem getExitItem()
        {
            var items = getItems();
            return items?.FirstOrDefault(x => x.Name == "exit");
        }

        private IEnumerable<ToolStripMenuItem> getItems()
        {
            return _notifyIcon.ContextMenuStrip.Items.Cast<ToolStripItem>().Where(x => x as ToolStripMenuItem != null).Cast<ToolStripMenuItem>();
        }

        private void invokeIfRequired(Action action)
        {
            if (_notifyIcon.ContextMenuStrip.InvokeRequired)
            {
                _notifyIcon.ContextMenuStrip.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
