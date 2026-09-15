using projektRPG.Infrastructure;
using projektRPG.MODEL.Board.Builder;
using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Board.Themes;
using projektRPG.MODEL.Players;
using projektRPG.MODEL.Players.Enemies;
using projektRPG.MODEL.Players.Events;

namespace projektRPG.MODEL
{
    /// <summary>
    /// Authoritative server-side game state: map, players, enemies, and intro description.
    /// Pattern: Observer — implements INoisePublisher (notifies enemies of player noise)
    /// and IGameEventRecorder (collects server-side event logs).
    /// Map creation uses Builder + Director with a theme selected from config.
    /// </summary>
    public class GameModel : INoisePublisher, IGameEventRecorder
    {
        private readonly List<INoiseObserver> _noiseObservers = new List<INoiseObserver>();
        private readonly List<string> _pendingServerLogs = new List<string>();

        public IReadOnlyList<string> PendingServerLogs => _pendingServerLogs;

        public void RecordServerLog(string message) => _pendingServerLogs.Add(message);

        public void ClearServerLogs() => _pendingServerLogs.Clear();

        public void Attach(INoiseObserver obs) => _noiseObservers.Add(obs);
        public void Detach(INoiseObserver obs) => _noiseObservers.Remove(obs);

        /// <summary>Broadcasts a noise event (position, range, map) to all subscribed enemies.</summary>
        public void Notify(object data)
        {
            foreach (var obs in _noiseObservers.ToList()) obs.OnNoiseNotify(data);
        }

        public GameMap Map { get; private set; }
        public List<Player> Players { get; private set; } = new List<Player>();
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();

        private Director director;
        private LabyrinthBuilder labyrinthBuilder;

        /// <summary>Intro plot text built by DescriptionBuilder, sent to clients on connect.</summary>
        public string description;
        private DescriptionBuilder descriptionBuilder;

        /// <summary>
        /// Creates the world for the given theme name, builds map + intro, and indexes all enemies.
        /// </summary>
        public GameModel(string themeName)
        {
            ITheme selectedTheme = themeName switch
            {
                "HogwartsDungeons" => new HogwartsDungeonsTheme(),
                "TriwizardMaze" => new TriwizardMazeTheme(),
                "Library" => new LibraryTheme(),
                _ => new HogwartsDungeonsTheme()
            };

            // Builder + Director: LabyrinthBuilder produces the map, DescriptionBuilder produces the intro text.
            labyrinthBuilder = new LabyrinthBuilder(selectedTheme, this, this);
            descriptionBuilder = new DescriptionBuilder(selectedTheme);
            director = new Director();
            director.AddBuilder(labyrinthBuilder);
            director.AddBuilder(descriptionBuilder);
            director.ConstructDungeon(selectedTheme);

            Map = labyrinthBuilder.GetMap();
            description = descriptionBuilder.GetResult();

            ExtractEnemies();
        }

        /// <summary>Spawns a new networked player at the default start position.</summary>
        public void AddNewPlayer(int id, string playerName)
        {
            var newPlayer = new Player(playerName, (Consts.PlayerStartX, Consts.PlayerStartY));
            newPlayer.Id = id;

            if (!string.IsNullOrEmpty(playerName))
            {
                newPlayer.Symbol = char.ToUpper(playerName[0]);
            }

            Players.Add(newPlayer);
        }

        public void RemovePlayer(int id)
        {
            var playerToRemove = Players.FirstOrDefault(p => p.Id == id);

            if (playerToRemove != null)
            {
                Players.Remove(playerToRemove);
            }
        }

        /// <summary>Resolves a player by network session id (used when processing client actions).</summary>
        public Player GetPlayerById(int playerId)
        {
            return Players.FirstOrDefault(p => p.Id == playerId);
        }

        /// <summary>Scans the map grid and populates the Enemies list for the server update loop.</summary>
        private void ExtractEnemies()
        {
            Enemies.Clear();
            for (int y = 0; y < Consts.MapHeight + 2; y++)
            {
                for (int x = 0; x < Consts.MapWidth + 2; x++)
                {
                    var e = Map.GetField(x, y).enemy;
                    if (e != null) Enemies.Add(e);
                }
            }
        }

        /// <summary>
        /// Runs one enemy turn for all living enemies. Skips enemies standing on a player's tile.
        /// Called periodically by ServerController.
        /// </summary>
        public void UpdateEnemies()
        {
            Enemies.RemoveAll(e => e.HP <= 0);

            foreach (var enemy in Enemies)
            {
                if (enemy == null) continue;
                bool isPlayerHere = Players.Any(p => p.Position.x == enemy.Position.x && p.Position.y == enemy.Position.y);
                if (isPlayerHere) continue;
                enemy.TakeTurn(Map, Players);
            }
        }
    }
}
