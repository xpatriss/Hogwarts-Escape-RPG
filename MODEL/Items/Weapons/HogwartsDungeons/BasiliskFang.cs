using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items.Weapons;

namespace projektRPG.MODEL.Items.Weapons.HogwartsDungeons
{
    public class BasiliskFang : LightWeapon
    {
        public override string Name => "Basilisk Fang";
        public override char Symbol => '^';

        public override int RequiredHands => 1;
        public override int Damage => 4;
    }
}
