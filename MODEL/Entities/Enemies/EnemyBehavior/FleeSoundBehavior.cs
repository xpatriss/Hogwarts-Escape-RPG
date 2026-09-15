using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Players;
using projektRPG.MODEL.Players.Enemies;

namespace projektRPG.MODEL.Entities.Enemies.EnemyBehavior
{
    /// <summary>Moves away from the last heard noise source.</summary>
    public class FleeSoundBehavior : IEnemyBehavior
    {
        private static readonly (int dx, int dy)[] Directions = { (0, -1), (0, 1), (-1, 0), (1, 0) };

        public void ExecuteMove(Enemy enemy, GameMap map, List<Player> players)
        {
            if (!enemy.SoundTarget.HasValue) return;

            int soundX = enemy.SoundTarget.Value.x;
            int soundY = enemy.SoundTarget.Value.y;

            var safeDirections = new List<(int dx, int dy)>();

            foreach (var dir in Directions)
            {
                int newX = enemy.Position.x + dir.dx;
                int newY = enemy.Position.y + dir.dy;

                if (!enemy.CanMoveOneStep(map, newX, newY))
                    continue;

                if (MovesCloserToSound(enemy.Position.x, enemy.Position.y, newX, newY, soundX, soundY))
                    continue;

                safeDirections.Add(dir);
            }

            if (safeDirections.Count == 0)
            {
                enemy.DefaultBehavior.ExecuteMove(enemy, map, players);
                return;
            }

            var chosen = safeDirections[Random.Shared.Next(safeDirections.Count)];
            enemy.MoveOneStep(map, enemy.Position.x + chosen.dx, enemy.Position.y + chosen.dy);
        }

        private static bool MovesCloserToSound(int fromX, int fromY, int toX, int toY, int soundX, int soundY)
        {
            int oldDist = Math.Abs(fromX - soundX) + Math.Abs(fromY - soundY);
            int newDist = Math.Abs(toX - soundX) + Math.Abs(toY - soundY);
            return newDist < oldDist;
        }
    }
}
