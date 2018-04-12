using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Attributes;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Sensors.TimeSwitch.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Timers;

namespace AnAusAutomat.Sensors.TimeSwitch
{
    [Parameter("powerOn", typeof(DateTime), "", "HH:mm:ss")]
    [Parameter("powerOff", typeof(DateTime), "", "HH:mm:ss")]
    [Description("Fires turn on or turn off event at a specific time.")]
    public class TimeSwitch : ISensor
    {
        private Timer _timer;
        private IEnumerable<Cache> _cache;

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public void Initialize(SensorSettings settings)
        {
            _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _cache = settings.Sockets.Select(x => new Cache(x, parseParameters(x.Parameters))).ToList();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var cache in _cache)
            {
                bool powerOn = cache.Parameters.PowerOn.Any(x => (int)((DateTime.Now.TimeOfDay - x.TimeOfDay).TotalSeconds) == 0);
                bool powerOff = cache.Parameters.PowerOff.Any(x => (int)((DateTime.Now.TimeOfDay - x.TimeOfDay).TotalSeconds) == 0);

                if (powerOn)
                {
                    StatusChanged?.Invoke(this, new StatusChangedEventArgs("", "", cache.Socket, PowerStatus.On));
                }
                if (powerOff)
                {
                    StatusChanged?.Invoke(this, new StatusChangedEventArgs("", "", cache.Socket, PowerStatus.Off));
                }
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private Parameters parseParameters(IEnumerable<SensorParameter> parameters)
        {
            var powerOn = new List<DateTime>();
            var powerOff = new List<DateTime>();

            foreach (var parameter in parameters)
            {
                DateTime result;
                bool successful = DateTime.TryParseExact(parameter.Value, "HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out result);

                if (successful)
                {
                    if (parameter.Name.ToLower() == "on")
                    {
                        powerOn.Add(result);
                    }
                    else if (parameter.Name.ToLower() == "off")
                    {
                        powerOff.Add(result);
                    }
                }
                //else log...
            }

            return new Parameters(powerOn, powerOff);
        }
    }
}
