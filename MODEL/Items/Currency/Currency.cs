using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Weapons.Visitor;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.Items.Currency
{
    /// <summary>
    /// Base class for currency types (coins, gold) directly credited to player totals upon pickup.
    /// </summary>
    public abstract class Currency : Item
    {
        public override int RequiredHands => 0;
        public override string Color => "#FFD700";

        public override int Damage => 0;
        public override int HealthBonus => 0;
        public override int StrengthBonus => 0;
        public override int DexterityBonus => 0;
        public override int LuckBonus => 0;
        public override int AgressionBonus => 0;
        public override int WisdomBonus => 0;

        public override int NoiseRange => 0;

        public override bool Equip(Player player)
        {
            return false;
        }

        public override (int Damage, int Defense) Accept(IAttackVisitor visitor, Player p)
            => visitor.Visit(this, p);
    }
}