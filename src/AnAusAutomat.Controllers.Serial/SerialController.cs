﻿using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Controllers.Serial.Internals;
using AnAusAutomat.Toolbox.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TimersTimer = System.Timers.Timer;

namespace AnAusAutomat.Controllers.Serial
{
    public class SerialController : IController
    {
        private DeviceSettings _settings;
        private SerialDevice _device;
        private TimersTimer _timer;

        private Communicator _communicator;

        private bool _activeFlag = false;
        private int _sessionId;
        private Dictionary<int, bool> _socketStates;

        private bool _connectionLostMessageWritten = false;

        public IDevice Device { get; private set; }

        public SerialController(DeviceSettings settings)
        {
            _settings = settings;
            _communicator = new Communicator();

            _timer = new TimersTimer(200);
            _timer.Elapsed += _timer_Elapsed;

            _sessionId = -1;
            _socketStates = new Dictionary<int, bool>();
        }

        public void Connect()
        {
            _device = _communicator.Search(_settings.Name);
            _communicator.Connect(_device.SerialPort);

            _timer.Start();
        }

        public void Disconnect()
        {
            _timer.Stop();

            _communicator.Disconnect();
        }

        public bool TurnOff(Socket socket)
        {
            int internalID = convertSocketIDToInternalID(socket.ID);
            _socketStates[internalID] = false;

            return _communicator.TurnOff(internalID);
        }

        public bool TurnOn(Socket socket)
        {
            int internalID = convertSocketIDToInternalID(socket.ID);
            _socketStates[internalID] = true;

            return _communicator.TurnOn(internalID);
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _activeFlag = false;

            if (!_activeFlag)
            {
                int currentSessionId = _communicator.Ping();
                bool connectionLost = currentSessionId == -1;
                bool connectionRestored = currentSessionId != _sessionId;

                if (connectionLost)
                {
                    if (!_connectionLostMessageWritten)
                    {
                        Logger.Warning(String.Format("Connection to {0} lost.", _device.Name));
                        _connectionLostMessageWritten = true;
                    }

                    var device = _communicator.Search(_device.Name);

                    if (device != null)
                    {
                        Logger.Information(String.Format("Establishing connection to {0} ...", _device.Name));

                        _device = device;
                        Connect();
                    }
                }
                else if (connectionRestored)
                {
                    Logger.Information(String.Format("Connection to {0} restored.", _device.Name));
                    _connectionLostMessageWritten = false;

                    try
                    {
                        Logger.Information(String.Format("Resending states to {0}.", _device.Name));

                        foreach (var socketState in _socketStates)
                        {
                            bool powerOn = socketState.Value;
                            int internalID = socketState.Key;
                            if (powerOn)
                            {
                                _communicator.TurnOn(internalID);
                            }
                            else
                            {
                                _communicator.TurnOff(internalID);
                            }
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
