using AnAusAutomat.Toolbox.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Controllers.Serial.Internals
{
    public class ResponseParser
    {
        public bool ParseHello(string text, out string name, out IEnumerable<int> sockets)
        {
            name = string.Empty;
            sockets = new List<int>();

            text = text?.Trim();
            bool isDevice = !string.IsNullOrEmpty(text) && text.StartsWith("AnAusAutomat");
            if (isDevice)
            {
                Logger.Debug(string.Format("{0} seems to be a valid response. Trying to parse.", text));
                var array = text.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Count() == 3)
                {
                    name = array.ElementAt(1);
                    sockets = parseHelloSockets(array.ElementAt(2));

                    if (string.IsNullOrEmpty(name))
                    {
                        Logger.Error(string.Format("No name received: {0}", text));
                    }

                    if (sockets == null)
                    {
                        Logger.Error(string.Format("No sockets received: {0}", text));
                    }

                    if (!string.IsNullOrEmpty(name) && sockets != null)
                    {
                        return true;
                    }
                }
                else
                {
                    Logger.Error(string.Format("The Hello response is not valid: {0}", text));
                }
            }

            return false;
        }

        private IEnumerable<int> parseHelloSockets(string text)
        {
            string temp = text.Replace("{", "").Replace("}", "");
            var array = temp.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            var ids = new List<int>();
            foreach (string idAsString in array)
            {
                bool successful = int.TryParse(idAsString, out int result);
                if (successful)
                {
                    ids.Add(result);
                }
                else
                {
                    return null;
                }
            }

            return ids;
        }
    }
}
