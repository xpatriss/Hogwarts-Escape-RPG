using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Weapons;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.Items.Weapons.Visitor
{
    /// <summary>
    /// Arcane combat strategy scaling with player Wisdom when wielding magical weapons.
    /// Pattern: Visitor (ConcreteVisitor).
    /// </summary>
    public class MagicAttack : IAttackVisitor
    {
        public string Name => "Magic Attack";

        public (int Damage, int Defense) Visit(HeavyWeapon w, Player p)
        {
            return (1, p.Luck);
        }

        public (int Damage, int Defense) Visit(LightWeapon w, Player p)
        {
            return (1, p.Luck);
        }

        public (int Damage, int Defense) Visit(MagicWeapon w, Player p)
        {
            int damage = w.Damage + p.Wisdom;
            int defense = p.Wisdom * 2;
            return (damage, defense);
        }

        public (int Damage, int Defense) Visit(Item i, Player p)
        {
            return (0, p.Luck);
        }
    }
}