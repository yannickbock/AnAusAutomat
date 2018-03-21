using System;

namespace AnAusAutomat.Contracts
{
    public class Socket : IEquatable<Socket>
    {
        public Socket(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public int ID { get; private set; }

        public string Name { get; private set; }

        public override string ToString()
        {
            return string.Format("Socket [ {0}: {1} ]", ID, Name);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode() + Name.GetHashCode();
        }

        public bool Equals(Socket other)
        {
            return ID == other.ID && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Socket;
            if (other == null) return false;

            return Equals(other);
        }
    }
}
