using System.Text;
using projektRPG.Infrastructure;
using projektRPG.MODEL.Items;
using projektRPG.MODEL.Players.Enemies;

namespace projektRPG.MODEL.Board.Map
{
    /// <summary>
    /// Base type for a single map tile. Holds optional enemy and item stack.
    /// Subclasses define whether the tile is walkable (Empty) or blocked (Wall).
    /// </summary>
    public abstract class Field
    {
        public abstract bool IsAvailable { get; }
        public abstract char Symbol { get; }

        public Enemy? enemy { get; private set; }

        private List<Item> items = new List<Item>();

        public IReadOnlyList<Item> Items => items;

        public bool AddItem(Item item)
        {
            if (items.Count >= Consts.FieldCapacity)
            {
                return false;
            }
            items.Add(item);
            return true;
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }

        public void RemoveItemAt(int index)
        {
            if (index >= 0 && index < items.Count)
            {
                items.RemoveAt(index);
            }
        }

        public void AddEnemy(Enemy enemy)
        {
            this.enemy = enemy;
            enemy.SetField(this);
        }

        public void RemoveEnemy()
        {
            enemy = null;
        }

        public bool HasEnemy()
        {
            return enemy != null;
        }

        /// <summary>Returns a multi-line description shown in the field info panel (enemy stats or item list).</summary>
        public virtual string Info()
        {
            if (items.Count == 0 && enemy == null)
                return "";

            if (enemy != null)
                return $"ENEMY: {enemy.Name} \n(HP: {enemy.HP}   Attack: {enemy.Attack}   Armor: {enemy.Armor})";

            StringBuilder sb = new StringBuilder();
            foreach (var item in items)
            {
                sb.Append(item.ToString());
                sb.Append('\n');
            }

            return sb.ToString();
        }
    }
}
