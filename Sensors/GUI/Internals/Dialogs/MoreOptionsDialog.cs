using AnAusAutomat.Contracts;
using GUI.Internals.Scheduling;
using System;
using System.Drawing;
using System.Windows.Forms;
using SensorPowerStatus = AnAusAutomat.Contracts.Sensor.PowerStatus;

namespace GUI.Internals.Dialogs
{
    internal class MoreOptionsDialog
    {
        private Translation _translation;

        private readonly Font regularFont = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        private readonly Font boldFont = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);

        private Form _form;
        private Button _btnOn;
        private Button _btnOff;
        private Button _btnUndefined;
        private ComboBox _cbTime;
        private Button _btnExecute;

        private bool _btnExecutePressed;
        private SensorPowerStatus _status;
        private Socket _socket;

        public MoreOptionsDialog(Socket socket)
        {
            _socket = socket;
            _translation = new Translation();

            _btnExecutePressed = false;
            _status = SensorPowerStatus.On;

            string title = _translation.GetSocketWithIDAndName(socket);
            _form = createForm(title);
            _btnOn = createSocketStateButton(_translation.GetOn(), new Point(12, 12), true);
            _btnOff = createSocketStateButton(_translation.GetOff(), new Point(119, 12), false);
            _btnUndefined = createSocketStateButton(_translation.GetUndefined(), new Point(226, 12), false);
            _cbTime = createComboBox();
            _btnExecute = createExecuteButton();

            _form.Controls.Add(_btnOn);
            _form.Controls.Add(_btnOff);
            _form.Controls.Add(_btnUndefined);
            _form.Controls.Add(_cbTime);
            _form.Controls.Add(_btnExecute);
        }

        internal MoreOptionsDialogResult ShowDialog()
        {
            _form.ShowDialog();

            bool canceled = _btnExecutePressed ? false : true;
            var scheduleItem = (ComboBoxScheduleItem)_cbTime.SelectedItem;

            var task = new ScheduledTask(
                executeAt: DateTime.Now + scheduleItem.CountDown,
                status: _status,
                socket: _socket);

            return new MoreOptionsDialogResult(canceled, task);
        }

        private Form createForm(string text)
        {
            var form = new Form();
            form.Text = text;
            form.AutoScaleMode = AutoScaleMode.Font;
            form.ClientSize = new Size(334, 128);
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
            form.ShowIcon = false;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Font = regularFont;
            return form;
        }

        private Button createSocketStateButton(string text, Point location, bool bold)
        {
            var button = new Button();
            button.FlatStyle = FlatStyle.Popup;
            button.Size = new Size(95, 23);
            button.UseVisualStyleBackColor = false;
            button.Location = location;
            button.Text = text;
            button.Click += _btnSocketState_Click;
            button.Font = bold ? boldFont : regularFont;
            return button;
        }

        private ComboBox createComboBox()
        {
            var comboBox = new ComboBox();
            comboBox.FlatStyle = FlatStyle.Popup;
            comboBox.Size = new Size(310, 21);
            comboBox.FormattingEnabled = true;
            comboBox.Location = new Point(12, 41);
            comboBox.Font = regularFont;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Items.AddRange(new object[] {
                new ComboBoxScheduleItem("Now", new TimeSpan(0, 0, 0)),
                new ComboBoxScheduleItem("In 10 seconds", new TimeSpan(0, 0, 10)),
                new ComboBoxScheduleItem("In 30 seconds", new TimeSpan(0, 0, 30)),
                new ComboBoxScheduleItem("In 1 minute", new TimeSpan(0, 1, 0)),
                new ComboBoxScheduleItem("In 5 minutes", new TimeSpan(0, 5, 0)),
                new ComboBoxScheduleItem("In 15 minutes", new TimeSpan(0, 15, 0)),
                new ComboBoxScheduleItem("In 30 minutes", new TimeSpan(0, 30, 0)),
                new ComboBoxScheduleItem("In 1 hour", new TimeSpan(1, 0, 0)),
                new ComboBoxScheduleItem("In 2 hours", new TimeSpan(2, 0, 0)),
                new ComboBoxScheduleItem("In 3 hours", new TimeSpan(3, 0, 0)),
                new ComboBoxScheduleItem("In 4 hours", new TimeSpan(4, 0, 0)),
                new ComboBoxScheduleItem("In 5 hours", new TimeSpan(5, 0, 0)),
                new ComboBoxScheduleItem("In 6 hours", new TimeSpan(6, 0, 0)),
                new ComboBoxScheduleItem("Disabled", new TimeSpan(0, 0, 0)),
            });
            comboBox.SelectedIndex = 0;
            return comboBox;
        }

        private Button createExecuteButton()
        {
            var button = new Button();
            button.FlatStyle = FlatStyle.Popup;
            button.Size = new Size(310, 23);
            button.UseVisualStyleBackColor = true;
            button.Location = new Point(12, 93);
            button.Text = "Execute";
            button.Font = regularFont;
            button.Click += _btnExecute_Click;
            return button;
        }

        private void _btnSocketState_Click(object sender, EventArgs e)
        {
            _btnOn.Font = regularFont;
            _btnOff.Font = regularFont;
            _btnUndefined.Font = regularFont;

            var button = ((Button)sender);
            button.Font = boldFont;

            if (button.Text == _translation.GetOn())
                _status = SensorPowerStatus.On;
            else if (button.Text == _translation.GetOff())
                _status = SensorPowerStatus.Off;
            else if (button.Text == _translation.GetUndefined())
                _status = SensorPowerStatus.Undefined;
        }

        private void _btnExecute_Click(object sender, EventArgs e)
        {
            _btnExecutePressed = true;

            _form.Close();
            _form.Dispose();
        }
    }
}
