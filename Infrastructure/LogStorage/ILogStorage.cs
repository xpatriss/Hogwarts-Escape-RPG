namespace projektRPG.Infrastructure.LogStorage
{
    /// <summary>
    /// Abstraction for persisting log entries (e.g. to a file).
    /// Allows Logger to stay independent of the concrete storage backend.
    /// </summary>
    public interface ILogStorage
    {
        /// <summary>Writes a single formatted log line to the underlying storage.</summary>
        void Write(string message);
    }
}
