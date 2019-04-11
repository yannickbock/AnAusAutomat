using System;
using AnAusAutomat.Contracts;

namespace AnAusAutomat.Sensors.GUI.TrayIcon
{
    public interface ITrayIconMenu
    {
        event EventHandler<ExitOnClickEventArgs> ExitOnClick;
        event EventHandler<ModeOnClickEventArgs> ModeOnClick;
        event EventHandler<MoreOptionsOnClickEventArgs> MoreOptionsOnClick;
        event EventHandler<StatusOnClickEventArgs> StatusOnClick;

        void Hide();
        void SetModeState(ConditionMode mode);
        void SetPhysicalStatus(Socket socket, PowerStatus status);
        void SetSensorStatus(Socket socket, PowerStatus status);
        void Show();
        void ShowPhysicalStatusBalloonTip(Socket socket, PowerStatus status, DateTime timeStamp, string triggeredBy, string condition);
        void ShowStatusForecastBalloonTip(Socket socket, PowerStatus status, TimeSpan countdown, string triggeredBy);
    }
}