using System.IO;
using System.Text.Json;

namespace projektRPG.Infrastructure.Config
{
    /// <summary>
    /// Provides application-wide access to game configuration.
    /// Pattern: Singleton — only one ConfigurationManager exists for the entire process.
    /// </summary>
    public class ConfigurationManager
    {
        private static ConfigurationManager instance;

        /// <summary>Currently loaded configuration values.</summary>
        public ConfigData Data { get; private set; }

        private ConfigurationManager() { }

        /// <summary>
        /// Global access point for the configuration manager.
        /// Creates the instance lazily on first use.
        /// </summary>
        public static ConfigurationManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ConfigurationManager();
                return instance;
            }
        }

        /// <summary>
        /// Loads settings from a JSON file. If the file is missing, falls back to built-in defaults.
        /// Called once at startup from Program.Main.
        /// </summary>
        /// <param name="path">Path to the config file (default: config.json in the working directory).</param>
        public void LoadConfig(string path = "config.json")
        {
            if (!File.Exists(path))
            {
                Data = new ConfigData
                {
                    PlayerName = "Wizard",
                    DungeonTheme = "HogwartsDungeons",
                    LogFilePath = "logs"
                };
                return;
            }

            string jsonString = File.ReadAllText(path);
            Data = JsonSerializer.Deserialize<ConfigData>(jsonString);
        }
    }
}
