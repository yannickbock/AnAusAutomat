namespace AnAusAutomat.Contracts
{
    public class ConditionMode
    {
        public ConditionMode(string name, bool isActive)
        {
            Name = name;
            IsActive = isActive;
        }

        public string Name { get; private set; }

        public bool IsActive { get; set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + IsActive.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }
    }
}
