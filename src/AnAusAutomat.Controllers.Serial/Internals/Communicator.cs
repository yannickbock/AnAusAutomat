using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace AnAusAutomat.Controllers.Serial.Internals
{
    public class Communicator
    {
        private SerialPort _serialPort;

        public Communicator()
        {

        }     

        public SerialDevice Search(string name)
        {
            var result = Search();
            return result.Any(x => x.Name == name) ? result.FirstOrDefault(x => x.Name == name) : null;
        }

        public IEnumerable<SerialDevice> Search()
        {
            var bytes = ByteBuilder.Hello();
            var parser = new ResponseParser();

            var list = new List<SerialDevice>();
            foreach (var serialPort in SerialPort.GetPortNames())
            {
                var connection = buildConnection(serialPort);
                string response = "";
                try
                {
                    connection.Open();
                    Thread.Sleep(15);
                    connection.Write(bytes, 0, bytes.Length);
                    Thread.Sleep(15);
                    response = connection.ReadExisting();
                    connection.Close();
                    
                    bool successful = parser.ParseHello(response, out string name, out IEnumerable<int> sockets);
                    if (successful)
                    {
                        list.Add(new SerialDevice(name, sockets, serialPort));
                    }
                }
                catch (Exception)
                {
                    // No AnAusAutomat. Wrong Sketch. Connection Problems...
                }
            }

            return list;
        }

        public void Connect(string port)
        {
            _serialPort = buildConnection(port);
            _serialPort.Open();
            Thread.Sleep(15);
        }

        public void Disconnect()
        {
            _serialPort.Close();
        }

        public bool TurnOff(int internalID)
        {
            var bytes = ByteBuilder.TurnOff(internalID);

            try
            {
                _serialPort.Write(bytes, 0, bytes.Length);
                Thread.Sleep(15);
                return (_serialPort.ReadChar() - 48) == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TurnOn(int internalID)
        {
            var bytes = ByteBuilder.TurnOn(internalID);

            try
            {
                _serialPort.Write(bytes, 0, bytes.Length);
                Thread.Sleep(15);
                return (_serialPort.ReadChar() - 48) == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int Ping()
        {
            var bytes = ByteBuilder.Ping();

            try
            {
                _serialPort.Write(bytes, 0, bytes.Length);
                Thread.Sleep(15);
                string result = _serialPort.ReadExisting();
                return int.Parse(result);
            }
            catch (Exception)
            {
                return -1;
            }
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
    }
}
