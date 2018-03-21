using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Controller;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace AnAusAutomat.Core.Hubs
{
    // chain of responsibility pattern?
    public class ControllerHub
    {
        private IEnumerable<Device> _devices;
        private IEnumerable<IController> _controllers;

        public ControllerHub(IEnumerable<Device> devices)
        {
            _devices = devices;
            initializeControllers();
        }

        public void Connect()
        {
            _controllers.AsParallel().ForAll(x => x.Connect());
        }

        public void Disconnect()
        {
            _controllers.AsParallel().ForAll(x => x.Disconnect());
        }

        private void initializeControllers()
        {
            var controllerNames = getControllerNames();
            var availableControllers = getControllerTypes();
            checkIfControllerIsImplementedAndThrowExceptionIfNot(availableControllers, controllerNames);

            _controllers = _devices.Select(device =>
            {
                var associatedControllerType = availableControllers.FirstOrDefault(x => x.Name.Replace("Controller", "") == device.Type);
                var associatedControllerInstance = (IController)Activator.CreateInstance(associatedControllerType);
                associatedControllerInstance.Device = device;
                return associatedControllerInstance;
            }).ToList();
        }

        private IEnumerable<Type> getControllerTypes()
        {
            var controllersAssembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "Controllers.dll");

            return controllersAssembly.GetTypes().Where(x => typeof(IController).IsAssignableFrom(x) && x.IsClass).ToList();
        }

        private void checkIfControllerIsImplementedAndThrowExceptionIfNot(IEnumerable<Type> foundControllers, IEnumerable<string> requiredControllers)
        {
            var foundControllersAsStringList = foundControllers.Select(x => x.Name);
            bool controllerNotImplemented = requiredControllers.Count(x => !foundControllersAsStringList.Contains(x)) > 0;

            if (controllerNotImplemented)
            {
                string message = string.Format(
                    "There is a mismatch between the controllers defined in the config file and the implemented controllers. " +
                    "Config file: {0}. Implemented: {1}",
                    string.Join(", ", requiredControllers),
                    string.Join(", ", foundControllers.Select(x => x.Name))
                    );

                Log.Error(message);

                throw new ObjectNotFoundException(message);
            }
        }

        private IEnumerable<string> getControllerNames()
        {
            return _devices.Select(x => string.Format("{0}Controller", x.Type)).Distinct().ToList();
        }

        public void TurnOn(Socket socket)
        {
            Log.Information(string.Format("Turn on {0}", socket));

            var controllers = GetAssociatedControllers(socket);
            foreach (var controller in controllers)
            {
                Log.Debug(string.Format("Turn on {0}@{1}", socket, controller.Device));
                controller.TurnOn(socket);
            }
        }

        public void TurnOff(Socket socket)
        {
            Log.Information(string.Format("Turn off {0}", socket));

            var controllers = GetAssociatedControllers(socket);
            foreach (var controller in controllers)
            {
                Log.Debug(string.Format("Turn off {0}@{1}", socket, controller.Device));
                controller.TurnOff(socket);
            }
        }

        private IEnumerable<IController> GetAssociatedControllers(Socket socket)
        {
            return _controllers.Where(x => x.Device.Sockets.Contains(socket)).ToList();
        }
    }
}
