using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Items.Weapons.Visitor;
using projektRPG.MODEL.Players;
using projektRPG.MODEL.Players.Events;
using projektRPG.MODEL.Entities.Enemies.EnemyBehavior;

namespace projektRPG.MODEL.Players.Enemies
{
    /// <summary>
    /// Abstract base class for all dungeon enemies.
    /// Pattern: Observer — implements IDeathObserver (reacts to species deaths) 
    /// and INoiseObserver (reacts to player noise propagation).
    /// Uses the Strategy pattern via IEnemyBehavior to dynamically dictate movement and combat ticks.
    /// </summary>
    public abstract class Enemy : IDeathObserver, INoiseObserver
    {
        public string Name { get; set; }

        public char Symbol { get; set; } = '☠';

        public string Color { get; set; } = "#ffffff";
        public (int x, int y) Position { get; private set; } = (0, 0);

        private Field? currentField;

        public int Attack { get; protected set; }
        public int HP { get; protected set; }
        public int maxHP { get; protected set; }
        public int Armor { get; protected set; }

        protected IDeathPublisher speciesGroup;
        protected INoisePublisher modelSubject;
        protected IGameEventRecorder eventRecorder;

        public IEnemyBehavior DefaultBehavior { get; set; }
        public IEnemyBehavior SoundBehavior { get; set; }
        public IEnemyBehavior SightBehavior { get; set; }

        private IEnemyBehavior currentBehavior;

        /// <summary>Vision range in tiles for line-of-sight player detection.</summary>
        public int VisionRange { get; protected set; } = 5;

        /// <summary>The player currently spotted within vision range.</summary>
        public Player TargetPlayer { get; protected set; }

        /// <summary>Coordinates of the most recently heard noise source.</summary>
        public (int x, int y)? SoundTarget { get; protected set; }

        /// <summary>Reference to the player attacked during the current turn (processed by server controller).</summary>
        public Player? LastAttackedPlayer { get; private set; }
        public int LastAttackDamage { get; private set; }

        protected Enemy(string name, int attack, int hP, int armor, IDeathPublisher speciesGroup, INoisePublisher modelSubject, IGameEventRecorder eventRecorder)
        {
            Name = name;
            Attack = attack;
            HP = hP;
            maxHP = hP;
            Armor = armor;
            this.speciesGroup = speciesGroup;
            this.modelSubject = modelSubject;
            this.eventRecorder = eventRecorder;

            speciesGroup.Attach(this);
            modelSubject.Attach(this);

            DefaultBehavior = new RandomMoveBehavior();
        }

        /// <summary>Executes a physical strike against a target player.</summary>
        public int PerformAttack(Player player, IAttackVisitor attack)
        {
            LastAttackedPlayer = player;
            LastAttackDamage = player.ReceiveAttack(Attack, attack);
            return LastAttackDamage;
        }

        /// <summary>Applies incoming damage, accounting for enemy armor mitigation.</summary>
        public virtual void ReceiveAttack(int damage)
        {
            int realDamage = damage - Armor;
            if (realDamage < 0)
                realDamage = 0;
            HP -= realDamage;

            if (HP <= 0)
                OnDie();
        }

        public void SetPosition(int x, int y)
        {
            Position = (x, y);
        }

        public void SetField(Field field)
        {
            currentField = field;
        }

        public bool CanMoveOneStep(GameMap map, int targetX, int targetY)
        {
            if (!map.CanMoveTo(targetX, targetY))
                return false;

            return !map.GetField(targetX, targetY).HasEnemy();
        }

        public bool MoveOneStep(GameMap map, int targetX, int targetY)
        {
            if (!CanMoveOneStep(map, targetX, targetY))
                return false;

            currentField?.RemoveEnemy();
            map.GetField(targetX, targetY).AddEnemy(this);
            SetPosition(targetX, targetY);
            return true;
        }

        public void OnDeathNotify(object data)
        {
            ReactToOthersDeath();
        }

        public virtual void OnNoiseNotify(object data)
        {
            var noiseData = ((int x, int y, int range, GameMap map))data;
            HandleNoise(noiseData.x, noiseData.y, noiseData.range, noiseData.map);
        }

        protected abstract void ReactToOthersDeath();

        protected void RecordServerLog(string message) => eventRecorder.RecordServerLog(message);

        private void HandleNoise(int sourceX, int sourceY, int range, GameMap map)
        {
            int distance = CalculatePathDistance(sourceX, sourceY, Position.x, Position.y, map, range);

            if (distance <= range && distance != -1)
            {
                RecordServerLog($"{Name} at ({Position.x},{Position.y}) heard noise (distance: {distance}, source: {sourceX},{sourceY})!");
                SoundTarget = (sourceX, sourceY);
            }
        }

        private int CalculatePathDistance(int startX, int startY, int endX, int endY, GameMap map, int maxRange)
        {
            if (Math.Abs(startX - endX) + Math.Abs(startY - endY) > maxRange) return -1;

            var queue = new Queue<(int x, int y, int dist)>();
            var visited = new HashSet<(int, int)>();

            queue.Enqueue((startX, startY, 0));
            visited.Add((startX, startY));

            while (queue.Count > 0)
            {
                var (currX, currY, currDist) = queue.Dequeue();

                if (currX == endX && currY == endY)
                    return currDist;
                if (currDist >= maxRange)
                    continue;

                int[] dx = { 0, 0, 1, -1 };
                int[] dy = { 1, -1, 0, 0 };

                for (int i = 0; i < 4; i++)
                {
                    int nextX = currX + dx[i];
                    int nextY = currY + dy[i];

                    if (map.CanMoveTo(nextX, nextY) && !visited.Contains((nextX, nextY)))
                    {
                        visited.Add((nextX, nextY));
                        queue.Enqueue((nextX, nextY, currDist + 1));
                    }
                }
            }
            return -1;
        }

        /// <summary>Handles enemy death, detaches observers, and broadcasts death notifications.</summary>
        public void OnDie()
        {
            RecordServerLog($"Enemy {Name} defeated");

            speciesGroup.Detach(this);
            modelSubject.Detach(this);

            speciesGroup.Notify(this);
        }

        /// <summary>
        /// Executes a single enemy AI turn. Evaluates sight and sound priorities 
        /// to select the appropriate behavior strategy.
        /// </summary>
        public virtual void TakeTurn(GameMap map, List<Player> players)
        {
            LastAttackedPlayer = null;
            LastAttackDamage = 0;

            if (IsPlayerInSight(map, players))
            {
                currentBehavior = SightBehavior;
                SoundTarget = null;
            }
            else if (HasRecentSoundTarget())
            {
                currentBehavior = SoundBehavior;
            }
            else
            {
                currentBehavior = DefaultBehavior;
            }

            currentBehavior.ExecuteMove(this, map, players);
        }

        protected bool IsPlayerInSight(GameMap map, List<Player> players)
        {
            TargetPlayer = null;
            int minDistance = int.MaxValue;

            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                for (int step = 1; step <= VisionRange; step++)
                {
                    int checkX = Position.x + (dx[i] * step);
                    int checkY = Position.y + (dy[i] * step);

                    // Stop raycasting if a wall is encountered
                    if (!map.CanMoveTo(checkX, checkY))
                        break;

                    var player = players.FirstOrDefault(p => p.Position.x == checkX && p.Position.y == checkY);
                    if (player != null)
                    {
                        if (step < minDistance)
                        {
                            minDistance = step;
                            TargetPlayer = player;
                        }
                        break;
                    }
                }
            }

            return TargetPlayer != null;
        }

        protected bool HasRecentSoundTarget()
        {
            // Forget sound target once reached
            if (SoundTarget.HasValue && SoundTarget.Value.x == Position.x && SoundTarget.Value.y == Position.y)
            {
                SoundTarget = null;
            }

            return SoundTarget.HasValue;
        }
    }
}