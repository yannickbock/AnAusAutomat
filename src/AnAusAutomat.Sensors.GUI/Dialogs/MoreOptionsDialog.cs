using AnAusAutomat.Contracts;
using System;
using System.Drawing;
using System.Windows.Forms;
using SensorPowerStatus = AnAusAutomat.Contracts.PowerStatus;

namespace AnAusAutomat.Sensors.GUI.Dialogs
{
    public class MoreOptionsDialog
    {
        private readonly Font regularFont = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        private readonly Font boldFont = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);

        private ITranslation _translation;
        private Form _form;
        private Button _btnOn;
        private Button _btnOff;
        private Button _btnUndefined;
        private Button _btnOK;
        private ComboBox _cbTimeSpan;

        private bool _canceled;
        private SensorPowerStatus _status;
        private TimeSpan _timeSpan;

        public MoreOptionsDialog(ITranslation translation)
        {
            _translation = translation;
        }

        internal MoreOptionsDialogResult ShowDialog(Socket socket)
        {
            string title = _translation.GetSocketWithIDAndName(socket);
            _form = createForm(title);

            _btnOn = createOnButton();
            _btnOff = createOffButton();
            _btnUndefined = createUndefinedButton();
            _cbTimeSpan = createComboBox();
            _btnOK = createOKButton();

            _form.Controls.Add(_btnOn);
            _form.Controls.Add(_btnOff);
            _form.Controls.Add(_btnUndefined);
            _form.Controls.Add(_cbTimeSpan);
            _form.Controls.Add(_btnOK);

            _btnOn.Click += _btnStatus_Click;
            _btnOff.Click += _btnStatus_Click;
            _btnUndefined.Click += _btnStatus_Click;
            _cbTimeSpan.SelectedValueChanged += _cbTimeSpan_SelectedValueChanged;
            _btnOK.Click += _btnOK_Click;

            _canceled = true;
            _status = SensorPowerStatus.On;
            _timeSpan = new TimeSpan();

            _form.ShowDialog();

            _btnOn.Click -= _btnStatus_Click;
            _btnOff.Click -= _btnStatus_Click;
            _btnUndefined.Click -= _btnStatus_Click;
            _cbTimeSpan.SelectedValueChanged -= _cbTimeSpan_SelectedValueChanged;
            _btnOK.Click -= _btnOK_Click;

            _form = null;
            _btnOn = null;
            _btnOff = null;
            _btnUndefined = null;
            _cbTimeSpan = null;
            _btnOK = null;

            return new MoreOptionsDialogResult(socket, _status, _timeSpan, _canceled);
        }

        private void _cbTimeSpan_SelectedValueChanged(object sender, EventArgs e)
        {
            var item = ((ComboBox)sender).SelectedItem as ComboBoxScheduleItem;
            _timeSpan = item.CountDown;
        }

        private void _btnStatus_Click(object sender, EventArgs e)
        {
            _btnOn.Font = regularFont;
            _btnOff.Font = regularFont;
            _btnUndefined.Font = regularFont;

            var button = (Button)sender;
            button.Font = boldFont;
            _status = (SensorPowerStatus)button.Tag;
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            _canceled = false;

            _form.Close();
            _form.Dispose();
        }

        private Form createForm(string title)
        {
            return new Form()
            {
                Text = title,
                AutoScaleMode = AutoScaleMode.Font,
                ClientSize = new Size(334, 128),
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false,
                ShowIcon = false,
                StartPosition = FormStartPosition.CenterScreen,
                Font = regularFont
            };
        }

        private Button createOnButton()
        {
            return createStatusButton(
                text: _translation.GetOn(),
                status: SensorPowerStatus.On,
                location: new Point(12, 12),
                bold: true);
        }

        private Button createOffButton()
        {
            return createStatusButton(
                text: _translation.GetOff(),
                status: SensorPowerStatus.Off,
                location: new Point(119, 12),
                bold: false);
        }

        private Button createUndefinedButton()
        {
            return createStatusButton(
                text: _translation.GetUndefined(),
                status: SensorPowerStatus.Undefined,
                location: new Point(226, 12),
                bold: false);
        }

        private Button createStatusButton(string text, SensorPowerStatus status, Point location, bool bold)
        {
            return new Button()
            {
                Text = text,
                FlatStyle = FlatStyle.Popup,
                Size = new Size(95, 23),
                UseVisualStyleBackColor = false,
                Location = location,
                Tag = status,
                Font = bold ? boldFont : regularFont
            };
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

        private Button createOKButton()
        {
            return new Button()
            {
                Text = "OK",
                FlatStyle = FlatStyle.Popup,
                Size = new Size(310, 23),
                UseVisualStyleBackColor = true,
                Location = new Point(12, 93),
                Font = regularFont
            };
        }
    }
}
