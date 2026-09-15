namespace projektRPG.MODEL.Board.Builder
{
    /// <summary>
    /// Common construction steps for building a dungeon and its intro description.
    /// Pattern: Builder — concrete builders (LabyrinthBuilder, DescriptionBuilder) implement these steps.
    /// </summary>
    public interface IBuilder
    {
        /// <summary>Fills the layout with walkable floor tiles.</summary>
        void BuildEmpty();

        /// <summary>Fills the layout with wall tiles.</summary>
        void BuildFull();

        /// <summary>Carves horizontal and vertical corridors into the map.</summary>
        void AddCorridors(int n);

        /// <summary>Carves small square chambers into the map.</summary>
        void AddChambers(int n);

        /// <summary>Adds a centered rectangular room (map builder) or room lore (description builder).</summary>
        void AddRoom(int x, int y);

        /// <summary>Schedules consumable items to be placed (map) or documents them (description).</summary>
        void AddItems(int n);

        /// <summary>Schedules weapons to be placed (map) or documents them (description).</summary>
        void AddWeapons(int n);

        /// <summary>Schedules currency to be placed (map) or documents it (description).</summary>
        void AddMoneyAndGold(int n);

        /// <summary>Schedules enemies to be placed (map) or documents them (description).</summary>
        void AddEnemies(int n);

        /// <summary>Resets internal state before a new build.</summary>
        void Reset();
    }
}
