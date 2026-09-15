using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Entities.Enemies.EnemyBehavior;
using projektRPG.MODEL.Players.Events;

namespace projektRPG.MODEL.Players.Enemies
{
    /// <summary>
    /// Aggressive enemy archetype (e.g., Death Eaters).
    /// Chases visible players and investigates sounds aggressively. 
    /// Gains attack and armor buffs when witnessing the death of a species companion.
    /// </summary>
    public class BraveEnemy : Enemy
    {
        public BraveEnemy(IDeathPublisher speciesGroup, INoisePublisher playerSubject, IGameEventRecorder eventRecorder, string name)
            : base(name, 28, 48, 5, speciesGroup, playerSubject, eventRecorder)
        {
            Symbol = '☬';
            SoundBehavior = new FollowSoundBehavior();
            SightBehavior = new FollowPlayerBehavior();
        }

        protected override void ReactToOthersDeath()
        {
            RecordServerLog($"{Name} noticed the death of another enemy and flew into a rage.");
            Attack += 2;
            Armor += 1;
        }
    }
}