using System;
using AnAusAutomat.Contracts;

namespace AnAusAutomat.Sensors.GUI
{
    public interface ITranslation
    {
        string GetBallonTipTitleForecast(Socket socket, string triggeredBy);
        string GetBallonTipTitleOff(Socket socket);
        string GetBallonTipTitleOn(Socket socket);
        string GetBalloonTipText(DateTime timeStamp, string triggeredBy, string condition);
        string GetBalloonTipTextForecast(string sensorName, TimeSpan countdown, PowerStatus status);
        string GetCountdown(TimeSpan timeSpan);
        string GetExit();
        string GetMessageBoxExitText();
        string GetMoreOptions();
        string GetOff();
        string GetOn();
        string GetSocket();
        string GetSocketNameAndStatus(Socket socket, PowerStatus status);
        string GetSocketWithID(Socket socket);
        string GetSocketWithIDAndName(Socket socket);
        string GetUndefined();
    }
}