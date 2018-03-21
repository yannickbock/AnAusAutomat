using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace GUI.Internals
{
    internal class Translation
    {
        private CultureInfo _culture;

        internal Translation()
        {
            _culture = Thread.CurrentThread.CurrentCulture;
        }

        internal Translation(CultureInfo culture)
        {
            _culture = culture;
        }

        internal string GetOn()
        {
            return _culture.Name == "de-DE" ? "An" : "On";
        }

        internal string GetOff()
        {
            return _culture.Name == "de-DE" ? "Aus" : "Off";
        }

        internal string GetUndefined()
        {
            return _culture.Name == "de-DE" ? "Undefiniert" : "Undefined";
        }

        internal string GetCountDown(TimeSpan timeSpan)
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

        internal string GetSocket()
        {
            return _culture.Name == "de-DE" ? "Steckdose" : "Socket";
        }

        internal string GetSocketWithID(Socket socket)
        {
            return string.Format(_culture.Name == "de-DE" ? "Steckdose {0}" : "Socket {0}", socket.ID);
        }

        internal string GetSocketWithIDAndName(Socket socket)
        {
            return string.Format(_culture.Name == "de-DE" ? "Steckdose {0}: {1}" : "Socket {0}: {1}", socket.ID, socket.Name);
        }

        internal string GetSocketNameAndStatus(Socket socket, PowerStatus status)
        {
            return string.Format("{0} [{1}]", socket.Name, (status == PowerStatus.On ? GetOn() : GetOff()).ToLower());
        }

        internal string GetMoreOptions()
        {
            return _culture.Name == "de-DE" ? "Weitere Optionen ..." : "More options ...";
        }

        internal string GetBallonTipTitleOnInCountDown(Socket socket, TimeSpan countDown)
        {
            return string.Format(_culture.Name == "de-DE" ?
                "Steckdose {0}: {1} wird in {2} eingeschaltet." :
                "Socket {0}: {1} will turned on in {2}.",
                socket.ID, socket.Name, GetCountDown(countDown));
        }

        internal string GetBallonTipTitleOffInCountDown(Socket socket, TimeSpan countDown)
        {
            return string.Format(_culture.Name == "de-DE" ?
                "Steckdose {0}: {1} wird in {2} ausgeschaltet." :
                "Socket {0}: {1} will turned off in {2}.",
                socket.ID, socket.Name, GetCountDown(countDown));
        }

        internal string GetBallonTipTitleOn(Socket socket)
        {
            return string.Format(_culture.Name == "de-DE" ?
                "Steckdose {0}: {1} eingeschaltet." :
                "Socket {0}: {1} is now on.",
                socket.ID, socket.Name);
        }

        internal string GetBallonTipTitleOff(Socket socket)
        {
            return string.Format(_culture.Name == "de-DE" ?
                "Steckdose {0}: {1} ausgeschaltet." :
                "Socket {0}: {1} is now off.",
                socket.ID, socket.Name);
        }

        internal string GetBalloonTipText(DateTime timeStamp, ISensorMetadata sensor, string condition)
        {
            return string.Format(_culture.Name == "de-DE" ?
                "Uhrzeit: {0}\nTrigger: {1}\nBedingung: {2}" :
                "TimeStamp: {0}\nTriggeredBy: {1}\nCondition: {2}",
                timeStamp, sensor.Name, condition);
        }

        internal string GetExit()
        {
            return _culture.Name == "de-DE" ? "Beenden ..." : "Exit ...";
        }

        internal string GetMessageBoxExitText()
        {
            return _culture.Name == "de-DE" ?
                "Steckdosen ausschalten und AnAusAutomat beenden?" :
                "Power off sockets and exit AnAusAutomat?";
        }
    }
}
