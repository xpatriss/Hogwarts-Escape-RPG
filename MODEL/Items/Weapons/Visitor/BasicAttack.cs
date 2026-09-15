using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Weapons;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.Items.Weapons.Visitor
{
    /// <summary>
    /// Standard attack strategy prioritizing strength on heavy weapons and dexterity on light weapons.
    /// Pattern: Visitor (ConcreteVisitor).
    /// </summary>
    public class BasicAttack : IAttackVisitor
    {
        public string Name => "Basic Attack";

        public (int Damage, int Defense) Visit(HeavyWeapon w, Player p)
        {
            int damage = w.Damage + p.Strength + p.Agression;
            int defense = p.Strength + p.Luck;
            return (damage, defense);
        }

        public (int Damage, int Defense) Visit(LightWeapon w, Player p)
        {
            int damage = w.Damage + p.Dexterity + p.Luck;
            int defense = p.Dexterity + p.Luck;
            return (damage, defense);
        }

        public (int Damage, int Defense) Visit(MagicWeapon w, Player p)
        {
            int damage = 1;
            int defense = p.Dexterity + p.Luck;
            return (damage, defense);
        }

        public (int Damage, int Defense) Visit(Item i, Player p)
        {
            return (0, p.Dexterity);
        }
    }
}