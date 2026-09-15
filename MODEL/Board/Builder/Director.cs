using projektRPG.MODEL.Board.Themes;

namespace projektRPG.MODEL.Board.Builder
{
    /// <summary>
    /// Orchestrates dungeon construction by invoking the same steps on every registered builder.
    /// Pattern: Director — runs a fixed build sequence without knowing builder internals.
    /// Uses Strategy (via ITheme.BuildStrategy) to pick map parameters per theme.
    /// </summary>
    public class Director
    {
        private List<IBuilder> builders = new List<IBuilder>();

        /// <summary>Registers a builder that will participate in the next construction run.</summary>
        public void AddBuilder(IBuilder builder) => builders.Add(builder);

        public void ConstructEmpty()
        {
            foreach (var builder in builders)
                builder.BuildEmpty();
        }

        public void ConstructFull()
        {
            foreach (var builder in builders)
                builder.BuildFull();
        }

        public void ConstructRoom(int x, int y)
        {
            foreach (var builder in builders)
            {
                builder.BuildFull();
                builder.AddRoom(x, y);
            }
        }

        /// <summary>Builds a dungeon using hard-coded default parameters.</summary>
        public void ConstructDungeon()
        {
            foreach (var builder in builders)
            {
                builder.BuildFull();

                builder.AddCorridors(30);
                builder.AddChambers(5);
                builder.AddRoom(10, 5);

                builder.AddItems(20);
                builder.AddWeapons(20);
                builder.AddMoneyAndGold(20);

                builder.AddEnemies(5);
            }
        }

        /// <summary>
        /// Builds a themed dungeon. Map layout counts come from the theme's Strategy object.
        /// Both LabyrinthBuilder and DescriptionBuilder receive the same step sequence.
        /// </summary>
        public void ConstructDungeon(ITheme theme)
        {
            var strategy = theme.BuildStrategy;

            foreach (var builder in builders)
            {
                builder.BuildFull();

                builder.AddCorridors(strategy.CorridorCount);
                builder.AddChambers(strategy.ChamberCount);
                builder.AddRoom(strategy.RoomWidth, strategy.RoomHeight);

                builder.AddItems(strategy.ItemCount);
                builder.AddWeapons(strategy.WeaponCount);
                builder.AddMoneyAndGold(strategy.CurrencyCount);

                builder.AddEnemies(strategy.EnemyCount);
            }
        }
    }
}
