using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items.Weapons;

namespace projektRPG.MODEL.Items.Weapons.HogwartsDungeons
{
    public class ElderWand : MagicWeapon
    {
        public override string Name => "Elder Wand";
        public override char Symbol => '*';

        public override int RequiredHands => 1;
        public override int Damage => 3;

    }
}
