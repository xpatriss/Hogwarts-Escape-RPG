using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Players;
using projektRPG.MODEL.Players.Enemies;

namespace projektRPG.MODEL.Entities.Enemies.EnemyBehavior
{
    /// <summary>Moves toward the last heard noise source until the tile is reached.</summary>
    public class FollowSoundBehavior : IEnemyBehavior
    {
        public void ExecuteMove(Enemy enemy, GameMap map, List<Player> players)
        {
            if (!enemy.SoundTarget.HasValue) return;

            int soundX = enemy.SoundTarget.Value.x;
            int soundY = enemy.SoundTarget.Value.y;

            if (enemy.Position.x == soundX && enemy.Position.y == soundY)
                return;

            int dx = soundX - enemy.Position.x;
            int dy = soundY - enemy.Position.y;

            int stepX = 0;
            int stepY = 0;

            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                if (dx != 0) stepX = dx > 0 ? 1 : -1;
                else if (dy != 0) stepY = dy > 0 ? 1 : -1;
            }
            else
            {
                if (dy != 0) stepY = dy > 0 ? 1 : -1;
                else if (dx != 0) stepX = dx > 0 ? 1 : -1;
            }

            if (enemy.MoveOneStep(map, enemy.Position.x + stepX, enemy.Position.y + stepY))
                return;

            if (stepX != 0)
                enemy.MoveOneStep(map, enemy.Position.x, enemy.Position.y + (dy > 0 ? 1 : dy < 0 ? -1 : 0));
            else if (stepY != 0)
                enemy.MoveOneStep(map, enemy.Position.x + (dx > 0 ? 1 : dx < 0 ? -1 : 0), enemy.Position.y);
        }
    }
}
