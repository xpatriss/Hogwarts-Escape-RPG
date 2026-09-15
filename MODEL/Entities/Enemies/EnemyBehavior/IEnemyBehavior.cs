using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Players;
using projektRPG.MODEL.Players.Enemies;

namespace projektRPG.MODEL.Entities.Enemies.EnemyBehavior
{
    /// <summary>
    /// Defines how an enemy moves during its turn (patrol, chase, flee, etc.).
    /// Pattern: Strategy — Enemy selects a behavior based on sight and sound.
    /// </summary>
    public interface IEnemyBehavior
    {
        void ExecuteMove(Enemy enemy, GameMap map, List<Player> players);
    }
}
