using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pastel;
using projektRPG.MODEL.Items.Weapons.Visitor;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.Items
{
    /// <summary>
    /// Abstract base class for all collectible world items, equipment, and consumables.
    /// Pattern: Visitor — elements accept <see cref="IAttackVisitor"/> to calculate attack and defense metrics.
    /// Pattern: Decorator — serves as the base Component decorated by <see cref="Decorator.ItemDecorator"/>.
    /// </summary>
    public abstract class Item
    {
        public abstract string Name { get; }
        public abstract char Symbol { get; }
        public abstract string Color { get; }
        public abstract int RequiredHands { get; }

        public abstract int HealthBonus { get; }
        public abstract int DexterityBonus { get; }
        public abstract int LuckBonus { get; }
        public abstract int AgressionBonus { get; }
        public abstract int WisdomBonus { get; }

        public abstract int StrengthBonus { get; }
        public abstract int Damage { get; }

        /// <summary>Noise radius emitted when picking up or dropping this item (Observer trigger).</summary>
        public abstract int NoiseRange { get; }

        /// <summary>
        /// Accepts a combat visitor to resolve attack and defense values dynamically.
        /// Pattern: Visitor (Element.Accept).
        /// </summary>
        public abstract (int Damage, int Defense) Accept(IAttackVisitor visitor, Player p);

        public abstract bool PickUp(Player player);
        public abstract bool Equip(Player player);

        public override string ToString()
        {
            var symbol = Symbol.ToString().Pastel(Color);

            if (Damage > 0)
                return $"{Symbol} {Name}({Damage})";

            return $"{Symbol} {Name}";
        }
    }
}