namespace projektRPG.MODEL.Board.Strategy
{
    /// <summary>
    /// Map parameters for the Triwizard Maze — more corridors, slightly fewer coins.
    /// </summary>
    public class MazeStrategy : IDungeonStrategy
    {
        public int CorridorCount => 35;
        public int ChamberCount => 5;
        public int ItemCount => 10;
        public int WeaponCount => 15;
        public int EnemyCount => 10;

        public int RoomWidth => 10;
        public int RoomHeight => 5;

        public int CurrencyCount => 15;
        public int EnemiesCount => 10;
    }
}
