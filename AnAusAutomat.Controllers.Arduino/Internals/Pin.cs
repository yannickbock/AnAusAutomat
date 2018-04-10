namespace AnAusAutomat.Controllers.Arduino.Internals
{
    public class Pin
    {
        public Pin(int socketID, int address, string name, PinLogic logic)
        {
            SocketID = socketID;
            Address = address;
            Name = name;
            Logic = logic;
        }

        public int SocketID { get; private set; }

        public int Address { get; private set; }

        public string Name { get; private set; }

        public PinLogic Logic { get; private set; }

        public override string ToString()
        {
            return string.Format("Pin [ SocketID: {0} | Address: {1} | Name: {2} | Logic: {3} ]", SocketID, Address, Name, Logic);
        }
    }
}
