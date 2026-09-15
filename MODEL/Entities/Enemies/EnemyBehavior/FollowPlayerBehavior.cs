using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Items.Weapons.Visitor;
using projektRPG.MODEL.Players;
using projektRPG.MODEL.Players.Enemies;

namespace projektRPG.MODEL.Entities.Enemies.EnemyBehavior
{
    /// <summary>Chases the visible player. Attacks when adjacent (Manhattan distance = 1).</summary>
    public class FollowPlayerBehavior : IEnemyBehavior
    {
        public void ExecuteMove(Enemy enemy, GameMap map, List<Player> players)
        {
            var target = enemy.TargetPlayer;
            if (target == null) return;

            int dx = target.Position.x - enemy.Position.x;
            int dy = target.Position.y - enemy.Position.y;

            if (Math.Abs(dx) + Math.Abs(dy) == 1)
            {
                enemy.PerformAttack(target, new BasicAttack());
                return;
            }

            int stepX = 0;
            int stepY = 0;
            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                if (dx != 0)
                    stepX = dx > 0 ? 1 : -1;
                else if (dy != 0)
                    stepY = dy > 0 ? 1 : -1;
            }
            else
            {
                if (dy != 0)
                    stepY = dy > 0 ? 1 : -1;
                else if (dx != 0)
                    stepX = dx > 0 ? 1 : -1;
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
