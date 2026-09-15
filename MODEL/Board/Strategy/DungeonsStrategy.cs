namespace projektRPG.MODEL.Board.Strategy
{
    /// <summary>
    /// Map parameters for Hogwarts Dungeons — balanced corridors, chambers, and enemy count.
    /// </summary>
    public class DungeonsStrategy : IDungeonStrategy
    {
        public int CorridorCount => 30;
        public int ChamberCount => 5;
        public int ItemCount => 10;
        public int WeaponCount => 15;
        public int EnemyCount => 10;

        public int RoomWidth => 10;
        public int RoomHeight => 5;

        public int CurrencyCount => 20;
        public int EnemiesCount => 10;
    }
}
