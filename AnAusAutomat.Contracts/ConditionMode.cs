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
    }
}
