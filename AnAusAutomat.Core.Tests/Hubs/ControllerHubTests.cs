using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Core.Hubs;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AnAusAutomat.Core.Tests.Hubs
{
    public class ControllerHubTests
    {
        [Fact]
        public void Connect()
        {
            var controllerOne = new Mock<IController>();
            controllerOne.Setup(x => x.Connect());

            var controllerTwo = new Mock<IController>();
            controllerTwo.Setup(x => x.Connect());

            var controllerHub = new ControllerHub(new List<IController> { controllerOne.Object, controllerTwo.Object });
            controllerHub.Connect();

            controllerOne.Verify(x => x.Connect(), Times.Once);
            controllerTwo.Verify(x => x.Connect(), Times.Once);
        }

        [Fact]
        public void Disconnect()
        {
            var controllerOne = new Mock<IController>();
            controllerOne.Setup(x => x.Disconnect());

            var controllerTwo = new Mock<IController>();
            controllerTwo.Setup(x => x.Disconnect());

            var controllerHub = new ControllerHub(new List<IController> { controllerOne.Object, controllerTwo.Object });
            controllerHub.Disconnect();

            controllerOne.Verify(x => x.Disconnect(), Times.Once);
            controllerTwo.Verify(x => x.Disconnect(), Times.Once);
        }

        [Fact]
        public void TurnOn()
        {
            var socket = new Socket(1, "Toster");

            var controllerOne = new Mock<IController>();
            controllerOne.Setup(x => x.TurnOn(socket));

            var controllerTwo = new Mock<IController>();
            controllerTwo.Setup(x => x.TurnOn(socket));

            var controllerHub = new ControllerHub(new List<IController> { controllerOne.Object, controllerTwo.Object });
            controllerHub.TurnOn(socket);

            controllerOne.Verify(x => x.TurnOn(socket), Times.Once);
            controllerTwo.Verify(x => x.TurnOn(socket), Times.Once);
        }

        [Fact]
        public void TurnOff()
        {
            var socket = new Socket(1, "Toster");

            var controllerOne = new Mock<IController>();
            controllerOne.Setup(x => x.TurnOff(socket));

            var controllerTwo = new Mock<IController>();
            controllerTwo.Setup(x => x.TurnOff(socket));

            var controllerHub = new ControllerHub(new List<IController> { controllerOne.Object, controllerTwo.Object });
            controllerHub.TurnOff(socket);

            controllerOne.Verify(x => x.TurnOff(socket), Times.Once);
            controllerTwo.Verify(x => x.TurnOff(socket), Times.Once);
        }
    }
}
