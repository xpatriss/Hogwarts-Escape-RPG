namespace projektRPG.MODEL.Players.Events
{
    /// <summary>
    /// Reacts when another enemy of the same species dies (weakens, rages, or ignores).
    /// Pattern: Observer (listener side).
    /// </summary>
    public interface IDeathObserver
    {
        void OnDeathNotify(object data);
    }
}
