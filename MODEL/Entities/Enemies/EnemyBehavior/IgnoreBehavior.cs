using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Players;
using projektRPG.MODEL.Players.Enemies;

namespace projektRPG.MODEL.Entities.Enemies.EnemyBehavior
{
    /// <summary>Does nothing — used by NeutralEnemy before being provoked.</summary>
    public class IgnoreBehavior : IEnemyBehavior
    {
        public void ExecuteMove(Enemy enemy, GameMap map, List<Player> players)
        {
        }
    }
}
