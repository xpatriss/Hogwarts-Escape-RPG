using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items.Weapons;

namespace projektRPG.MODEL.Items.Weapons.TriwizardMaze
{
    public class GoldenEgg : LightWeapon
    {
        public override string Name => "Golden Egg";
        public override char Symbol => 'o';

        public override int RequiredHands => 1;
        public override int Damage => 4;
    }
}
