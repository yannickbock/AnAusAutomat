namespace AnAusAutomat.Contracts.Controller
{
    public class Pin
    {
        public Pin(int address, string name, PinLogic logic)
        {
            Address = address;
            Name = name;
            Logic = logic;
        }

        public int Address { get; private set; }

        public string Name { get; private set; }

        public PinLogic Logic { get; private set; }

        public override string ToString()
        {
            return string.Format("Pin [ Address: {0} | Name: {1} | Logic: {2} ]", Address, Name, Logic);
        }
    }
}
