namespace projektRPG.MODEL.Players.Events
{
    /// <summary>
    /// Publishes death events within an enemy species group (e.g. all Dementors).
    /// Pattern: Observer (subject side).
    /// </summary>
    public interface IDeathPublisher
    {
        void Attach(IDeathObserver observer);
        void Detach(IDeathObserver observer);

        /// <summary>Notifies all observers except the dead enemy itself.</summary>
        void Notify(object data);
    }
}
