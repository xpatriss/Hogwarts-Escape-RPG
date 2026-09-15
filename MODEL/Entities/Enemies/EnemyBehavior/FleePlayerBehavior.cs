using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Players;
using projektRPG.MODEL.Players.Enemies;

namespace projektRPG.MODEL.Entities.Enemies.EnemyBehavior
{
    /// <summary>
    /// Runs away from visible players. Picks a direction that does not reduce Manhattan distance.
    /// Falls back to random movement if cornered (unless surrounded on all four sides).
    /// </summary>
    public class FleePlayerBehavior : IEnemyBehavior
    {
        private static readonly (int dx, int dy)[] Directions = { (0, -1), (0, 1), (-1, 0), (1, 0) };

        public void ExecuteMove(Enemy enemy, GameMap map, List<Player> players)
        {
            var visiblePlayers = GetVisiblePlayers(enemy, map, players);
            if (visiblePlayers.Count == 0) return;

            var safeDirections = new List<(int dx, int dy)>();

            foreach (var dir in Directions)
            {
                int newX = enemy.Position.x + dir.dx;
                int newY = enemy.Position.y + dir.dy;

                if (!enemy.CanMoveOneStep(map, newX, newY))
                    continue;

                if (MovesCloserToAnyPlayer(enemy.Position.x, enemy.Position.y, newX, newY, visiblePlayers))
                    continue;

                safeDirections.Add(dir);
            }

            if (safeDirections.Count == 0)
            {
                if (IsSurroundedByFourPlayers(enemy, players))
                    return;

                enemy.DefaultBehavior.ExecuteMove(enemy, map, players);
                return;
            }

            var chosen = safeDirections[Random.Shared.Next(safeDirections.Count)];
            enemy.MoveOneStep(map, enemy.Position.x + chosen.dx, enemy.Position.y + chosen.dy);
        }

        private static bool IsSurroundedByFourPlayers(Enemy enemy, List<Player> players)
        {
            int adjacentPlayers = 0;

            foreach (var dir in Directions)
            {
                int checkX = enemy.Position.x + dir.dx;
                int checkY = enemy.Position.y + dir.dy;

                if (players.Any(p => p.Position.x == checkX && p.Position.y == checkY))
                    adjacentPlayers++;
            }

            return adjacentPlayers == 4;
        }

        private static bool MovesCloserToAnyPlayer(int fromX, int fromY, int toX, int toY, List<Player> visiblePlayers)
        {
            foreach (var player in visiblePlayers)
            {
                int oldDist = Math.Abs(fromX - player.Position.x) + Math.Abs(fromY - player.Position.y);
                int newDist = Math.Abs(toX - player.Position.x) + Math.Abs(toY - player.Position.y);
                if (newDist < oldDist)
                    return true;
            }
            return false;
        }

        private static List<Player> GetVisiblePlayers(Enemy enemy, GameMap map, List<Player> players)
        {
            var visible = new List<Player>();
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                for (int step = 1; step <= enemy.VisionRange; step++)
                {
                    int checkX = enemy.Position.x + dx[i] * step;
                    int checkY = enemy.Position.y + dy[i] * step;

                    if (!map.CanMoveTo(checkX, checkY))
                        break;

                    var player = players.FirstOrDefault(p => p.Position.x == checkX && p.Position.y == checkY);
                    if (player != null)
                    {
                        if (!visible.Contains(player))
                            visible.Add(player);
                        break;
                    }
                }
            }

            return visible;
        }
    }
}
