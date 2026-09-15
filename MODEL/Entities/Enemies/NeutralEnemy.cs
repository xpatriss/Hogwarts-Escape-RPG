using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Entities.Enemies.EnemyBehavior;
using projektRPG.MODEL.Players;
using projektRPG.MODEL.Players.Events;
using System.Collections.Generic;

namespace projektRPG.MODEL.Players.Enemies
{
    /// <summary>
    /// Neutral enemy archetype (e.g., Slytherin Students / Trolls).
    /// Ignores players and sounds until provoked. Once attacked, reacts aggressively 
    /// if healthy, or turns cowardly (flees) if HP drops below 50%.
    /// </summary>
    public class NeutralEnemy : Enemy
    {
        private bool wasAttacked = false;

        public NeutralEnemy(IDeathPublisher speciesGroup, INoisePublisher playerSubject, IGameEventRecorder eventRecorder, string name)
            : base(name, 22, 42, 3, speciesGroup, playerSubject, eventRecorder)
        {
            Symbol = '◎';
            SoundBehavior = new IgnoreBehavior();
            SightBehavior = new IgnoreBehavior();
        }

        protected override void ReactToOthersDeath()
        {
            RecordServerLog($"{Name} noticed the death of another enemy, but remains indifferent.");
        }

        public override void OnNoiseNotify(object data)
        {
            if (!wasAttacked) return;
            base.OnNoiseNotify(data);
        }

        public override void TakeTurn(GameMap map, List<Player> players)
        {
            // Pre-provocation: patrols randomly, completely ignoring players and noises
            if (!wasAttacked)
            {
                DefaultBehavior.ExecuteMove(this, map, players);
                return;
            }

            base.TakeTurn(map, players);
        }

        public override void ReceiveAttack(int damage)
        {
            base.ReceiveAttack(damage);

            if (HP <= 0) return;

            wasAttacked = true;

            // Switch behavior dynamics depending on remaining health threshold
            if (HP >= maxHP / 2)
            {
                SightBehavior = new FollowPlayerBehavior();
                SoundBehavior = new FollowSoundBehavior();
            }
            else
            {
                SightBehavior = new FleePlayerBehavior();
                SoundBehavior = new FleeSoundBehavior();
            }
        }
    }
}