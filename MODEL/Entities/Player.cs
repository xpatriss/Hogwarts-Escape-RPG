using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Weapons.Visitor;
using projektRPG.MODEL.Players.Enemies;

namespace projektRPG.MODEL.Players
{
    /// <summary>
    /// Represents a connected player on the server. Holds stats, inventory, equipped hands,
    /// and pending log messages sent to the client each tick.
    /// Combat uses the Visitor pattern: weapons Accept() an IAttackVisitor to compute damage/defense.
    /// </summary>
    public class Player
    {
        public string Name { get; set; }

        /// <summary>First letter of the name, shown on the map for multiplayer.</summary>
        public char Symbol { get; set; }

        public int Id { get; set; }

        /// <summary>Messages queued for the next personalized GameStateDTO sent to this player.</summary>
        public List<string> PendingLogs { get; private set; } = new List<string>();

        public (int x, int y) Position { get; private set; } = (0, 0);

        public int Health { get; private set; }
        public int Strength { get; private set; }
        public int Dexterity { get; private set; }
        public int Luck { get; private set; }
        public int Agression { get; private set; }
        public int Wisdom { get; private set; }

        public int baseHealth { get; private set; } = 30;
        private int baseStrength = 10;
        private int baseDexterity = 10;
        private int baseLuck = 10;
        private int baseAgression = 10;
        private int baseWisdom = 10;

        public Equipment Inventory { get; private set; }
        public Item? LeftHand { get; set; }
        public Item? RightHand { get; set; }

        public int Coins { get; set; }
        public int Gold { get; set; }

        public Player(string name, (int, int) position)
        {
            Name = name;
            Position = position;
            Health = baseHealth;
            Strength = baseStrength;
            Dexterity = baseDexterity;
            Luck = baseLuck;
            Agression = baseAgression;
            Wisdom = baseWisdom;
            Inventory = new Equipment();
            LeftHand = null;
            RightHand = null;

            Coins = 0;
            Gold = 0;
        }

        public void Walk(int x, int y)
        {
            Position = (x, y);
        }

        /// <summary>Picks up an item into inventory. Returns noise range if the action succeeds (Observer trigger).</summary>
        public bool TakeItem(Item item, out int noise)
        {
            noise = 0;
            bool canPickUp = item.PickUp(this);
            UpdateAttributes();
            if (canPickUp) noise = item.NoiseRange;
            return canPickUp;
        }

        /// <summary>Drops an inventory item by index. Returns noise range of the dropped item.</summary>
        public Item? ThrowItem(int id, out int noise)
        {
            noise = 0;
            if (id >= 0 && id < Inventory.EquipmentList.Count)
            {
                Item thrownItem = Inventory.EquipmentList[id];
                Inventory.RemoveItemAt(id);
                UpdateAttributes();
                noise = thrownItem.NoiseRange;
                return thrownItem;
            }

            return null;
        }

        public bool EquipItem(Item item)
        {
            if (LeftHand != null && RightHand != null)
            {
                return false;
            }

            if (item.RequiredHands == 2)
            {
                if (LeftHand == null && RightHand == null)
                {
                    LeftHand = item;
                    RightHand = item;
                    Inventory.RemoveItem(item);
                    UpdateAttributes();
                    return true;
                }
                else return false;
            }
            else
            {
                if (LeftHand == null)
                {
                    LeftHand = item;
                }
                else
                {
                    RightHand = item;
                }
                Inventory.RemoveItem(item);
                UpdateAttributes();
                return true;
            }
        }

        /// <summary>Unequips a weapon from hand(s) back into inventory. Fails if inventory is full.</summary>
        public bool FreeHand()
        {
            if (LeftHand != null)
            {
                if (Inventory.AddItem(LeftHand) == false)
                    return false;

                if (LeftHand.RequiredHands == 2)
                {
                    LeftHand = null;
                    RightHand = null;
                }
                else LeftHand = null;
            }
            else if (RightHand != null)
            {
                if (Inventory.AddItem(RightHand) == false)
                    return false;
                RightHand = null;
            }

            UpdateAttributes();

            return true;
        }

        /// <summary>Recalculates stats from base values plus bonuses from equipped items and inventory.</summary>
        public void UpdateAttributes()
        {
            int healthBonus = 0;
            int strengthBonus = 0;
            int dexterityBonus = 0;
            int luckBonus = 0;
            int agressionBonus = 0;
            int wisdomBonus = 0;
            if (LeftHand != null)
            {
                healthBonus += LeftHand.HealthBonus;
                strengthBonus += LeftHand.StrengthBonus;
                dexterityBonus += LeftHand.DexterityBonus;
                luckBonus += LeftHand.LuckBonus;
                agressionBonus += LeftHand.AgressionBonus;
                wisdomBonus += LeftHand.WisdomBonus;

                if (LeftHand.RequiredHands == 2)
                {
                    foreach (var item in Inventory.EquipmentList)
                    {
                        healthBonus += item.HealthBonus;
                        strengthBonus += item.StrengthBonus;
                        dexterityBonus += item.DexterityBonus;
                        luckBonus += item.LuckBonus;
                        agressionBonus += item.AgressionBonus;
                        wisdomBonus += item.WisdomBonus;
                    }

                    Health = baseHealth + healthBonus;
                    Strength = baseStrength + strengthBonus;
                    Dexterity = baseDexterity + dexterityBonus;
                    Luck = baseLuck + luckBonus;
                    Agression = baseAgression + agressionBonus;
                    Wisdom = baseWisdom + wisdomBonus;

                    return;
                }
            }
            if (RightHand != null)
            {
                healthBonus += RightHand.HealthBonus;
                strengthBonus += RightHand.StrengthBonus;
                dexterityBonus += RightHand.DexterityBonus;
                luckBonus += RightHand.LuckBonus;
                agressionBonus += RightHand.AgressionBonus;
                wisdomBonus += RightHand.WisdomBonus;
            }

            foreach (var item in Inventory.EquipmentList)
            {
                healthBonus += item.HealthBonus;
                strengthBonus += item.StrengthBonus;
                dexterityBonus += item.DexterityBonus;
                luckBonus += item.LuckBonus;
                agressionBonus += item.AgressionBonus;
                wisdomBonus += item.WisdomBonus;
            }

            Health = baseHealth + healthBonus;
            Strength = baseStrength + strengthBonus;
            Dexterity = baseDexterity + dexterityBonus;
            Luck = baseLuck + luckBonus;
            Agression = baseAgression + agressionBonus;
            Wisdom = baseWisdom + wisdomBonus;
        }

        /// <summary>
        /// Applies incoming damage after weapon defense (Visitor). Returns actual damage taken.
        /// Two-handed weapons use a single defense value instead of summing both hands.
        /// </summary>
        public int ReceiveAttack(int damage, IAttackVisitor attack)
        {
            UpdateAttributes();

            int left = 0;
            int right = 0;
            int total = 0;

            int realDamage = damage;

            if (LeftHand != null)
            {
                left = LeftHand.Accept(attack, this).Defense;
            }
            if (RightHand != null)
            {
                right = RightHand.Accept(attack, this).Defense;
            }

            if (LeftHand != null && RightHand != null && LeftHand.RequiredHands == 2)
            {
                total = left;
            }
            else
            {
                total = left + right;
            }

            realDamage -= total;

            if (realDamage < 0)
                realDamage = 0;

            baseHealth -= realDamage;
            if (baseHealth < 0)
                baseHealth = 0;

            UpdateAttributes();
            return realDamage;
        }

        /// <summary>
        /// Attacks an enemy using equipped weapon(s) and the selected attack type (Visitor).
        /// </summary>
        public void PerformAttack(Enemy enemy, IAttackVisitor attack)
        {
            UpdateAttributes();

            if (enemy == null) return;

            int left = 0;
            int right = 0;
            int total = 0;

            if (LeftHand != null)
            {
                left = LeftHand.Accept(attack, this).Damage;
            }
            if (RightHand != null)
            {
                right = RightHand.Accept(attack, this).Damage;
            }

            if (LeftHand != null && RightHand != null && LeftHand.RequiredHands == 2)
            {
                total = left;
            }
            else
            {
                total = left + right;
            }

            enemy.ReceiveAttack(total);
        }
    }
}
