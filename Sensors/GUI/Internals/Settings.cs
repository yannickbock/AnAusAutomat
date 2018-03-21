namespace GUI.Internals
{
    internal class Settings
    {
        internal Settings(bool startMinimized)
        {
            StartMinimized = startMinimized;
        }

        internal bool StartMinimized { get; private set; }
    }
}
