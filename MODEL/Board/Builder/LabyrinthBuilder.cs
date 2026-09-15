using projektRPG.Infrastructure;
using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Board.Themes;
using projektRPG.MODEL.Items;
using projektRPG.MODEL.Players.Enemies;
using projektRPG.MODEL.Players.Events;

namespace projektRPG.MODEL.Board.Builder
{
    /// <summary>
    /// Procedurally generates the dungeon layout and places themed content on the map.
    /// Pattern: Builder — Director calls each step; GetMap() returns the final product.
    /// Uses ITheme (Abstract Factory) to create items, weapons, currency, and enemies.
    /// Enemies subscribe to INoisePublisher / IGameEventRecorder passed in from GameModel (Observer).
    /// </summary>
    public class LabyrinthBuilder : IBuilder
    {
        // Internal tile map: '#' = wall, '.' = floor (before conversion to Field objects)
        char[,] pattern = new char[Consts.MapHeight, Consts.MapWidth];

        private int itemsToPlace = 0;
        private int weaponsToPlace = 0;
        private int currencyToPlace = 0;
        private int enemiesToPlace = 0;

        private ITheme theme;
        private INoisePublisher noisePublisher;
        private IGameEventRecorder eventRecorder;

        public LabyrinthBuilder(ITheme theme, INoisePublisher noisePublisher, IGameEventRecorder eventRecorder)
        {
            this.theme = theme;
            this.noisePublisher = noisePublisher;
            this.eventRecorder = eventRecorder;
            Reset();
        }

        public LabyrinthBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            itemsToPlace = 0;
            weaponsToPlace = 0;
            currencyToPlace = 0;
            enemiesToPlace = 0;
            BuildEmpty();
        }

        public void BuildEmpty()
        {
            for (int i = 0; i < Consts.MapHeight; i++)
            {
                for (int j = 0; j < Consts.MapWidth; j++)
                    pattern[i, j] = '.';
            }
        }

        public void BuildFull()
        {
            for (int i = 0; i < Consts.MapHeight; i++)
            {
                for (int j = 0; j < Consts.MapWidth; j++)
                    pattern[i, j] = '#';
            }
        }

        /// <summary>Carves random square chambers into the wall grid.</summary>
        public void AddChambers(int n)
        {
            Random random = new Random();

            for (int k = 0; k < n; k++)
            {
                int d = random.Next(2, 4);
                int x = random.Next(0, Consts.MapWidth - d);
                int y = random.Next(0, Consts.MapHeight - d);

                for (int j = y; j < y + d; j++)
                {
                    for (int i = x; i < x + d; i++)
                    {
                        pattern[j, i] = '.';
                    }
                }
            }
        }

        /// <summary>Carves alternating horizontal and vertical corridors.</summary>
        public void AddCorridors(int n)
        {
            Random random = new Random();

            for (int k = 0; k < n; k++)
            {
                int l = k % 2;

                if (l == 0)
                {
                    int d = random.Next(1, Consts.MapWidth);
                    int x = random.Next(0, Consts.MapWidth - d);
                    int y = random.Next(0, Consts.MapHeight - 1);

                    for (int i = x; i < x + d; i++)
                    {
                        pattern[y, i] = '.';
                    }
                }
                else
                {
                    int d = random.Next(1, Consts.MapHeight);
                    int x = random.Next(0, Consts.MapWidth - 1);
                    int y = random.Next(0, Consts.MapHeight - d);

                    for (int i = y; i < y + d; i++)
                    {
                        pattern[i, x] = '.';
                    }
                }
            }
        }

        /// <summary>Carves a bordered rectangular room in the center of the map.</summary>
        public void AddRoom(int x, int y)
        {
            if (x > Consts.MapWidth || y > Consts.MapHeight || x < 0 || y < 0)
                return;

            int sY = Consts.MapHeight / 2 - y / 2;
            int sX = Consts.MapWidth / 2 - x / 2;
            for (int i = sY; i < sY + y; i++)
            {
                pattern[i, sX - 1] = '#';
                pattern[i, sX + x] = '#';
                for (int j = sX; j < sX + x; j++)
                {
                    pattern[i, j] = '.';
                    pattern[sY - 1, j] = '#';
                    pattern[sY + y, j] = '#';
                }
            }
        }

        public void AddItems(int n) => itemsToPlace = n;
        public void AddWeapons(int n) => weaponsToPlace = n;
        public void AddMoneyAndGold(int n) => currencyToPlace = n;
        public void AddEnemies(int n) => enemiesToPlace = n;

        /// <summary>
        /// Places the theme artifact, items, weapons, currency, and enemies on random floor tiles.
        /// Objects are never placed on the player spawn tile.
        /// </summary>
        private void PlaceObjectsOnMap(GameMap map)
        {
            Random rand = new Random();
            Item item;

            int placedItems = 0;
            int placedWeapons = 0;
            int placedCurrency = 0;
            int placedEnemies = 0;

            var artifact = theme.Artifact();
            while (true)
            {
                int x = rand.Next(Consts.PlayerStartX + 1, Consts.MapWidth);
                int y = rand.Next(Consts.PlayerStartY + 1, Consts.MapHeight);

                if (pattern[y, x] == '.' && map.GetField(x + 1, y + 1).Items.Count < Consts.FieldCapacity)
                {
                    map.GetField(x + 1, y + 1).AddItem(artifact);
                    break;
                }
            }

            while (placedItems < itemsToPlace)
            {
                int x = rand.Next(Consts.PlayerStartX + 1, Consts.MapWidth);
                int y = rand.Next(Consts.PlayerStartY + 1, Consts.MapHeight);

                if (pattern[y, x] == '.' && map.GetField(x + 1, y + 1).Items.Count < Consts.FieldCapacity)
                {
                    item = theme.CreateItem();
                    map.GetField(x + 1, y + 1).AddItem(item);
                    placedItems++;
                }
            }

            while (placedWeapons < weaponsToPlace)
            {
                int x = rand.Next(Consts.PlayerStartX + 1, Consts.MapWidth);
                int y = rand.Next(Consts.PlayerStartY + 1, Consts.MapHeight);

                if (pattern[y, x] == '.' && map.GetField(x + 1, y + 1).Items.Count < Consts.FieldCapacity)
                {
                    item = theme.CreateWeapon();
                    map.GetField(x + 1, y + 1).AddItem(item);
                    placedWeapons++;
                }
            }

            while (placedCurrency < currencyToPlace)
            {
                int x = rand.Next(Consts.PlayerStartX + 1, Consts.MapWidth);
                int y = rand.Next(Consts.PlayerStartY + 1, Consts.MapHeight);

                if (pattern[y, x] == '.' && map.GetField(x + 1, y + 1).Items.Count < Consts.FieldCapacity)
                {
                    item = theme.CreateCurrency();
                    map.GetField(x + 1, y + 1).AddItem(item);
                    placedCurrency++;
                }
            }

            while (placedEnemies < enemiesToPlace)
            {
                int x = rand.Next(Consts.MapWidth);
                int y = rand.Next(Consts.MapHeight);

                if (pattern[y, x] == '.' && x + 1 != Consts.PlayerStartX && y + 1 != Consts.PlayerStartY)
                {
                    Enemy enemy = theme.CreateEnemy(noisePublisher, eventRecorder);
                    enemy.SetPosition(x + 1, y + 1);
                    map.GetField(x + 1, y + 1).AddEnemy(enemy);

                    placedEnemies++;
                }
            }
        }

        /// <summary>Adds a one-tile-thick wall border around the playable area.</summary>
        public void GenerateFrame(Field[,] grid)
        {
            for (int i = 0; i <= Consts.MapHeight + 1; i++)
            {
                grid[i, 0] = new Wall();
                grid[i, Consts.MapWidth + 1] = new Wall();
            }

            for (int i = 0; i <= Consts.MapWidth + 1; i++)
            {
                grid[0, i] = new Wall();
                grid[Consts.MapHeight + 1, i] = new Wall();
            }
        }

        /// <summary>
        /// Finalizes the build: ensures all floor areas are connected, converts the pattern to a GameMap,
        /// places objects, and resets the builder for potential reuse.
        /// </summary>
        public GameMap GetMap()
        {
            EnsureConnectivity();

            Field[,] finalGrid = new Field[Consts.MapHeight + 2, Consts.MapWidth + 2];

            for (int i = 0; i < Consts.MapHeight; i++)
            {
                for (int j = 0; j < Consts.MapWidth; j++)
                {
                    if (j + 1 == Consts.PlayerStartX && i + 1 == Consts.PlayerStartY)
                    {
                        finalGrid[i + 1, j + 1] = new Empty();
                    }
                    else if (pattern[i, j] == '#')
                    {
                        finalGrid[i + 1, j + 1] = new Wall();
                    }
                    else
                    {
                        finalGrid[i + 1, j + 1] = new Empty();
                    }
                }
            }

            GenerateFrame(finalGrid);

            GameMap resultMap = new GameMap(finalGrid);

            PlaceObjectsOnMap(resultMap);

            Reset();
            return resultMap;
        }

        /// <summary>
        /// Connects isolated floor regions with L-shaped corridors so every area is reachable.
        /// The region containing the player spawn is treated as the main area.
        /// </summary>
        private void EnsureConnectivity()
        {
            pattern[Consts.PlayerStartY - 1, Consts.PlayerStartX - 1] = '.';

            List<List<(int y, int x)>> areas = FindAllAreas();

            if (areas.Count <= 1) return;

            var mainArea = areas.Find(a => a.Contains((Consts.PlayerStartY, Consts.PlayerStartX))) ?? areas[0];

            foreach (var area in areas)
            {
                if (area == mainArea) continue;

                var (p1, p2) = FindClosestPoints(area, mainArea);

                CreateSimpleCorridor(p1, p2);
            }
        }

        private ((int y, int x) p1, (int y, int x) p2) FindClosestPoints(List<(int y, int x)> area1, List<(int y, int x)> area2)
        {
            double minDistance = double.MaxValue;
            (int y, int x) bestP1 = area1[0];
            (int y, int x) bestP2 = area2[0];

            foreach (var p1 in area1)
            {
                foreach (var p2 in area2)
                {
                    double dist = Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        bestP1 = p1;
                        bestP2 = p2;
                    }
                }
            }
            return (bestP1, bestP2);
        }

        /// <summary>Flood-fills the pattern grid to find all disconnected walkable regions.</summary>
        private List<List<(int y, int x)>> FindAllAreas()
        {
            var areas = new List<List<(int y, int x)>>();
            bool[,] visited = new bool[Consts.MapHeight, Consts.MapWidth];

            for (int i = 0; i < Consts.MapHeight; i++)
            {
                for (int j = 0; j < Consts.MapWidth; j++)
                {
                    if (pattern[i, j] == '.' && !visited[i, j])
                    {
                        var newArea = new List<(int y, int x)>();
                        Queue<(int y, int x)> q = new Queue<(int y, int x)>();
                        q.Enqueue((i, j));
                        visited[i, j] = true;

                        while (q.Count > 0)
                        {
                            var curr = q.Dequeue();
                            newArea.Add(curr);

                            foreach (var neighbor in GetNeighbors(curr.y, curr.x))
                            {
                                if (!visited[neighbor.y, neighbor.x] && pattern[neighbor.y, neighbor.x] == '.')
                                {
                                    visited[neighbor.y, neighbor.x] = true;
                                    q.Enqueue(neighbor);
                                }
                            }
                        }
                        areas.Add(newArea);
                    }
                }
            }
            return areas;
        }

        private List<(int y, int x)> GetNeighbors(int y, int x)
        {
            var neighbors = new List<(int y, int x)>();
            int[] dy = { -1, 1, 0, 0 };
            int[] dx = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int ny = y + dy[i];
                int nx = x + dx[i];

                if (ny >= 0 && ny < Consts.MapHeight && nx >= 0 && nx < Consts.MapWidth)
                {
                    neighbors.Add((ny, nx));
                }
            }
            return neighbors;
        }

        /// <summary>Carves an L-shaped corridor between two floor coordinates.</summary>
        private void CreateSimpleCorridor((int y, int x) start, (int y, int x) end)
        {
            int y = start.y;
            while (y != end.y)
            {
                pattern[y, start.x] = '.';
                y += end.y > y ? 1 : -1;
            }
            int x = start.x;
            while (x != end.x)
            {
                pattern[end.y, x] = '.';
                x += end.x > x ? 1 : -1;
            }
            pattern[end.y, end.x] = '.';
        }
    }
}
