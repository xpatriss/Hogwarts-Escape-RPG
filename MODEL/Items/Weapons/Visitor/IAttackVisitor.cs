using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Weapons;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.Items.Weapons.Visitor
{
    /// <summary>
    /// Declares visit operations for each weapon category to calculate damage and defense.
    /// Pattern: Visitor — isolates combat scaling formulas from concrete weapon classes.
    /// </summary>
    public interface IAttackVisitor
    {
        string Name { get; }
        (int Damage, int Defense) Visit(HeavyWeapon weapon, Player p);
        (int Damage, int Defense) Visit(LightWeapon weapon, Player p);
        (int Damage, int Defense) Visit(MagicWeapon weapon, Player p);
        (int Damage, int Defense) Visit(Item item, Player p);
    }
}