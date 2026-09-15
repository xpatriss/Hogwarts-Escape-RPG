using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Weapons.Visitor;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.Items.Decorator
{
    public class StrongModifier : ItemDecorator
    {
        private const int DamageBonus = 5;

        public StrongModifier(Item item) : base(item) { }

        public override string Name => decoratedItem.Name + "(Strong)";
        public override int Damage => decoratedItem.Damage > 0 ? decoratedItem.Damage + DamageBonus : 0;

        public override (int Damage, int Defense) Accept(IAttackVisitor visitor, Player p)
        {
            var (damage, defense) = decoratedItem.Accept(visitor, p);

            if (decoratedItem.Damage > 0)
                damage += DamageBonus;

            return (damage, defense);
        }
    }
}
