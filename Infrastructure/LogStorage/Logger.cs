namespace projektRPG.Infrastructure.LogStorage
{
    /// <summary>
    /// Central logging service used by both server and client.
    /// Keeps an in-memory history for the in-game log screen and forwards entries to file storage.
    /// Pattern: Singleton — one Logger instance per running process.
    /// </summary>
    public class Logger
    {
        private static Logger instance;
        private readonly List<string> allLogs = new List<string>();
        private ILogStorage storage;

        private Logger() { }

        /// <summary>Global access point for the logger. Created lazily on first use.</summary>
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                    instance = new Logger();
                return instance;
            }
        }

        /// <summary>
        /// Selects where log entries are persisted (server log dir, client log dir, etc.).
        /// Must be called once at startup before any Log() calls.
        /// </summary>
        public void SetStorage(ILogStorage storage) => this.storage = storage;

        /// <summary>
        /// Records a message with a timestamp. Stored in memory and written to file if storage is configured.
        /// </summary>
        public void Log(string message)
        {
            string entry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            allLogs.Add(entry);
            storage?.Write(entry);
        }

        /// <summary>Returns the full in-memory log history (used by the log screen).</summary>
        public List<string> GetAllLogs() => allLogs;

        /// <summary>Returns the most recent log entries for the compact HUD log panel.</summary>
        public IEnumerable<string> GetRecentLogs(int count) => allLogs.TakeLast(count);
    }
}
