namespace projektRPG.MODEL.Players.Events
{
    /// <summary>
    /// Reacts to player-generated noise (item pickup/drop). Enemies implement this interface.
    /// Pattern: Observer (listener side).
    /// </summary>
    public interface INoiseObserver
    {
        void OnNoiseNotify(object data);
    }
}
