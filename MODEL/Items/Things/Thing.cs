using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Weapons.Visitor;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.Items.Things
{
    /// <summary>
    /// Base class for passive collectibles and non-weapon dungeon objects.
    /// </summary>
    public abstract class Thing : Item
    {
        public override int RequiredHands => 0;
        public override string Color => "#00FFFF";

        public override int Damage => 0;
        public override int HealthBonus => 0;
        public override int StrengthBonus => 0;
        public override int DexterityBonus => 0;
        public override int LuckBonus => 0;
        public override int AgressionBonus => 0;
        public override int WisdomBonus => 0;

        public override int NoiseRange => 0;

        public override bool PickUp(Player player)
        {
            return player.Inventory.AddItem(this);
        }

        public override bool Equip(Player player)
        {
            return false;
        }

        public override (int Damage, int Defense) Accept(IAttackVisitor visitor, Player p)
            => visitor.Visit(this, p);
    }
}