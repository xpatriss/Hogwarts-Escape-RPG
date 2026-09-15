using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Items.Weapons;

namespace projektRPG.MODEL.Items.Weapons.TriwizardMaze
{
    public class SwordOfGryffindor : HeavyWeapon
    {
        public override string Name => "Sword of Gryffindor";
        public override char Symbol => '♱';

        public override int RequiredHands => 2;
        public override int Damage => 5;
    }
}
