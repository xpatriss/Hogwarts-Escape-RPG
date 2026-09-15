namespace projektRPG.MODEL.Players.Events
{
    /// <summary>
    /// Generic observer interface (legacy). Prefer INoiseObserver / IDeathObserver for typed events.
    /// Pattern: Observer.
    /// </summary>
    public interface IObserver
    {
        void OnNotify(string eventType, object data);
    }
}
