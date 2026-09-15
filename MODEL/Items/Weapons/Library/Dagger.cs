using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items.Weapons;

namespace projektRPG.MODEL.Items.Weapons.Library
{
    public class Dagger : HeavyWeapon
    {
        public override string Name => "Dagger";
        public override char Symbol => '♱';

        public override int RequiredHands => 2;
        public override int Damage => 5;
    }
}
