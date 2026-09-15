using Pastel;
using projektRPG.MODEL.Items;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.Items.Weapons
{
    /// <summary>
    /// Base weapon class defining equip behavior and default stat modifiers.
    /// Subclasses categorize weapons for visitor dispatch during combat.
    /// </summary>
    public abstract class Weapon : Item
    {
        public override string Color => "#FD3DB5";

        public override int HealthBonus => 0;
        public override int StrengthBonus => 0;
        public override int DexterityBonus => 0;
        public override int LuckBonus => 0;
        public override int AgressionBonus => 0;
        public override int WisdomBonus => 0;

        public override bool PickUp(Player player)
        {
            return player.Inventory.AddItem(this);
        }

        public override bool Equip(Player player)
        {
            return player.EquipItem(this);
        }
    }
}