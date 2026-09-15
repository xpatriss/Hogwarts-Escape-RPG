namespace projektRPG.Infrastructure.Config
{
    /// <summary>
    /// Holds runtime settings loaded from config.json or set during game launch.
    /// Shared by both server and client processes.
    /// </summary>
    public class ConfigData
    {
        /// <summary>Display name of the local player (can be overridden when joining as a client).</summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// Dungeon theme used when the server creates the game world.
        /// Supported values: HogwartsDungeons, TriwizardMaze, Library.
        /// </summary>
        public string DungeonTheme { get; set; }

        /// <summary>Directory where the server writes its log files.</summary>
        public string LogFilePath { get; set; }
    }
}
