using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items;

namespace projektRPG.MODEL.Items.Decorator
{
    public class LuckyModifier : ItemDecorator
    {
        public LuckyModifier(Item item) : base(item) { }


        public override string Name => decoratedItem.Name + "(Lucky)";

        public override int LuckBonus => decoratedItem.LuckBonus + 5;
    }
}
