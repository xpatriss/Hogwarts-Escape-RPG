using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items.Weapons;

namespace projektRPG.MODEL.Items.Weapons.Library
{
    public class Quill : LightWeapon
    {
        public override string Name => "Quill";
        public override char Symbol => 'q';

        public override int RequiredHands => 1;
        public override int Damage => 3;

    }
}
