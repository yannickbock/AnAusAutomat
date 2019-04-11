using System.Windows.Forms;

namespace AnAusAutomat.Sensors.GUI.HotKeys
{
    public class HotKey
    {
        public HotKey(KeyModifiers modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public KeyModifiers Modifier { get; }

        public Keys Key { get; }

        public override int GetHashCode()
        {
            return Modifier.GetHashCode() + Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
