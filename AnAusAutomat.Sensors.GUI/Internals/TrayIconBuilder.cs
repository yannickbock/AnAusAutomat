using AnAusAutomat.Contracts;
using AnAusAutomat.Sensors.GUI.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SensorPowerStatus = AnAusAutomat.Contracts.PowerStatus;

namespace AnAusAutomat.Sensors.GUI.Internals
{
    public class TrayIconBuilder
    {
        private Translation _translation;
        private List<ToolStripItem> _notifyIconItems = new List<ToolStripItem>();

        public TrayIconBuilder(Translation translation)
        {
            _translation = translation;
        }

        private NotifyIcon createNotifyIcon()
        {
            return new NotifyIcon()
            {
                Icon = Resources.Icon,
                Text = "AnAusAutomat",
                Visible = false,
                ContextMenuStrip = new ContextMenuStrip()
                {
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    ImageScalingSize = new Size(32, 32),
                    Text = "AnAusAutomat",
                    IsAccessible = true
                }
            };
        }

        public void AddExitStrip()
        {
            string text = _translation.GetExit();
            _notifyIconItems.Add(new ToolStripMenuItem(text, null, null, "exit"));
        }

        public void AddSocketStrip(Socket socket)
        {
            var menuItem = new ToolStripMenuItem()
            {
                Text = _translation.GetSocketNameAndStatus(socket, SensorPowerStatus.Off),
                Image = Resources.Off,
                ImageScaling = ToolStripItemImageScaling.None,
                Tag = socket
            };

            var statusItems = createStatusToolStrips();
            var moreOptionsItem = new ToolStripMenuItem(_translation.GetMoreOptions(), null, null, "moreOptions");

            var dropDownItems = new List<ToolStripItem>();
            dropDownItems.Add(new ToolStripLabel(_translation.GetSocketWithID(socket)));
            dropDownItems.Add(new ToolStripSeparator());
            dropDownItems.AddRange(statusItems);
            dropDownItems.Add(new ToolStripSeparator());
            dropDownItems.Add(moreOptionsItem);
            menuItem.DropDownItems.AddRange(dropDownItems.ToArray());

            _notifyIconItems.Add(menuItem);
        }

        private ToolStripItem[] createStatusToolStrips()
        {
            var onItem = createStatusToolStrip(_translation.GetOn(), "on", SensorPowerStatus.On, false);
            var offItem = createStatusToolStrip(_translation.GetOff(), "off", SensorPowerStatus.Off, false);
            var undefinedItem = createStatusToolStrip(_translation.GetUndefined(), "undefined", SensorPowerStatus.Undefined, true);

            onItem.Click += (sender, args) =>
            {
                onItem.Checked = true;
                offItem.Checked = false;
                undefinedItem.Checked = false;
            };

            offItem.Click += (sender, args) =>
            {
                onItem.Checked = false;
                offItem.Checked = true;
                undefinedItem.Checked = false;
            };

            undefinedItem.Click += (sender, args) =>
            {
                onItem.Checked = false;
                offItem.Checked = false;
                undefinedItem.Checked = true;
            };

            return new ToolStripItem[] { onItem, offItem, undefinedItem };
        }

        private ToolStripMenuItem createStatusToolStrip(string text, string name, SensorPowerStatus status, bool isChecked)
        {
            return new ToolStripMenuItem(text, null, null, name)
            {
                Checked = isChecked,
                ImageScaling = ToolStripItemImageScaling.None,
                Tag = status
            };
        }

        public void AddModeStrip(IEnumerable<ConditionMode> modes)
        {
            if (modes.Any())
            {
                var dropDownItems = modes.Select(x =>
                {
                    return new ToolStripMenuItem(x.Name, null, onClick: (sender, e) =>
                    {
                        var item = ((ToolStripMenuItem)sender);
                        item.Checked = !item.Checked;
                    })
                    {
                        ImageScaling = ToolStripItemImageScaling.None,
                        Checked = x.IsActive,
                        Tag = x
                    };
                }).ToList();

                var modesItem = new ToolStripMenuItem("Mode", null, null, "mode");
                modesItem.DropDownItems.AddRange(dropDownItems.ToArray());

                _notifyIconItems.Add(modesItem);
            }
        }

        public void AddSeparatorStrip()
        {
            _notifyIconItems.Add(new ToolStripSeparator());
        }

        public TrayIcon Build()
        {
            var notifyIcon = createNotifyIcon();
            notifyIcon.ContextMenuStrip.Items.AddRange(_notifyIconItems.ToArray());

            return new TrayIcon(notifyIcon, _translation);
        }
    }
}
