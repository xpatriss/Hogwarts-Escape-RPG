using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items;

namespace projektRPG.MODEL.Items.Decorator
{
    public class HealthyModifier : ItemDecorator
    {
        public HealthyModifier(Item item) : base(item) { }

        public override string Name => decoratedItem.Name + "(Healthy)";
        public override int HealthBonus => decoratedItem.HealthBonus + 2;
    }
}
