using AnAusAutomat.Contracts;
using GUI.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SensorPowerStatus = AnAusAutomat.Contracts.Sensor.PowerStatus;

namespace GUI.Internals
{
    internal class TrayIconFactory
    {
        private Translation _translation;

        internal TrayIconFactory()
        {
            _translation = new Translation();
        }

        internal NotifyIcon CreateNotifyIcon()
        {
            var notifyIcon = new NotifyIcon()
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

            return notifyIcon;
        }

        internal ToolStripMenuItem CreateModeToolStrip(IEnumerable<string> modes, string currentMode)
        {
            var dropDownItems = modes.Select(x =>
            {
                return new ToolStripMenuItem(x, null, onClick: (sender, e) =>
                {
                    var m = ((ToolStripMenuItem)sender);
                    var items = m.Owner.Items.Cast<ToolStripMenuItem>().Where(a => a.Checked);
                    foreach (var item in items)
                    {
                        item.Checked = false;
                    }
                    m.Checked = true;
                })
                {
                    ImageScaling = ToolStripItemImageScaling.None,
                    Checked = x == currentMode ? true : false
                };
            }).ToList();

            var modesItem = new ToolStripMenuItem("Mode");
            modesItem.DropDownItems.AddRange(dropDownItems.ToArray());

            return modesItem;
        }

        internal ToolStripMenuItem CreateExitToolStrip()
        {
            return new ToolStripMenuItem(_translation.GetExit());
        }

        internal ToolStripMenuItem CreateSocketToolStrip(Socket socket)
        {
            var menuItem = new ToolStripMenuItem()
            {
                Text = _translation.GetSocketNameAndStatus(socket, SensorPowerStatus.Off),
                Image = Resources.On,
                Name = string.Format("ID{0}", socket.ID.ToString()),
                ImageScaling = ToolStripItemImageScaling.None
            };

            var statusItems = createStatusToolStrips();
            var moreOptionsItem = createMoreOptionsToolStrip();

            var dropDownItems = new List<ToolStripItem>();
            dropDownItems.Add(new ToolStripLabel(_translation.GetSocketWithID(socket)));
            dropDownItems.Add(new ToolStripSeparator());
            dropDownItems.AddRange(statusItems);
            dropDownItems.Add(new ToolStripSeparator());
            dropDownItems.Add(moreOptionsItem);

            menuItem.DropDownItems.AddRange(dropDownItems.ToArray());

            return menuItem;
        }

        private ToolStripItem[] createStatusToolStrips()
        {
            var onItem = createStatusToolStrip(_translation.GetOn(), false);
            var offItem = createStatusToolStrip(_translation.GetOff(), false);
            var undefinedItem = createStatusToolStrip(_translation.GetUndefined(), true);

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

        private ToolStripMenuItem createStatusToolStrip(string text, bool isChecked)
        {
            return new ToolStripMenuItem(text)
            {
                Checked = isChecked,
                ImageScaling = ToolStripItemImageScaling.None
            };
        }

        private ToolStripItem createMoreOptionsToolStrip()
        {
            return new ToolStripMenuItem(_translation.GetMoreOptions());
        }
    }
}
