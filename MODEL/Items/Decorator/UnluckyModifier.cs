using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items;

namespace projektRPG.MODEL.Items.Decorator
{
    public class UnluckyModifier : ItemDecorator
    {
        public UnluckyModifier(Item item) : base(item) { }


        public override string Name => decoratedItem.Name + "(Unlucky)";

        public override int LuckBonus => decoratedItem.LuckBonus - 5;
    }
}
