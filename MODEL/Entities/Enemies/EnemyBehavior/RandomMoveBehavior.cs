using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Players;
using projektRPG.MODEL.Players.Enemies;

namespace projektRPG.MODEL.Entities.Enemies.EnemyBehavior
{
    /// <summary>Default patrol: moves one random step up to 6 attempts.</summary>
    public class RandomMoveBehavior : IEnemyBehavior
    {
        private static readonly (int dx, int dy)[] Directions = { (0, -1), (0, 1), (-1, 0), (1, 0) };

        public void ExecuteMove(Enemy enemy, GameMap map, List<Player> players)
        {
            for (int attempt = 0; attempt < 6; attempt++)
            {
                var dir = Directions[Random.Shared.Next(Directions.Length)];
                if (enemy.MoveOneStep(map, enemy.Position.x + dir.dx, enemy.Position.y + dir.dy))
                    return;
            }
        }
    }
}
