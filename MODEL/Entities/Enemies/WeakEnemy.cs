using projektRPG.MODEL.Entities.Enemies.EnemyBehavior;
using projektRPG.MODEL.Players.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace projektRPG.MODEL.Players.Enemies
{
    /// <summary>
    /// Timid enemy archetype (e.g., Dementors).
    /// Flees from visible players and sound sources. 
    /// Weakens (loses attack and armor) when witnessing the death of a companion.
    /// </summary>
    public class WeakEnemy : Enemy
    {
        public WeakEnemy(IDeathPublisher speciesGroup, INoisePublisher playerSubject, IGameEventRecorder eventRecorder, string name)
            : base(name, 24, 36, 2, speciesGroup, playerSubject, eventRecorder)
        {
            Symbol = '☥';
            SoundBehavior = new FleeSoundBehavior();
            SightBehavior = new FleePlayerBehavior();
        }

        protected override void ReactToOthersDeath()
        {
            RecordServerLog($"{Name} noticed the death of another enemy and grows weaker.");
            Attack -= 2;
            Armor -= 1;
        }
    }
}