namespace projektRPG.MODEL.Players.Events
{
    /// <summary>
    /// Publishes noise events when the player picks up or drops loud items.
    /// Implemented by GameModel; enemies subscribe as INoiseObserver.
    /// Pattern: Observer (subject side).
    /// </summary>
    public interface INoisePublisher
    {
        void Attach(INoiseObserver observer);
        void Detach(INoiseObserver observer);

        /// <summary>Broadcasts noise data: (sourceX, sourceY, noiseRange, map).</summary>
        void Notify(object data);
    }
}
