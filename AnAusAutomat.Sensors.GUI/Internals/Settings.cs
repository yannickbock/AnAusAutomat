namespace AnAusAutomat.Sensors.GUI.Internals
{
    public class Settings
    {
        public Settings(bool startMinimized)
        {
            StartMinimized = startMinimized;
        }

        public bool StartMinimized { get; private set; }
    }
}
