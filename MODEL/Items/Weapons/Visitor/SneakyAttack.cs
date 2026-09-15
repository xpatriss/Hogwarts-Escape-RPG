using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Weapons;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.Items.Weapons.Visitor
{
    /// <summary>
    /// Stealth attack strategy doubling light weapon potency while sacrificing heavy weapon damage and defense.
    /// Pattern: Visitor (ConcreteVisitor).
    /// </summary>
    public class SneakyAttack : IAttackVisitor
    {
        public string Name => "Sneaky Attack";

        public (int Damage, int Defense) Visit(HeavyWeapon w, Player p)
        {
            int damage = w.Damage / 2 + p.Strength + p.Agression;
            int defense = p.Strength;
            return (damage, defense);
        }

        public (int Damage, int Defense) Visit(LightWeapon w, Player p)
        {
            int damage = w.Damage * 2 + p.Dexterity + p.Luck;
            int defense = p.Dexterity;
            return (damage, defense);
        }

        public (int Damage, int Defense) Visit(MagicWeapon w, Player p)
        {
            return (1, 0);
        }

        public (int Damage, int Defense) Visit(Item i, Player p)
        {
            return (0, 0);
        }
    }
}