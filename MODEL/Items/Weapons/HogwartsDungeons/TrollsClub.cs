using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items.Weapons;

namespace projektRPG.MODEL.Items.Weapons.HogwartsDungeons
{
    public class TrollsClub : HeavyWeapon
    {
        public override string Name => "Troll's Club";
        public override char Symbol => '^';

        public override int RequiredHands => 1;
        public override int Damage => 4;
    }
}
