namespace projektRPG.MODEL.Board.Strategy
{
    /// <summary>
    /// Defines procedural generation parameters for a dungeon layout.
    /// Pattern: Strategy — each theme selects a different strategy implementation
    /// with its own corridor, room, and content counts.
    /// </summary>
    public interface IDungeonStrategy
    {
        int CorridorCount { get; }
        int ChamberCount { get; }
        int ItemCount { get; }
        int WeaponCount { get; }
        int EnemyCount { get; }

        int RoomWidth { get; }
        int RoomHeight { get; }

        int CurrencyCount { get; }
        int EnemiesCount { get; }
    }
}
