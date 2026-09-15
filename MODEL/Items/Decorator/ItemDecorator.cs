using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pastel;
using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Weapons.Visitor;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.Items.Decorator
{
    /// <summary>
    /// Abstract decorator extending Item properties dynamically at runtime.
    /// Pattern: Decorator (Decorator base) — maintains a reference to a wrapped Component 
    /// and delegates default behavior while allowing subclasses to alter stats.
    /// </summary>
    public abstract class ItemDecorator : Item
    {
        protected Item decoratedItem;

        protected ItemDecorator(Item item) : base()
        {
            decoratedItem = item;
        }

        public override string Name => decoratedItem.Name;
        public override char Symbol => decoratedItem.Symbol;
        public override int RequiredHands => decoratedItem.RequiredHands;

        public override string Color => decoratedItem.Color;

        public override int Damage => decoratedItem.Damage;
        public override int HealthBonus => decoratedItem.HealthBonus;
        public override int StrengthBonus => decoratedItem.StrengthBonus;
        public override int DexterityBonus => decoratedItem.DexterityBonus;
        public override int LuckBonus => decoratedItem.LuckBonus;
        public override int AgressionBonus => decoratedItem.AgressionBonus;
        public override int WisdomBonus => decoratedItem.WisdomBonus;

        public override int NoiseRange => decoratedItem.NoiseRange;

        public override bool PickUp(Player player)
        {
            return player.Inventory.AddItem(this);
        }

        public override bool Equip(Player player)
        {
            if (player.LeftHand != null && player.RightHand != null)
            {
                return false;
            }

            if (RequiredHands == 2)
            {
                if (player.LeftHand == null && player.RightHand == null)
                {
                    player.LeftHand = this;
                    player.RightHand = this;
                    player.Inventory.RemoveItem(this);
                    player.UpdateAttributes();
                    return true;
                }
                else return false;
            }
            else
            {
                if (player.LeftHand == null)
                {
                    player.LeftHand = this;
                }
                else
                {
                    player.RightHand = this;
                }
                player.Inventory.RemoveItem(this);
                player.UpdateAttributes();
                return true;
            }
        }

        public override string ToString()
        {
            var symbol = Symbol.ToString().Pastel(Color);

            if (Damage > 0)
                return $"{Symbol} {Name}({Damage})";

            return $"{Symbol} {Name}";
        }

        public override (int Damage, int Defense) Accept(IAttackVisitor visitor, Player p)
        {
            return decoratedItem.Accept(visitor, p);
        }
    }
}