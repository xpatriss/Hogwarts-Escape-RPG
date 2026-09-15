namespace projektRPG.MODEL.Players.Events
{
    /// <summary>
    /// Collects server-side diagnostic messages (enemy AI, combat events).
    /// Implemented by GameModel; flushed to Logger by ServerController.
    /// </summary>
    public interface IGameEventRecorder
    {
        void RecordServerLog(string message);
    }
}
