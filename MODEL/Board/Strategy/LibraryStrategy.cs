namespace projektRPG.MODEL.Board.Strategy
{
    /// <summary>
    /// Map parameters for Hogwarts Library — fewer corridors, many chambers, lots of items, few enemies.
    /// </summary>
    public class LibraryStrategy : IDungeonStrategy
    {
        public int CorridorCount => 10;
        public int ChamberCount => 20;
        public int ItemCount => 30;
        public int WeaponCount => 5;
        public int EnemyCount => 3;

        public int RoomWidth => 10;
        public int RoomHeight => 5;

        public int CurrencyCount => 15;
        public int EnemiesCount => 5;
    }
}
