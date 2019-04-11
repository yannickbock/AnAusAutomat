using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Sensors.GUI.HotKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AnAusAutomat.Sensors.GUI
{
    public class SettingsParser
    {
        private List<SensorParameter> _parameters;
        private Dictionary<Socket, IEnumerable<SensorParameter>> _sockets;

        public SettingsParser(List<SensorParameter> parameters, Dictionary<Socket, IEnumerable<SensorParameter>> sockets)
        {
            _parameters = parameters;
            _sockets = sockets;
        }

        public Settings Parse()
        {
            bool startMinimized = true;
            HotKey powerOnHotKey = null;
            HotKey powerOffHotKey = null;
            HotKey undefinedHotKey = null;
            var socketHotKeys = new Dictionary<Socket, HotKey>();

            if (_parameters.Any())
            {
                bool startMinimizedDefined = _parameters.Any(x => x.Name == "StartMinimized");
                bool powerOnHotKeyDefined = _parameters.Any(x => x.Name == "PowerOnHotKey");
                bool powerOffHotKeyDefined = _parameters.Any(x => x.Name == "PowerOffHotKey");
                bool undefinedHotKeyDefined = _parameters.Any(x => x.Name == "UndefinedHotKey");

                if (startMinimizedDefined)
                {
                    string startMinimizedAsString = getParameterValue("StartMinimized");
                    startMinimized = new string[] { "true", "yes", "1" }.Contains(startMinimizedAsString);
                }
                if (powerOnHotKeyDefined)
                {
                    string powerOnHotKeyAsString = getParameterValue("PowerOnHotKey");
                    powerOnHotKey = parseHotKey(powerOnHotKeyAsString);
                }
                if (powerOffHotKeyDefined)
                {
                    string powerOffHotKeyAsString = getParameterValue("PowerOffHotKey");
                    powerOffHotKey = parseHotKey(powerOffHotKeyAsString);
                }
                if (undefinedHotKeyDefined)
                {
                    string undefinedHotKeyAsString = getParameterValue("UndefinedHotKey");
                    undefinedHotKey = parseHotKey(undefinedHotKeyAsString);
                }

                socketHotKeys = _sockets.ToDictionary(
                    x => x.Key,
                    y => parseHotKey(getParameterValue(y.Key, "HotKey")));
            }

            return new Settings()
            {
                StartMinimized = startMinimized,
                HotKeys = new HotKeySettings()
                {
                    PowerOn = powerOnHotKey,
                    PowerOff = powerOffHotKey,
                    Undefined = undefinedHotKey,
                    Sockets = socketHotKeys
                }
            };
        }

        private HotKey parseHotKey(string text)
        {
            var split = text.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 2)
            {
                var keyModifier = parseKeyModifier(split.First());
                var key = parseKey(split.Last());

                return new HotKey(keyModifier, key);
            }

            return null;
        }

        private KeyModifiers parseKeyModifier(string text)
        {
            string keyModifierText = text.Trim().ToUpper();
            keyModifierText = keyModifierText.Replace("CTRL", "CONTROL");
            keyModifierText = keyModifierText.Equals("WINDOWS") ? "" : keyModifierText.Replace("WIN", "WINDOWS");

            var keyModifier = (KeyModifiers)Enum.Parse(typeof(KeyModifiers), keyModifierText, true);
            return keyModifier;
        }

        private Keys parseKey(string text)
        {
            string keyText = text.Trim().ToUpper();
            keyText = keyText.Replace("ESC", "ESCAPE");

            bool keyIsNumber = keyText.Length == 1 && char.IsNumber(keyText.First());
            if (keyIsNumber)
            {
                keyText = "D" + keyText;
            }

            var key = (Keys)Enum.Parse(typeof(Keys), keyText, true);
            return key;
        }

        private string getParameterValue(string name)
        {
            var parameter = _parameters.FirstOrDefault(x => x.Name.Equals(name));

            return parameter == null ? string.Empty : parameter.Value;
        }

        private string getParameterValue(Socket socket, string name)
        {
            if (_sockets.Any(x => x.Key.Equals(socket)))
            {
                var socketParameters = _sockets.FirstOrDefault(x => x.Key.Equals(socket)).Value;
                var parameter = socketParameters.FirstOrDefault(x => x.Name.Equals(name));

                return parameter == null ? string.Empty : parameter.Value;
            }

            return string.Empty;
        }
    }
}
