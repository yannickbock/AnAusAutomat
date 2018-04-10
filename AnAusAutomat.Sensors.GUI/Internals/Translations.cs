using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace AnAusAutomat.Sensors.GUI.Internals
{
    public class Translation
    {
        private CultureInfo _culture;

        public Translation()
        {
            _culture = Thread.CurrentThread.CurrentCulture;
        }

        public Translation(CultureInfo culture)
        {
            _culture = culture;
        }

        public string GetOn()
        {
            return _culture.Name == "de-DE" ? "An" : "On";
        }

        public string GetOff()
        {
            return _culture.Name == "de-DE" ? "Aus" : "Off";
        }

        public string GetUndefined()
        {
            return _culture.Name == "de-DE" ? "Undefiniert" : "Undefined";
        }

        public string GetCountDown(TimeSpan timeSpan)
        {
            var temp = new[]
            {
                new { Name = "Day", Value = timeSpan.Days },
                new { Name = "Hour", Value = timeSpan.Hours },
                new { Name = "Minute", Value = timeSpan.Minutes },
                new { Name = "Second", Value = timeSpan.Seconds }
            }.SkipWhile(x => x.Value <= 0).Select(x =>
            {
                string name = _culture.Name == "de-DE" ? new Dictionary<string, string>()
                {
                    { "Day", "Tag" },
                    { "Hour", "Stunde" },
                    { "Minute", "Minute" },
                    { "Second", "Sekunde" }
                }[x.Name] + (x.Value != 1 ? "n" : "") : x.Name + (x.Value != 1 ? "n" : "");
                return string.Format("{0} {1} ", x.Value, name);
            }).Aggregate((a, b) => a + "§" + b).TrimEnd();

            int index = temp.LastIndexOf('§');
            temp = (index != -1 ? temp.Remove(index, 1).Insert(index, _culture.Name == "de-DE" ? "und " : "and ") : temp).Replace("§", "");

            return temp;
        }

        public string GetSocket()
        {
            return _culture.Name == "de-DE" ? "Steckdose" : "Socket";
        }

        public string GetSocketWithID(Socket socket)
        {
            return string.Format(_culture.Name == "de-DE" ? "Steckdose {0}" : "Socket {0}", socket.ID);
        }

        public string GetSocketWithIDAndName(Socket socket)
        {
            return string.Format(_culture.Name == "de-DE" ? "Steckdose {0}: {1}" : "Socket {0}: {1}", socket.ID, socket.Name);
        }

        public string GetSocketNameAndStatus(Socket socket, PowerStatus status)
        {
            return string.Format("{0} [{1}]", socket.Name, (status == PowerStatus.On ? GetOn() : GetOff()).ToLower());
        }

        public string GetMoreOptions()
        {
            return _culture.Name == "de-DE" ? "Weitere Optionen ..." : "More options ...";
        }

        public string GetBallonTipTitleOnInCountDown(Socket socket, TimeSpan countDown)
        {
            return string.Format(_culture.Name == "de-DE" ?
                "Steckdose {0}: {1} wird in {2} eingeschaltet." :
                "Socket {0}: {1} will turned on in {2}.",
                socket.ID, socket.Name, GetCountDown(countDown));
        }

        public string GetBallonTipTitleOffInCountDown(Socket socket, TimeSpan countDown)
        {
            return string.Format(_culture.Name == "de-DE" ?
                "Steckdose {0}: {1} wird in {2} ausgeschaltet." :
                "Socket {0}: {1} will turned off in {2}.",
                socket.ID, socket.Name, GetCountDown(countDown));
        }

        public string GetBallonTipTitleOn(Socket socket)
        {
            return string.Format(_culture.Name == "de-DE" ?
                "Steckdose {0}: {1} eingeschaltet." :
                "Socket {0}: {1} is now on.",
                socket.ID, socket.Name);
        }

        public string GetBallonTipTitleOff(Socket socket)
        {
            return string.Format(_culture.Name == "de-DE" ?
                "Steckdose {0}: {1} ausgeschaltet." :
                "Socket {0}: {1} is now off.",
                socket.ID, socket.Name);
        }

        public string GetBalloonTipText(DateTime timeStamp, string triggeredBy, string condition)
        {
            return string.Format(_culture.Name == "de-DE" ?
                "Uhrzeit: {0}\nTrigger: {1}\nBedingung: {2}" :
                "TimeStamp: {0}\nTriggeredBy: {1}\nCondition: {2}",
                timeStamp, triggeredBy, condition);
        }

        public string GetExit()
        {
            return _culture.Name == "de-DE" ? "Beenden ..." : "Exit ...";
        }

        public string GetMessageBoxExitText()
        {
            return _culture.Name == "de-DE" ?
                "Steckdosen ausschalten und AnAusAutomat beenden?" :
                "Power off sockets and exit AnAusAutomat?";
        }
    }
}
