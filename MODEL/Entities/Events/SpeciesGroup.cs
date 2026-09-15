namespace projektRPG.MODEL.Players.Events
{
    /// <summary>
    /// Groups enemies of the same species so they can react to each other's deaths.
    /// Pattern: Observer — SpeciesGroup is the publisher, enemy subclasses are observers.
    /// When one enemy dies, Notify() informs all other members of the group.
    /// </summary>
    public class SpeciesGroup : IDeathPublisher
    {
        private readonly List<IDeathObserver> observers = new List<IDeathObserver>();

        public string SpeciesName { get; }

        public SpeciesGroup(string name) => SpeciesName = name;

        public void Attach(IDeathObserver observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        public void Detach(IDeathObserver observer) => observers.Remove(observer);

        public void Notify(object data)
        {
            foreach (var observer in observers.ToList())
            {
                // Skip the enemy that just died — only notify the survivors.
                if (observer != data)
                {
                    observer.OnDeathNotify(data);
                }
            }
        }
    }
}
