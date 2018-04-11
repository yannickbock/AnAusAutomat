using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Controllers.Proprietary.Internals;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using TimersTimer = System.Timers.Timer;

namespace AnAusAutomat.Controllers.Proprietary
{
    public class ProprietaryController : IController
    {
        private DeviceSettings _settings;
        private ProprietaryDevice _device;
        private TimersTimer _timer;
        private SerialPort _serialPort;

        private bool _activeFlag = false;
        private int _sessionId;
        private Dictionary<int, bool> _socketStates;

        private string _logMessagePrefix;

        public string DeviceIdentifier { get { return _device.Name; } }

        public ProprietaryController(DeviceSettings settings)
        {
            _logMessagePrefix = string.Format("ProprietaryController: [ {0} ] ", DeviceIdentifier);
            _settings = settings;

            _timer = new TimersTimer(200);
            _timer.Elapsed += _timer_Elapsed;

            _sessionId = -1;
            _socketStates = new Dictionary<int, bool>();
        }

        public void Connect()
        {
            _device = hello(_settings.Name);

            _serialPort = buildConnection(_device.SerialPort);
            _serialPort.Open();
            Thread.Sleep(15);
            _timer.Start();
        }

        public void Disconnect()
        {
            _timer.Stop();
            _serialPort.Close();
        }

        public bool TurnOff(Socket socket)
        {
            int internalID = convertSocketIDToInternalID(socket.ID);
            _socketStates[internalID] = false;

            return turnOnOrOff(internalID, false);
        }

        public bool TurnOn(Socket socket)
        {
            int internalID = convertSocketIDToInternalID(socket.ID);
            _socketStates[internalID] = true;

            return turnOnOrOff(internalID, true);
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_activeFlag)
            {
                int currentSessionId = ping();
                bool connectionLost = currentSessionId == -1;
                bool connectionRestored = currentSessionId != _sessionId;

                if (connectionLost)
                {
                    var device = hello(_device.Name);

                    if (device != null)
                    {
                        _device = device;
                        Connect();
                    }
                }
                else if (connectionRestored)
                {
                    try
                    {
                        foreach (var socketState in _socketStates)
                        {
                            turnOnOrOff(socketId: socketState.Key, powerOn: socketState.Value);
                        }
                        _sessionId = currentSessionId;
                    }
                    catch (Exception)
                    {
                        // Sometimes an exception is thrown in the loop above.
                        // I think this happens if a socket is turned on or off while I´m trying to resend the states to the device.
                        // It should not be a problem because I try to resend it again when the timer has elapsed the next time.
                    }
                }
            }
        }

        private ProprietaryDevice hello(string name)
        {
            return hello(3).FirstOrDefault(x => x.Name == name); ;
        }

        private IEnumerable<ProprietaryDevice> hello(int retries)
        {
            return Enumerable.Range(0, retries).SelectMany(x => hello()).Distinct().ToList();
        }

        private IEnumerable<ProprietaryDevice> hello()
        {
            var bytes = ByteBuilder.Hello();

            var list = new List<ProprietaryDevice>();
            foreach (var serialPort in SerialPort.GetPortNames())
            {
                var connection = buildConnection(serialPort);
                string result = "";
                try
                {
                    connection.Open();
                    Thread.Sleep(15);
                    connection.Write(bytes, 0, bytes.Length);
                    Thread.Sleep(15);
                    result = connection.ReadExisting();
                    connection.Close();

                    if (result.Contains("AnAusAutomat"))
                    {
                        var split = result.Split('|');
                        list.Add(new ProprietaryDevice(
                            name: split.ElementAt(1),
                            socketIDs: split.ElementAt(2).Replace("{", "").Replace("}", "").Split(';').Select(x => int.Parse(x)).ToList(),
                            serialPort: serialPort));
                    }
                }
                catch (Exception)
                {
                    // No AnAusAutomat. Wrong Sketch. Connection Problems...
                }
            }

            return list;
        }

        private bool turnOnOrOff(int socketId, bool powerOn)
        {
            var command = powerOn ? ByteBuilder.TurnOn(socketId) : ByteBuilder.TurnOff(socketId);
            bool successful = false;

            _activeFlag = true;
            try
            {
                _serialPort.Write(command, 0, command.Length);
                Thread.Sleep(15);
                successful = (_serialPort.ReadChar() - 48) == 1;
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    string.Format("{0} An error occurred while turning {1} socket with id {1}",
                        _logMessagePrefix,
                        powerOn ? "on" : "off",
                        convertInternalIDToSocketID(socketId)
                ));
            }
            _activeFlag = false;

            return successful;
        }

        private int ping()
        {
            var bytes = ByteBuilder.Ping();
            int sessionId = -1;

            try
            {
                _serialPort.Write(bytes, 0, bytes.Length);
                Thread.Sleep(15);
                string result = _serialPort.ReadExisting();

                sessionId = int.Parse(result);
            }
            catch (Exception)
            {
            }

            return sessionId;
        }

        private SerialPort buildConnection(string port)
        {
            return new SerialPort(port, 38400)
            {
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };
        }

        private int convertInternalIDToSocketID(int internalID)
        {
            return _settings.Mapping.ContainsValue(internalID) ? _settings.Mapping.FirstOrDefault(x => x.Value == internalID).Key : -1;
        }

        private int convertSocketIDToInternalID(int socketID)
        {
            return _settings.Mapping.ContainsKey(socketID) ? _settings.Mapping[socketID] : -1;
        }
    }
}
